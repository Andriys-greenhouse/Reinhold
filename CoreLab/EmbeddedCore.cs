using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;

namespace CoreLab
{
    class EmbeddedCore
    {
        [JsonProperty("contexts")]
        List<string> Contexts;
        [JsonProperty("contextIndex")]
        int curentContextIndex;
        [JsonProperty("intents")]
        List<string> Intents;
        [JsonProperty("vocab")]
        Dictionary<string, float[]> Vocabrulary = new Dictionary<string, float[]>();
        static Porter2 stemer = new Porter2();
        [JsonProperty("context")]
        object Context = new object();
        [JsonProperty("contextNetwork")]
        VavKavNetwork ContextNetwork;
        [JsonProperty("intentNetwork")]
        VavKavNetwork IntentNetwork;
        void InitConIntAndVocabrulary(string fileNameForVocabrularyInit)
        {
            Contexts = new List<string>();
            Intents = new List<string>();
            Vocabrulary = new Dictionary<string, float[]>();

            string[] words;
            string word;
            Contexts.Add("default"); //this one is always first
            Contexts.Add("chitchat");
            Contexts.Add("user");
            Contexts.Add("person");
            Contexts.Add("story");
            Contexts.Add("book");
            Contexts.Add("call");
            Contexts.Add("calendar");
            Contexts.Add("calendarAdd");
            Contexts.Add("wordSoccer");
            Contexts.Add("ticTacToe");
            Contexts.Add("RPS");
            Contexts.Add("guessToTen");
            Contexts.Add("rndNum");
            Contexts.Add("quote");
            Contexts.Add("search");
            Contexts.Add("news");
            Contexts.Add("command");
            //Intents.Add("EEGG"); //Andriy + hidden + get -> eegg
            Intents.Add("indefinite"); //I can't understand
            Intents.Add("about");
            Intents.Add("preferences"); //of Reinhold (what do you like -> I don't have an opinion on that) + esteg bib
            Intents.Add("greet");
            Intents.Add("mood"); //how are you
            Intents.Add("weather");
            Intents.Add("residence"); //where do you live
            Intents.Add("sugestSearch");
            Intents.Add("interjection"); //uh, oh, wow, really?, hm and similar
            Intents.Add("approval"); //OK, allright, yes
            Intents.Add("call");
            Intents.Add("calendar");
            Intents.Add("calendarAdd");
            Intents.Add("wordSoccer");
            Intents.Add("ticTacToe");
            Intents.Add("RPS");
            Intents.Add("guessToTen");
            Intents.Add("rndNum");
            Intents.Add("quote");
            Intents.Add("rndStory");
            Intents.Add("battery");
            Intents.Add("time");
            Intents.Add("search");
            Intents.Add("news");
            Intents.Add("TTS");
            Intents.Add("rndQuestion");
            //Intents.Add("function");

            using (StreamReader sr = new StreamReader(fileNameForVocabrularyInit))
            {
                Vocabrulary = JsonConvert.DeserializeObject<Dictionary<string, float[]>>(sr.ReadToEnd());
            }
        }
        void InitNetworks(int[] contextualHiddenLayers, int[] intentialHiddenLayers)
        {
            int[] cntNNLayers = new int[contextualHiddenLayers.Length + 2];
            int[] intNNLayers = new int[intentialHiddenLayers.Length + 2];
            contextualHiddenLayers.CopyTo(cntNNLayers, 1);
            intentialHiddenLayers.CopyTo(intNNLayers, 1);
            cntNNLayers[0] = Contexts.Count + Vocabrulary.First().Value.Length;
            intNNLayers[0] = Contexts.Count + Vocabrulary.First().Value.Length;
            cntNNLayers[cntNNLayers.Length - 1] = Contexts.Count;
            intNNLayers[intNNLayers.Length - 1] = Intents.Count;
            ContextNetwork = new VavKavNetwork(cntNNLayers);
            IntentNetwork = new VavKavNetwork(intNNLayers);
        }

