using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace CoreLab
{
    public class Core
    {
        [JsonProperty("contexts")]
        List<string> Contexts;
        [JsonProperty("contextIndex")]
        int curentContextIndex;
        [JsonProperty("intents")]
        List<string> Intents;
        [JsonProperty("bow")]
        List<string> BOW = new List<string>();
        static Porter2 stemer = new Porter2();
        [JsonProperty("context")]
        object Context = new object();
        [JsonProperty("contextNetwork")]
        VavKavNetwork ContextNetwork;
        [JsonProperty("intentNetwork")]
        VavKavNetwork IntentNetwork;
        void InitConIntAndBOW(string textForBOWInit)
        {
            Contexts = new List<string>();
            Intents = new List<string>();
            BOW = new List<string>();

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
            textForBOWInit = Regex.Replace(textForBOWInit, @"[^0-9a-zA-Z\ ]+", " ");
            words = textForBOWInit.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in words)
            {
                if(item != "XXX") //symbol for anything
                {
                    word = stemer.stem(item);
                    if (BOW.IndexOf(word) == -1)
                    {
                        BOW.Add(word);
                    }
                }
            }
            BOW.Add("ř"); //for unknown words
        }
        void InitNetworks(int[] contextualHiddenLayers, int[] intentialHiddenLayers)
        {
            int[] cntNNLayers = new int[contextualHiddenLayers.Length + 2];
            int[] intNNLayers = new int[intentialHiddenLayers.Length + 2];
            contextualHiddenLayers.CopyTo(cntNNLayers, 1);
            intentialHiddenLayers.CopyTo(intNNLayers, 1);
            cntNNLayers[0] = Contexts.Count + BOW.Count;
            intNNLayers[0] = Contexts.Count + BOW.Count;
            cntNNLayers[cntNNLayers.Length - 1] = Contexts.Count;
            intNNLayers[intNNLayers.Length - 1] = Intents.Count;
            ContextNetwork = new VavKavNetwork(cntNNLayers);
            IntentNetwork = new VavKavNetwork(intNNLayers);
        }

        TrainingDataSet LoadTrainingDataFromText(string text)
        {
            string[] lines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sbToInit = new StringBuilder();
            foreach (string line in lines)
            {
                sbToInit.Append(line.Split('ř')[1]);
            }
            InitConIntAndBOW(sbToInit.ToString());

            string[] parts;
            List<string> words = new List<string>();
            float[] inputs = new float[Contexts.Count + BOW.Count];
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
                words = new List<string>(parts[1].Split(' '));
                int index;
                index = Intents.IndexOf(parts[3]);
                if (index != -1)
                {
                    intentsOutputs[index] = 1;
                }
                else { throw new ArgumentException("Error, wrong intent."); }
                for (int i = 0; i < words.Count; i++)
                {
                    index = BOW.IndexOf(stemer.stem(words[i]));
                    inputs[Contexts.Count + (index == -1 ? BOW.Count - 1 : index)] += 1f; //last index for unknown words
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
            float[] NNinputs = new float[Contexts.Count + BOW.Count];
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
            int index;
            for (int i = 0; i < words.Length; i++)
            {
                index = BOW.IndexOf(stemer.stem(words[i]));
                NNinputs[Contexts.Count + (index == -1 ? BOW.Count - 1 : index)] += 1f;
            }

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
        public void Train(string trainingDataText, int[] contextualHiddenLayers, int[] intentialHiddenLayers, int iterations) //first of two methods to init network
        {
            TrainingDataSet trainingData = LoadTrainingDataFromText(trainingDataText);
            InitNetworks(contextualHiddenLayers, intentialHiddenLayers);
            Console.WriteLine("context");
            ContextNetwork.Train(trainingData.ContextNNData, iterations);
            Console.WriteLine("intent");
            IntentNetwork.Train(trainingData.IntentNNData, iterations);
        }
    }

    public struct AnalysisResult
    {
        public string PastContext;
        public string Context;
        public string Intent;
        public AnalysisResult(string aPastContext, string aContext, string aIntent)
        {
            PastContext = aPastContext;
            Context = aContext;
            Intent = aIntent;
        }
    }
    public struct TrainingDataSet
    {
        public Dictionary<float[], float[]> ContextNNData;
        public Dictionary<float[], float[]> IntentNNData;
        public TrainingDataSet(Dictionary<float[], float[]> aContextNNData, Dictionary<float[], float[]> aIntentNNData)
        {
            ContextNNData = aContextNNData;
            IntentNNData = aIntentNNData;
        }
    }
}

/*
 Contexts:
    doesn't matter (default)
    chitchat - info about reinhold, pre programmed answers
    user -> info
    person -> story, info
    story -> text, person, date
    book -> title
    function - ...
 Intents:
    info about reinhold
    music, film, book favourite -> dont have favourites, lets talk about you + easter egging Bible
    weather, place -> sugest internet search
    web search(keyword) -> return results
    function(needed arguments) <- one context for every step
 ContextřMessageřContextřIntent
 */