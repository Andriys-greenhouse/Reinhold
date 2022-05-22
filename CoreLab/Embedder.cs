using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CoreLab

{
    public class Embedder
    {
        string[] SplittedText;
        static string savingName = "embedding.json";
        static int numberOfDimensions = 75; // > 0
        static int contextSize = 4; //must be an even number && > 2
        static int iterations = 10_000;
        EmbeddingNetwork NeuNet;
        static Porter2 stemmer = new Porter2();
        List<string> Vocabulary;
        List<(float[], float[])> TrainingData;
        public Embedder(string text)
        {
            Vocabulary = new List<string>();
            TrainingData = new List<(float[], float[])>();
            PreprocessText(text);
            NeuNet = new EmbeddingNetwork(new int[] { Vocabulary.Count * contextSize, numberOfDimensions, Vocabulary.Count });
            Train();
        }

        void PreprocessText(string text)
        {
            SplittedText = Regex.Replace(text, @"[^0-9a-zA-Z\ ]+", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string word;
            for (int i = 0; i < SplittedText.Length; i++)
            {
                SplittedText[i] = stemmer.stem(SplittedText[i].ToLower());
                word = SplittedText[i];
                if (!Vocabulary.Contains(word)) { Vocabulary.Add(word); }
            }


            float[] inputZeros = new float[Vocabulary.Count * contextSize];
            float[] outputZeros = new float[Vocabulary.Count];
            for (int i = 0; i < inputZeros.Length; i++)
            {
                inputZeros[i] = 0f;
            }
            for (int i = 0; i < outputZeros.Length; i++)
            {
                outputZeros[i] = 0f;
            }
            float[] inCopy = new float[inputZeros.Length];
            float[] outCopy = new float[outputZeros.Length];

            for (int i = 0; i < Vocabulary.Count - contextSize; i++)
            {
                inputZeros.CopyTo(inCopy, 0);
                outputZeros.CopyTo(outCopy, 0);
                for (int j = 0; j < contextSize; j++)
                {
                    if (j < contextSize / 2) { word = SplittedText[i + j]; }
                    else { word = SplittedText[i + 1 + j]; }
                    inCopy[j * Vocabulary.Count + Vocabulary.IndexOf(word)] = 1f;
                }
                outCopy[Vocabulary.IndexOf(SplittedText[i + contextSize / 2])] = 1f;
                TrainingData.Add((inCopy, outCopy));
            }
        }

        public void Train()
        {
            Console.WriteLine("Starting word embedding analysis of text");
            NeuNet.Train(TrainingData, iterations, true);

            Dictionary<string, float[]> vectors = new Dictionary<string, float[]>();
            float[][] embedings = NeuNet.GetWeights(1);

            for (int i = 0; i < Vocabulary.Count; i++)
            {
                vectors.Add(Vocabulary[i], embedings[i]);
            }

            using (StreamWriter sw = new StreamWriter(savingName))
            {
                sw.Write(JsonConvert.SerializeObject(vectors));
            }

            Console.WriteLine("Vectors successfully saved");
        }
    }



    class EmbeddingNetwork
    {
        static float LearningRate = 0.005f; //0.001f best sofar
        static int randomNeeded = 150;

        float[] InputNeurons { get; set; }
        float[][] Activations { get; set; }
        [JsonProperty("biases")]
        float[][] Biases { get; set; }
        [JsonProperty("weights")]
        float[][][] Weights { get; set; }
        float[][] BiasChanges { get; set; }
        float[][][] WeightChanges { get; set; }
        float[][] Deltas { get; set; }
        float[][] Values { get; set; }
        [JsonProperty("layers")]
        int[] Layers { get; set; }

        static Random rnd = new Random();
        public float Cost { get; private set; } = 1f;

        float lowestCost { get; set; } = 1f;

        public EmbeddingNetwork(int[] Layers)
        {
            //check inputs
            if (Layers.Length < 3) { throw new ArgumentException("Invalid assignment. (wrong number of layres)"); }
            foreach (int num in Layers)
            {
                if (num < 1) { throw new ArgumentException("Invalid assignment. (wrong number of neurons)"); }
            }

            //initialize arrays
            InputNeurons = new float[Layers[0]];
            Activations = new float[Layers.Length - 1][];
            Values = new float[Layers.Length - 1][];
            Biases = new float[Layers.Length - 1][];
            Deltas = new float[Layers.Length - 1][];
            Weights = new float[Layers.Length - 1][][];
            WeightChanges = new float[Layers.Length - 1][][];
            BiasChanges = new float[Layers.Length - 1][];

            for (int i = 0; i < Layers.Length - 1; i++)
            {
                Activations[i] = new float[Layers[i + 1]];
                Values[i] = new float[Layers[i + 1]];
                Biases[i] = new float[Layers[i + 1]];
                Deltas[i] = new float[Layers[i + 1]];
                BiasChanges[i] = new float[Layers[i + 1]];
                Weights[i] = new float[Layers[i + 1]][];
                WeightChanges[i] = new float[Layers[i + 1]][];
                for (int j = 0; j < Layers[i + 1]; j++)
                {
                    Weights[i][j] = new float[Layers[i]];
                    WeightChanges[i][j] = new float[Layers[i]];
                    Biases[i][j] = 0f;
                    BiasChanges[i][j] = 0f;
                }
            }

            this.Layers = Layers;
            InitWeights();
        }

        void InitWeights()
        {
            for (int i = 0; i < Weights[Weights.Length - 1].Length; i++)//last layer
            {
                for (int j = 0; j < Weights[Weights.Length - 1][i].Length; j++)
                {
                    Weights[Weights.Length - 1][i][j] = SampleGaussian(0, (float)Math.Sqrt(2 / (float)(Layers[Layers.Length - 2] + Layers[Layers.Length - 1])));
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

        void InitBiases()
        {
            for (int i = 0; i < Biases.Length; i++)
            {
                for (int j = 0; j < Biases[i].Length; j++)
                {
                    Biases[i][j] = 0f;
                }
            }
        }

        public float[] Result(float[] Inputs)
        {
            if (InputNeurons.Length != Inputs.Length) { throw new ArgumentException($"Invalid number of inputs. ({InputNeurons.Length} neurons in input layer)"); }

            float[] final = new float[Activations[Activations.Length - 1].Length];

            for (int i = 0; i < Inputs.Length; i++)
            {
                InputNeurons[i] = Inputs[i];
            }

            float value;
            for (int i = 0; i < Activations.Length - 1; i++) //all but the last
            {
                for (int j = 0; j < Activations[i].Length; j++)
                {
                    value = 0;
                    for (int k = 0; k < Weights[i][j].Length; k++)//adding 
                    {
                        value += Weights[i][j][k] * (i == 0 ? InputNeurons[k] : Activations[i - 1][k]);
                    }
                    value += Biases[i][j];
                    Activations[i][j] = ReLU(value);
                    Values[i][j] = value;
                }
            }

            for (int i = 0; i < Activations[Activations.Length - 1].Length; i++)
            {
                value = 0;
                for (int j = 0; j < Activations[Activations.Length - 2].Length; j++)
                {
                    value += Activations[Activations.Length - 2][j] * Weights[Weights.Length - 1][i][j];
                }
                value += Biases[Biases.Length - 1][i];
                Activations[Activations.Length - 1][i] = Sigmoid(value);
                Values[Values.Length - 1][i] = value;
            }

            for (int i = 0; i < final.Length; i++)
            {
                final[i] = Activations[Activations.Length - 1][i];
            }
            return final;
        }

        public void Train(List<(float[], float[])> Inputs, int Iterations, bool DisplayProgress = true) //Inputs is list of input-output pairs consisting of float arrays
        {
            //check inputs
            foreach ((float[], float[]) pair in Inputs)
            {
                if (pair.Item1.Length != InputNeurons.Length || pair.Item2.Length != Activations[Activations.Length - 1].Length) { throw new ArgumentException("Invalid input or output elements of an input-output pair!"); }
                foreach (float result in pair.Item2)
                {
                    if (result < 0 || result > 1) { throw new ArgumentException("Some result is greater than 1 or smaller than 0!"); }
                }
            }
            if (Iterations < 1) { throw new ArgumentException("At least one iteration is required!"); }


            //training itself
            int lastDisplay = 0;
            int lastCheck = 0;
            string lastDisplayedString = "";
            string currentString;
            float error, referenceError;
            float lastDif = 0;
            int suspectCount = 0;

            float costOfCurentRun;

            for (int Iter = 0; Iter < Iterations; Iter++)
            {
                lastCheck++;
                lastDisplay++;

                //display progress
                currentString = $"progress: {Iter * 100 / Iterations}%    Cost: {Cost}";
                if (DisplayProgress && lastDisplay % 2 == 0 && currentString != lastDisplayedString) //display after x iterations (500 is best sofar)
                {
                    lastDisplay = 0;
                    //Console.Clear();
                    Console.WriteLine(currentString);
                    lastDisplayedString = currentString;
                }

                //check for stagnation
                if (Cost > 0.999999 && suspectCount != -1)
                {
                    suspectCount++;
                    if (suspectCount > 500)
                    {
                        Console.WriteLine($"Stagnation detected    cost: {Cost}");
                        throw new Exception("VavKavNetwork stagnates.");
                    }
                }
                else { suspectCount = -1; }


                //set changes to zeros
                for (int i = 0; i < Activations.Length; i++)
                {
                    for (int j = 0; j < Activations[i].Length; j++)
                    {
                        BiasChanges[i][j] = 0;
                        for (int k = 0; k < WeightChanges[i][j].Length; k++)
                        {
                            WeightChanges[i][j][k] = 0;
                        }
                    }
                }

                (float[], float[]) pair;
                for (int IRnd = 0; IRnd < randomNeeded; IRnd++)
                {
                    pair = Inputs[rnd.Next(Inputs.Count)];

                    //set Deltas to zeros
                    for (int i = 0; i < Activations.Length; i++)
                    {
                        for (int j = 0; j < Activations[i].Length; j++)
                        {
                            Deltas[i][j] = 0;
                        }
                    }

                    Result(pair.Item1); //update values

                    costOfCurentRun = 0;
                    for (int i = 0; i < Activations[Activations.Length - 1].Length; i++) //for neurons in last layer
                    {
                        costOfCurentRun += (Activations[Activations.Length - 1][i] - pair.Item2[i]) * (Activations[Activations.Length - 1][i] - pair.Item2[i]);
                        Deltas[Deltas.Length - 1][i] = 2 * (Activations[Activations.Length - 1][i] - pair.Item2[i]) * DerivativeSigmoid(Values[Values.Length - 1][i]);
                    }
                    Cost = (costOfCurentRun + Cost) / 2; //average last and curent cost

                    for (int i = Activations.Length - 2; i >= 0; i--) //starting from pre-last layer and going backwards
                    {
                        float newDelta;
                        for (int j = 0; j < Activations[i].Length; j++) //neuron in layer
                        {
                            newDelta = 0;

                            for (int k = 0; k < Activations[i + 1].Length; k++)//neurons from last layer
                            {
                                newDelta += Deltas[i + 1][k] * Weights[i + 1][k][j];
                            }
                            Deltas[i][j] = newDelta * DerivativeReLU(Values[i][j]);
                        }
                    }

                    for (int i = 0; i < Biases.Length; i++)//layer
                    {
                        for (int j = 0; j < Biases[i].Length; j++)//neuron
                        {
                            BiasChanges[i][j] -= LearningRate * Deltas[i][j];
                            for (int k = 0; k < Weights[i][j].Length; k++)//weight
                            {
                                WeightChanges[i][j][k] -= LearningRate * Deltas[i][j] * ((i == 0 ? InputNeurons[k] : Activations[i - 1][k]));
                            }
                        }
                    }
                }

                //apply changes
                for (int i = 0; i < Biases.Length; i++)
                {
                    for (int j = 0; j < Biases[i].Length; j++)
                    {
                        Biases[i][j] += BiasChanges[i][j]; //averaging it
                        for (int k = 0; k < Weights[i][j].Length; k++)
                        {
                            Weights[i][j][k] += WeightChanges[i][j][k];
                        }
                    }
                }

                //check for treshhold
                if (Cost < 0.0001 && Iter > 30) //0.000001  Iter > 300
                { Iter = Iterations; }
            }
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
            VavKavNetwork ntw = obj as VavKavNetwork;
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

        public float[] GetBias(int layerIndex) { return Biases[layerIndex]; }
        public float[][] GetWeights(int layerIndex) { return Weights[layerIndex]; }
    }
}
