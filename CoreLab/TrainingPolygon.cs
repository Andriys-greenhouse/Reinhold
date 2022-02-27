using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace CoreLab
{
    class Core
    {
        List<string> Contexts;
        int curentContextIndex;
        List<string> Intents;
        List<string> BOW = new List<string>();
        static Porter2 stemer = new Porter2();
        object Context = new object();
        VavKavNetwork Network;
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
            //Contexts.Add("function");
            Intents.Add("indefinite"); //I can't understand
            Intents.Add("aboutBot");
            Intents.Add("preferences"); //of Reinhold (what do you like) + esteg bib
            Intents.Add("greet");
            Intents.Add("mood"); //how are you
            Intents.Add("weather");
            Intents.Add("residence"); //where do you live
            Intents.Add("sugestSearch");
            Intents.Add("interjection"); //uh, oh, wow, really?, hm and similar
            //Intents.Add("function");
            textForBOWInit = Regex.Replace(textForBOWInit, @"[^0-9a-zA-Z\ ]+", " ");
            words = textForBOWInit.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in words)
            {
                word = stemer.stem(item);
                if (BOW.IndexOf(word) == -1)
                {
                    BOW.Add(word);
                }
            }
            BOW.Add("ř"); //for unknown words
        }
        void InitNetwork(int[] hiddenLayers)
        {
            int[] NNLayers = new int[hiddenLayers.Length + 2];
            hiddenLayers.CopyTo(NNLayers, 1);
            NNLayers[0] = Contexts.Count + BOW.Count;
            NNLayers[NNLayers.Length - 1] = Contexts.Count + Intents.Count;
            Network = new VavKavNetwork(NNLayers);
        }

        Dictionary<float[], float[]> LoadTrainingDataFromText(string text)
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
            float[] outputs = new float[Contexts.Count + Intents.Count];
            Dictionary<float[], float[]> final = new Dictionary<float[], float[]>();

            foreach (string line in lines)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = 0;
                }
                for (int i = 0; i < outputs.Length; i++)
                {
                    outputs[i] = 0;
                }
                parts = line.Split('ř');
                inputs[Contexts.IndexOf(parts[0])] = 1;
                outputs[Contexts.IndexOf(parts[2])] = 1;
                //from https://stackoverflow.com/questions/49868766/get-the-first-word-from-the-string
                parts[1] = Regex.Replace(parts[1], @"[^0-9a-zA-Z\ ]+", "");
                words = new List<string>(parts[1].Split(' '));
                int index;
                index = Intents.IndexOf(parts[3]);
                if (index != -1)
                {
                    outputs[Contexts.Count + index] = 1;
                }
                else { throw new ArgumentException("Error, wrong intent."); }
                for (int i = 0; i < words.Count; i++)
                {
                    index = BOW.IndexOf(stemer.stem(words[i]));
                    inputs[Contexts.Count + (index == -1 ? BOW.Count - 1 : index)] = 1f; //last index for unknown words
                }
                final.Add((float[])inputs.Clone(), (float[])outputs.Clone());
            }

            curentContextIndex = 0;
            return final;
        }
        public void Process(string input)
        {
            float[] NNinputs = new float[Contexts.Count + BOW.Count];
            float[] outputs;
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
                NNinputs[Contexts.Count + (index == -1 ? BOW.Count - 1 : index)] = 1f;
            }

            outputs = Network.Result(NNinputs);

            float highest = 0;
            int highestIndex = 0;
            for (int i = 0; i < Contexts.Count; i++)
            {
                if (highest < outputs[i])
                {
                    highest = outputs[i];
                    highestIndex = i;
                }
            }
            if (highestIndex != curentContextIndex)
            {
                curentContextIndex = highestIndex;
                //analyze text for new context object based on index
            }

            highest = 0;
            highestIndex = 0;
            for (int i = 0; i < Intents.Count; i++)
            {
                if (highest < outputs[Contexts.Count + i])
                {
                    highest = outputs[i];
                    highestIndex = i;
                }
            }

            //analyze text for data or Context object change
            //take action on highestIndex which corresponds to index in Intents
        }
        public void Train(string trainingDataText, int[] hiddenLayers, int iterations) //first of two methods to init network
        {
            Dictionary<float[], float[]> trainingData = LoadTrainingDataFromText(trainingDataText);
            InitNetwork(hiddenLayers);
            Network.Train(trainingData, iterations);
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