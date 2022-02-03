using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLab
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] layers = new int[] { 4, 5, 3, 2 };
            double[] outputs;
            Dictionary<double[], double[]> inputs = new Dictionary<double[], double[]>();

            /*
            inputs.Add(new double[] { 0, 0, 1 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 0, 0, 0 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 0, 1, 1 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 0, 1, 0 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 1, 1, 1 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 1, 1, 0 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 1, 0, 1 }, new double[] { 0, 1 });
            //inputs.Add(new double[] { 1, 0, 0 }, new double[] { 0, 1 });
            */
            /*
            inputs.Add(new double[] { 0, 1 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 25, 0 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 1, 152 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 35, 0 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 584, 254 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 78, 25 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 100, 152 }, new double[] { 0, 1 });
            */
            
            inputs.Add(new double[] { 0, 0, 0, 0 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 0, 0, 0, 1 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 0, 0, 1, 0 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 0, 0, 1, 1 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 0, 1, 0, 0 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 0, 1, 0, 1 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 0, 1, 1, 0 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 0, 1, 1, 1 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 1, 0, 0, 0 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 1, 0, 0, 1 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 1, 0, 1, 0 }, new double[] { 1, 0 });
            //inputs.Add(new double[] { 1, 0, 1, 1 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 1, 1, 0, 0 }, new double[] { 1, 0 });
            inputs.Add(new double[] { 1, 1, 0, 1 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 1, 1, 1, 0 }, new double[] { 0, 1 });
            inputs.Add(new double[] { 1, 1, 1, 1 }, new double[] { 1, 0 });
            
            /*
            inputs.Add(new double[] { 0, 0, 1,
                                      0, 1, 0,
                                      1, 0, 0 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 1, 1, 1,
                                      0, 0, 0,
                                      0, 0, 0 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 0, 0, 0,
                                      1, 1, 1,
                                      0, 0, 0 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 0, 0, 0,
                                      0, 0, 0,
                                      1, 1, 1 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 0, 0, 1,
                                      0, 0, 1,
                                      0, 0, 1 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 0, 1, 0,
                                      0, 1, 0,
                                      0, 1, 0 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 1, 0, 0,
                                      1, 0, 0,
                                      1, 0, 0 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 1, 0, 0,
                                      0, 1, 0,
                                      0, 0, 1 }, new double[] { 1, 0 });

            inputs.Add(new double[] { 0, 1, 0,
                                      0, 1, 0,
                                      0, 0, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 1, 0,
                                      0, 1, 0,
                                      1, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 0, 0,
                                      0, 0, 0,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 1, 0,
                                      0, 0, 0,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 1,
                                      1, 0, 0,
                                      0, 0, 0 }, new double[] { 0, 1 });
            
            inputs.Add(new double[] { 1, 1, 0,
                                      0, 0, 0,
                                      0, 0, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 1, 0,
                                      0, 0, 0,
                                      1, 1, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 1,
                                      0, 0, 0,
                                      0, 0, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 0,
                                      0, 0, 0,
                                      1, 0, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 0,
                                      1, 0, 1,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 1,
                                      0, 1, 0,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 0,
                                      0, 0, 0,
                                      0, 1, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 1, 1,
                                      0, 0, 1,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 1, 0,
                                      0, 0, 0,
                                      1, 1, 0 }, new double[] { 0, 1 });
            //
            inputs.Add(new double[] { 0, 1, 1,
                                      0, 0, 1,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 0, 0,
                                      1, 0, 0,
                                      1, 0, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 0,
                                      0, 0, 1,
                                      1, 0, 0 }, new double[] { 0, 1 });
            
            inputs.Add(new double[] { 1, 0, 1,
                                      0, 0, 0,
                                      0, 1, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 1, 0, 0,
                                      0, 0, 0,
                                      1, 0, 1 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 1, 0,
                                      0, 1, 1,
                                      0, 0, 0 }, new double[] { 0, 1 });

            inputs.Add(new double[] { 0, 0, 0,
                                      1, 1, 0,
                                      0, 1, 0 }, new double[] { 0, 1 });
            */

            Network subject = new Network(layers);

            subject.TrainWithCost(inputs, 300000); //500000 feasable, 1000000 shure

            /*
            Console.WriteLine(subject.PrintableResult(new double[] { 1, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 0, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 1, 0, 0 }));
            */
            /*
            Console.WriteLine(subject.PrintableResult(new double[] { 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 0, 1 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 50, 25 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 25, 50 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 125, 115 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 115, 125 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 120, 125 }));
            */
            
            Console.WriteLine(subject.PrintableResult(new double[] { 0, 1, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 1, 0, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 1, 1, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new double[] { 1, 0, 1, 1 }));
            
            /*
            Console.WriteLine(subject.PrintableResult(
                new double[] { 1, 0, 0,
                               0, 1, 0,
                               0, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(
                new double[] { 1, 0, 1,
                               0, 0, 0,
                               0, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(
                new double[] { 1, 1, 1,
                               0, 0, 0,
                               0, 0, 0 }));
            Console.WriteLine(subject.PrintableResult(
                new double[] { 1, 0, 0,
                               1, 0, 0,
                               0, 1, 0 }));
            */

            Console.ReadLine();
        }

    }
}
