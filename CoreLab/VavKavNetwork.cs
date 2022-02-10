using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLab
{
    class VavKavNetwork
    {
        static float stepFactor = 0.001f;

        float[] InputNeurons { get; set; }
        float[][] Values { get; set; }
        float[][] Biases { get; set; }
        float[][][] Weights { get; set; }
        float[][] BiasChanges { get; set; }
        float[][][] WeightChanges { get; set; }
        static Random rnd = new Random();
        public float Cost { get; private set; }

        public VavKavNetwork(int[] Layers)
        {
            //check inputs
            if (Layers.Length < 3) { throw new ArgumentException("Invalid assignment. (wrong number of layres)"); }
            foreach (int num in Layers)
            {
                if (num < 1) { throw new ArgumentException("Invalid assignment. (wrong number of neurons)"); }
            }

            //initialize arrays
            InputNeurons = new float[Layers[0]];
            Values = new float[Layers.Length - 1][];
            Biases = new float[Layers.Length - 1][];
            Weights = new float[Layers.Length - 1][][];
            WeightChanges = new float[Layers.Length - 1][][];
            BiasChanges = new float[Layers.Length - 1][];

            for (int i = 1; i < Layers.Length; i++)
            {
                Values[i] = new float[Layers[i]];
                Biases[i] = new float[Layers[i]];
                BiasChanges[i] = new float[Layers[i]];
                Weights[i] = new float[Layers[i]][];
                WeightChanges[i] = new float[Layers[i]][];
                for (int j = 0; j < Layers[i]; j++)
                {
                    Weights[i][j] = new float[Layers[i - 1]];
                    WeightChanges[i][j] = new float[Layers[i - 1]];
                    Biases[i][j] = 0f;
                    BiasChanges[i][j] = 0f;
                }
            }

            //initialize weights
            for (int i = 0; i < Weights[Weights.Length - 1].Length; i++)//last layer
            {
                for (int j = 0; j < Weights[Weights.Length - 1][i].Length; j++)
                {
                    Weights[Weights.Length - 1][i][j] = SampleGaussian(0, (float)Math.Sqrt(2 / (Layers[Layers.Length - 2] + Layers[Layers.Length - 1])));
                }
            }

            for (int i = 0; i < Weights.Length - 1; i++)//other layers
            {
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    for (int k = 0; k < Weights[i][j].Length; k++)
                    {
                        Weights[i][j][k] = SampleGaussian(0, (float)(2 / Math.Sqrt(Layers[i] + Layers[i + 1])));
                    }
                }
            }
        }

        public float[] Result(float[] Inputs)
        {
            if (InputNeurons.Length != Inputs.Length) { throw new ArgumentException($"Invalid number of inputs. ({InputNeurons.Length} neurons in input layer)"); }

            float[] final = new float[Values[Values.Length - 1].Length];

            for (int i = 0; i < Inputs.Length; i++)
            {
                InputNeurons[i] = Inputs[i];
            }

            float value;
            for (int i = 0; i < Values.Length - 1; i++) //all but the last
            {
                for (int j = 0; j < Values[i].Length; j++)
                {
                    value = 0;
                    for (int k = 0; k < Weights[i][j].Length; k++)//adding 
                    {
                        value += Weights[i][j][k] * (i == 0 ? InputNeurons[k] : Values[i - 1][k]);
                    }
                    value += Biases[i][j];
                    Values[i][j] = ReLU(value);
                }
            }

            for (int i = 0; i < Values[Values.Length - 1].Length; i++)
            {
                value = 0;
                for (int j = 0; j < Values[Values.Length - 2].Length; j++)
                {
                    value += Values[Values.Length - 1][j] * Weights[Weights.Length - 1][i][j];
                }
                value += Biases[Biases.Length - 1][i];
                Values[Values.Length - 1][i] = Sigmoid(value);
            }

            for (int i = 0; i < final.Length; i++)
            {
                final[i] = Values[Values.Length - 1][i];
            }
            return final;
        }

        public void Train(Dictionary<float[], float[]> Inputs, int Iterations, bool DisplayProgress = true) //Inputs is list of input-output pairs consisting of float arrays
        {
            //check inputs
            foreach (KeyValuePair<float[], float[]> pair in Inputs)
            {
                if (pair.Key.Length != InputNeurons.Length || pair.Value.Length != Values[Values.Length - 1].Length) { throw new ArgumentException("Invalid input or output elements of an input-output pair!"); }
                foreach (float result in pair.Value)
                {
                    if (result < 0 || result > 1) { throw new ArgumentException("Some result is greater than 1 or smaller than 0!"); }
                }
            }
            if (Iterations < 1) { throw new ArgumentException("At least one iteration is required!"); }

            //devide into batches
            List<Dictionary<float[], float[]>> batches = new List<Dictionary<float[], float[]>>();
            int num = 0;
            batches.Add(new Dictionary<float[], float[]>());
            foreach (KeyValuePair<float[], float[]> pair in Inputs)
            {
                if (num == 100) //limit for batch population
                {
                    num = 0;
                    batches.Add(new Dictionary<float[], float[]>());
                }
                batches[batches.Count - 1].Add(pair.Key, pair.Value);
                num++;
            }

            //training itself
            int lastDisplay = 0;
            int lastCheck = 0;
            string lastDisplayedString = "";
            string currentString;
            float error, referenceError;
            float lastDif = 0;
            int suspectCount = 0;

            for (int Iter = 0; Iter < Iterations; Iter++)
            {
                lastCheck++;
                lastDisplay++;

                //display progress
                currentString = $"progress: {Iter * 100 / Iterations}%";
                if (DisplayProgress && lastDisplay % 500 == 0 && currentString != lastDisplayedString) //display after x iterations
                {
                    lastDisplay = 0;
                    Console.Clear();
                    Console.WriteLine(currentString);
                    lastDisplayedString = currentString;
                }

                //check stagnation
                if (lastCheck % 1000 == 0 && lastCheck > 1)
                {
                    lastCheck = 0;

                    int countHere = 0;
                    float[] lastOutput = null, curent, lastExpected = null;
                    foreach (KeyValuePair<float[], float[]> pair in batches[0])
                    {
                        curent = Result(pair.Key);
                        if (lastExpected != null && !EqualContents(pair.Value, lastExpected) && EqualContents(lastOutput, curent))
                        {
                            countHere++;

                        }
                        lastOutput = curent;
                        lastExpected = pair.Value;
                    }
                    if (countHere > 0) { suspectCount++; }
                    if (suspectCount >= 1 + Iterations / 9000)
                    {
                        //Training stagnates!
                        Iter = Iterations; //finish this iteration and end training
                    }
                }


                foreach (Dictionary<float[], float[]> batch in batches)
                {
                    foreach (KeyValuePair<float[], float[]> pair in batch)
                    {

                        float[] changesToPrevious = new float[] { };
                        float[] presentChanges = new float[] { };
                        float changeWanted;
                        float[] output = Result(pair.Key);
                        for (int layer = Neurons.Count - 1; layer >= 0; layer--)
                        {

                            bool notLastLayer = layer > 0;

                            if (notLastLayer)
                            {
                                changesToPrevious = new float[Neurons[layer - 1].Length];
                                for (int i = 0; i < changesToPrevious.Length; i++)
                                {
                                    changesToPrevious[i] = 0;
                                }
                                if (layer == Neurons.Count - 1) { presentChanges = new float[Neurons[layer - 1].Length]; }
                            }
                            else { changesToPrevious = new float[] { }; } //on the last layer you don't have to note any changes


                            float cost = 0;
                            for (int i = 0; i < Neurons[Neurons.Count - 1].Length; i++)
                            {
                                cost += Math.Pow(pair.Value[i] - Neurons[Neurons.Count - 1][i].Value, 2);
                            }

                            for (int index = 0; index < Neurons[layer].Length; index++)
                            {
                                changeWanted = (layer == Neurons.Count - 1 ? pair.Value[index] - Neurons[layer][index].Value : presentChanges[index]);

                                //bias
                                Neurons[layer][index].BiasChange += changeWanted * cost;
                                //synapses
                                for (int i = 0; i < Neurons[layer][index].InputSynapseValuesChange.Count; i++)
                                {
                                    Neurons[layer][index].InputSynapseValuesChange[i] += changeWanted * (notLastLayer ? Neurons[layer - 1][i].Value : Perceptrons[i].Value) * cost;
                                }
                                //previous layer
                                if (notLastLayer)
                                {
                                    for (int i = 0; i < changesToPrevious.Length; i++)
                                    {
                                        changesToPrevious[i] += changeWanted * (Neurons[layer][index].InputSynapseValues[i]) * cost;// / (Neurons.Count - layer);
                                    }
                                }
                            } //favourite chackpoint place ;-)

                            //copy changesToPrevious to presentChanges
                            if (notLastLayer)
                            {
                                presentChanges = new float[changesToPrevious.Length];
                                for (int i = 0; i < presentChanges.Length; i++)
                                {
                                    presentChanges[i] = changesToPrevious[i];
                                }
                            }
                        }
                    }

                    foreach (Neuron[] nrnArr in Neurons)
                    {
                        foreach (Neuron nrn in nrnArr)
                        {
                            nrn.ApplyChanges();
                        }
                    }
                }
            }

            UpdateCost(batches[0]); //automatic cost update with the first batch
        }

        public void UpdateCost(Dictionary<float[], float[]> TestSamples)
        {
            float cost = 0;
            float costPerSample = 0;
            float[] outputs;
            foreach (KeyValuePair<float[], float[]> sample in TestSamples)
            {
                outputs = Result(sample.Key);
                for (int i = 0; i < outputs.Length; i++)
                {
                    costPerSample += (float)Math.Pow(outputs[i] - sample.Value[i], 2);
                }
                cost += costPerSample / outputs.Length;
                costPerSample = 0;
            }
            Cost = cost / TestSamples.Count;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; }
            Network ntw = obj as Network;
            if (obj == null) { throw new ArgumentException("Object is not a Network instance!"); }
            return ntw.Cost == Cost ? 0 : (ntw.Cost > Cost ? 1 : -1);
        }

        public static float Sigmoid(float X) { return (float)(1 / (1 + Math.Exp(-X))); }
        public static float DerivativeSigmoid(float X) { return Sigmoid(X) * (1 - Sigmoid(X)); }
        public static float AlternativeSigmoid(float X) { return 2 * X * (1 - X) - 1; } //from https://www.youtube.com/watch?v=-WjKICvAOsY
        public static float ReLU(float X) { return X < 0 ? 0 : X; }
        public static float DerivativeReLU(float X) { return X < 0 ? 0 : 1; }
        public static float SampleGaussian(float mean, float stddev) //https://gist.github.com/tansey/1444070
        {
            double x1 = 1 - rnd.NextDouble();
            double x2 = 1 - rnd.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return (float)(y1 * stddev + mean);
        }
        public string PrintableResult(float[] Inputs)
        {
            StringBuilder sb = new StringBuilder("Results for inputs\n");
            float[] result = Result(Inputs);
            for (int i = 0; i < Inputs.Length; i++)
            {
                sb.Append(i == Inputs.Length - 1 ? $"{Inputs[i].ToString()}\n\n" : $"{Inputs[i]}, ");
            }
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append($"neuron {i + 1}: {Math.Round(result[i], 4)}\n");
            }
            return sb.ToString();
        }

        bool EqualContents(float[] First, float[] Second)
        {
            if (First.Length != Second.Length) { return false; }
            for (int i = 0; i < First.Length; i++)
            {
                if (First[i] != Second[i]) { return false; }
            }
            return true;
        }
    }
}
