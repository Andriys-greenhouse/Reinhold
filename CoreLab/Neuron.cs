using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLab
{
    public class Neuron : Perceptron
    {
        static Random rnd = new Random();

        public double Bias { get; set; }
        public double BiasChange { get; set; }
        public List<double> InputSynapseValues { get; set; }
        public List<double> InputSynapseValuesChange { get; set; }


        public void UpdateValue(Perceptron[] PreviousLayer)
        {
            if (PreviousLayer.Length != InputSynapseValues.Count) { throw new ArgumentException("Different neuron and synapse count in previous layer!"); }
            double final = 0;
            for (int i = 0; i < PreviousLayer.Length; i++)
            {
                final += PreviousLayer[i].Value * InputSynapseValues[i];
            }
            Value = Network.Sigmoid(final + Bias);
            if (Value == 0) { Value = 0.00000001; }
            else if (Value == 1) { Value = 0.9999999; }
        }

        public Neuron(int NumberOfPreviousLayerNeurons)
        {
            InputSynapseValues = new List<double>();
            Bias = rnd.NextDouble() + rnd.Next(-100, 100);
            for (int i = 0; i < NumberOfPreviousLayerNeurons; i++)
            {
                InputSynapseValues.Add(rnd.NextDouble() + rnd.Next(-100, 100));
            }

            BiasChange = 0;
            InputSynapseValuesChange = new List<double>();
            for (int i = 0; i < InputSynapseValues.Count; i++)
            {
                InputSynapseValuesChange.Add(0);
            }
        }

        public void ApplyChanges()
        {
            Bias += BiasChange;
            BiasChange = 0;
            for (int i = 0; i < InputSynapseValues.Count; i++)
            {
                InputSynapseValues[i] += InputSynapseValuesChange[i];
                InputSynapseValuesChange[i] = 0;
            }
        }
    }

    public class Perceptron
    {
        public double Value { get; set; }
    }
}
