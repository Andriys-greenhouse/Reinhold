using System;
using System.Collections.Generic;
using System.Text;

namespace CoreLab
{
    public class Network : IComparable
    {
        static double neuronBiasStep = 0.1; //coeficient > 0
        static double neuronSynapseStep = 0.1; //coeficient > 0
        static double wantedChangeStep = 0.1; //coeficient > 0

        public Perceptron[] Perceptrons { get; set; }
        public List<Neuron[]> Neurons { get; set; }
        static Random rnd = new Random();
        public double Cost { get; private set; }

        public Network(int[] Layers)
        {
            Neurons = new List<Neuron[]>();

            if (Layers.Length < 2) { throw new ArgumentException("Invalid assignment. (wrong number of layres)"); }
            foreach (int num in Layers)
            {
                if (num < 1) { throw new ArgumentException("Invalid assignment. (wrong number of neurons)"); }
            }

            Perceptrons = new Perceptron[Layers[0]];
            for (int i = 0; i < Layers[0]; i++)
            {
                Perceptrons[i] = new Perceptron();
            }

            for (int g = 1; g < Layers.Length; g++)
            {
                Neurons.Add(new Neuron[Layers[g]]);
                for (int i = 0; i < Layers[g]; i++)
                {
                    Neurons[Neurons.Count - 1][i] = new Neuron((Neurons.Count == 1) ? Perceptrons.Length : Neurons[Neurons.Count - 2].Length);
                }
            }
        }

        public double[] Result(double[] Inputs)
        {
            if (Perceptrons.Length != Inputs.Length) { throw new ArgumentException($"Invalid number of inputs. ({Perceptrons.Length} perceptrons in input layer)"); }

            double[] final = new double[Neurons[Neurons.Count - 1].Length];

            for (int i = 0; i < Inputs.Length; i++)
            {
                Perceptrons[i].Value = Inputs[i];
            }

            for (int i = 0; i < Neurons[0].Length; i++)
            {
                Neurons[0][i].UpdateValue(Perceptrons);
            }
            for (int i = 1; i < Neurons.Count; i++)
            {
                foreach (Neuron nrn in Neurons[i])
                {
                    nrn.UpdateValue(Neurons[i - 1]);
                }
            }

            for (int i = 0; i < final.Length; i++)
            {
                final[i] = Neurons[Neurons.Count - 1][i].Value;
            }
            return final;
        }

        public void TrainWithCost(Dictionary<double[], double[]> Inputs, int Iterations, bool DisplayProgress = true) //Inputs is list of input-output pairs consisting of double arrays
        {
            //check inputs
            foreach (KeyValuePair<double[], double[]> pair in Inputs)
            {
                if (pair.Key.Length != Perceptrons.Length || pair.Value.Length != Neurons[Neurons.Count - 1].Length)
                {
                    throw new ArgumentException("Invalid input or output elements of an input-output pair!");
                }
                foreach (double result in pair.Value)
                {
                    if (result < 0 || result > 1) { throw new ArgumentException("Some result is greater than 1 or smaller than 0!"); }
                }
            }
            if (Iterations < 1) { throw new ArgumentException("At least one iteration is required!"); }

            //devide into batches
            List<Dictionary<double[], double[]>> batches = new List<Dictionary<double[], double[]>>();
            int num = 0;
            batches.Add(new Dictionary<double[], double[]>());
            foreach (KeyValuePair<double[], double[]> pair in Inputs)
            {
                if (num == 100) //limit for batch population
                {
                    num = 0;
                    batches.Add(new Dictionary<double[], double[]>());
                }
                batches[batches.Count - 1].Add(pair.Key, pair.Value);
                num++;
            }

            //training itself
            int lastDisplay = 0;
            int lastCheck = 0;
            string lastDisplayedString = "";
            string currentString;
            double error, referenceError;
            double lastDif = 0;
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
                if(lastCheck % 1000 == 0 && lastCheck > 1)
                {
                    lastCheck = 0;

                    int countHere = 0;
                    double[] lastOutput = null, curent, lastExpected = null;
                    foreach (KeyValuePair<double[], double[]> pair in batches[0])
                    {
                        curent = Result(pair.Key);
                        if (lastExpected != null && !EqualContents(pair.Value, lastExpected) && EqualContents(lastOutput, curent))
                        {
                            countHere++;
                            
                        }
                        lastOutput = curent;
                        lastExpected = pair.Value;
                    }
                    if(countHere > 0) { suspectCount++; }
                    if (suspectCount >= 1 + Iterations / 9000)
                    {
                            //Training stagnates!
                        Iter = Iterations; //finish this iteration and end training
                    }
                }


                foreach (Dictionary<double[], double[]> batch in batches)
                {
                    foreach (KeyValuePair<double[], double[]> pair in batch)
                    {

                        double[] changesToPrevious = new double[] { };
                        double[] presentChanges = new double[] { };
                        double changeWanted;
                        double[] output = Result(pair.Key);
                        for (int layer = Neurons.Count - 1; layer >= 0; layer--)
                        {

                            bool notLastLayer = layer > 0;

                            if (notLastLayer)
                            {
                                changesToPrevious = new double[Neurons[layer - 1].Length];
                                for (int i = 0; i < changesToPrevious.Length; i++)
                                {
                                    changesToPrevious[i] = 0;
                                }
                                if (layer == Neurons.Count - 1) { presentChanges = new double[Neurons[layer - 1].Length]; }
                            }
                            else { changesToPrevious = new double[] { }; } //on the last layer you don't have to note any changes


                            double cost = 0;
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
                                presentChanges = new double[changesToPrevious.Length];
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

        public void UpdateCost(Dictionary<double[], double[]> TestSamples)
        {
            double cost = 0;
            double costPerSample = 0;
            double[] outputs;
            foreach (KeyValuePair<double[], double[]> sample in TestSamples)
            {
                outputs = Result(sample.Key);
                for (int i = 0; i < outputs.Length; i++)
                {
                    costPerSample += Math.Pow(outputs[i] - sample.Value[i], 2);
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

        public static double Sigmoid(double X) { return 1 / (1 + Math.Exp(-X)); }
        //from https://www.youtube.com/watch?v=-WjKICvAOsY
        public static double DerivativeSigmoid(double X) { return X * (1 - X); }
        public static double AlternativeSigmoid(double X) { return 2 * X * (1 - X) - 1; }
        public string PrintableResult(double[] Inputs)
        {
            StringBuilder sb = new StringBuilder("Results for inputs\n");
            double[] result = Result(Inputs);
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
        void RandomiseBiases()
        {
            for (int i = 0; i < Neurons.Count; i++)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    Neurons[i][j].Bias *= rnd.NextDouble();
                }
            }
        }
        void RandomiseWeights()
        {
            for (int i = 0; i < Neurons.Count; i++)
            {
                for (int j = 0; j < Neurons[i].Length; j++)
                {
                    for (int k = 0; k < Neurons[i][j].InputSynapseValues.Count; k++)
                    {
                        Neurons[i][j].InputSynapseValues[k] *= rnd.NextDouble();
                    }
                }
            }
        }

        bool EqualContents(double[] First, double[] Second)
        {
            if (First.Length != Second.Length) { return false; }
            for (int i = 0; i < First.Length; i++)
            {
                if(First[i] != Second[i]) { return false; }
            }
            return true;
        }
    }
}
