using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLab
{
    class TrainingPolygon
    {
        Dictionary<double[], double[]> inputs { get; set; }
        int[] topology { get; set; }
        public Network Winner { get; private set; }

        public TrainingPolygon(Dictionary<double[], double[]> Inputs, int[] NetworkLayersTopology)
        {
            inputs = Inputs;
            topology = NetworkLayersTopology;
        }

        public void Train(int Iterations = 1000000, bool DisplayProgress = true)
        {
            Network[][] competitors = new Network[5][]; //number of finalists
            int numOfCompetInGroup = 5;

            //fill competitors
            for (int i = 0; i < competitors.Length; i++)
            {
                competitors[i] = new Network[numOfCompetInGroup];
                for (int j = 0; j < numOfCompetInGroup; j++)
                {
                    competitors[i][j] = new Network(topology);
                }
            }

            
        }
    }
}