        TrainingDataSet LoadTrainingDataFromText(string text, string fileNameForVocabrularyInit)
        {
            string[] lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sbToInit = new StringBuilder();
            foreach (string line in lines)
            {
                sbToInit.Append(line.Split('ř')[1]);
            }
            InitConIntAndVocabrulary(fileNameForVocabrularyInit);

            string[] parts;
            List<string> words = new List<string>();
            float[] inputs = new float[Contexts.Count + Vocabrulary.First().Value.Length];
            float[] contextsOutputs = new float[Contexts.Count];
            float[] intentsOutputs = new float[Intents.Count];
            Dictionary<float[], float[]> finalContexts = new Dictionary<float[], float[]>();
            Dictionary<float[], float[]> finalIntents = new Dictionary<float[], float[]>();

            foreach (string line in lines)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = 0;
                }
                for (int i = 0; i < contextsOutputs.Length; i++)
                {
                    contextsOutputs[i] = 0;
                }
                for (int i = 0; i < intentsOutputs.Length; i++)
                {
                    intentsOutputs[i] = 0;
                }
                parts = line.Split('ř');
                inputs[Contexts.IndexOf(parts[0])] = 1;
                contextsOutputs[Contexts.IndexOf(parts[2])] = 1;
                //from https://stackoverflow.com/questions/49868766/get-the-first-word-from-the-string
                parts[1] = Regex.Replace(parts[1], @"[^0-9a-zA-Z\ ]+", "");
                words = new List<string>(parts[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                int index;
                index = Intents.IndexOf(parts[3]);
                if (index != -1)
                {
                    intentsOutputs[index] = 1;
                }
                else { throw new ArgumentException("Error, wrong intent."); }

                float[] inputArray = new float[Vocabrulary.First().Value.Length];
                string word;
                for (int i = 0; i < inputArray.Length; i++)
                {
                    inputArray[i] = 0f;
                }
                for (int i = 0; i < words.Count; i++)
                {
                    word = stemer.stem(words[i].ToLower());
                    if (Vocabrulary.ContainsKey(word))
                    {
                        for (int j = 0; j < inputArray.Length; j++)
                        {
                            inputArray[i] += Vocabrulary[word][i];
                            inputArray[i] /= 2;
                        }
                    }
                }
                finalContexts.Add((float[])inputs.Clone(), (float[])contextsOutputs.Clone());
                finalIntents.Add((float[])inputs.Clone(), (float[])intentsOutputs.Clone());
            }

            curentContextIndex = 0;
            return new TrainingDataSet(finalContexts, finalIntents);
        }
        public AnalysisResult Process(string input)
        {
            AnalysisResult final = new AnalysisResult();
            float[] NNinputs = new float[Contexts.Count + Vocabrulary.First().Value.Length];
            float[] intOutputs, cntOutputs;
            for (int i = 0; i < NNinputs.Length; i++)
            {
                NNinputs[i] = 0f;
            }
            string[] words = Regex.Replace(input, @"[^0-9a-zA-Z\ ]+", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Contexts.Count; i++)
            {
                NNinputs[i] = i == curentContextIndex ? 1 : 0;
            }

            float[] inputArray = new float[Vocabrulary.First().Value.Length];
            string word;
            for (int i = 0; i < inputArray.Length; i++)
            {
                inputArray[i] = 0f;
            }
            for (int i = 0; i < words.Length; i++)
            {
                word = stemer.stem(words[i].ToLower());
                if (Vocabrulary.ContainsKey(word))
                {
                    for (int j = 0; j < inputArray.Length; j++)
                    {
                        inputArray[i] += Vocabrulary[word][i];
                        inputArray[i] /= 2;
                    }
                }
            }
            inputArray.CopyTo(NNinputs, Contexts.Count + 1);

            cntOutputs = ContextNetwork.Result(NNinputs);
            intOutputs = IntentNetwork.Result(NNinputs);

            float highest = 0;
            int highestIndex = 0;
            for (int i = 0; i < Contexts.Count; i++)
            {
                if (highest < cntOutputs[i])
                {
                    highest = cntOutputs[i];
                    highestIndex = i;
                }
            }
            final.Context = Contexts[highestIndex];
            final.PastContext = Contexts[curentContextIndex];
            curentContextIndex = highestIndex;

            highest = 0;
            highestIndex = 0;
            for (int i = 0; i < Intents.Count; i++)
            {
                if (highest < intOutputs[i])
                {
                    highest = intOutputs[i];
                    highestIndex = i;
                }
            }
            final.Intent = Intents[highestIndex];

            return final;
        }
        public void Train(string serializedVocabrularyFileName, string trainingDataText, int[] contextualHiddenLayers, int[] intentialHiddenLayers, int iterations) //first of two methods to init network
        {
            TrainingDataSet trainingData = LoadTrainingDataFromText(trainingDataText, serializedVocabrularyFileName);
            InitNetworks(contextualHiddenLayers, intentialHiddenLayers);
            Console.WriteLine("context");
            ContextNetwork.Train(trainingData.ContextNNData, iterations);
            Console.WriteLine("intent");
            IntentNetwork.Train(trainingData.IntentNNData, iterations);
        }
    }
}
