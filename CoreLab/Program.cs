﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoreLab
{
    class Program
    {
        static void Main(string[] args)
        {
            Core cr = new Core();
            string insides;
            using (StreamReader sr = new StreamReader("input.txt"))
            {
                insides = sr.ReadToEnd();
            }
            cr.Train(insides, new int[] { 16, 16, 8 }, 50000);
            cr.Process("Hi, who are you?");
        }
        static void VavKavTest()
        {
            int[] layers = new int[] { 9, 16, 8, 8, 2 };
            float[] outputs;
            Dictionary<float[], float[]> inputs = new Dictionary<float[], float[]>();

            /*
            inputs.Add(new float[] { 0, 0, 1 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 0, 0, 0 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 0, 1, 1 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 0, 1, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 1, 1 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 1, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 0, 1 }, new float[] { 0, 1 });
            //inputs.Add(new float[] { 1, 0, 0 }, new float[] { 0, 1 });
            */
            /*
            inputs.Add(new float[] { 0, 1 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 25, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 152 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 35, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 584, 254 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 78, 25 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 100, 152 }, new float[] { 0, 1 });
            */
            /*
            inputs.Add(new float[] { 0, 0, 0, 0 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 0, 0, 0, 1 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 0, 0, 1, 0 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 0, 0, 1, 1 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 0, 1, 0, 0 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 0, 1, 0, 1 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 0, 1, 1, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 0, 1, 1, 1 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 1, 0, 0, 0 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 1, 0, 0, 1 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 0, 1, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 0, 1, 1 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 1, 1, 0, 0 }, new float[] { 1, 0 });
            inputs.Add(new float[] { 1, 1, 0, 1 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 1, 1, 1, 0 }, new float[] { 0, 1 });
            inputs.Add(new float[] { 1, 1, 1, 1 }, new float[] { 1, 0 });
            */

            inputs.Add(new float[] { 0, 0, 1,
                                      0, 1, 0,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 1, 1,
                                      0, 0, 0,
                                      0, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 0,
                                      1, 1, 1,
                                      0, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 0,
                                      0, 0, 0,
                                      1, 1, 1 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 1,
                                      0, 0, 1,
                                      0, 0, 1 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 1, 0,
                                      0, 1, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 0, 0,
                                      1, 0, 0,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 1, 0,
                                      0, 0, 1 }, new float[] { 1, 0 });
            
            inputs.Add(new float[] { 0, 0, 1,
                                      0, 1, 1,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 1, 1,
                                      1, 0, 0,
                                      0, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 1, 0,
                                      1, 1, 1,
                                      0, 0, 1 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 0,
                                      0, 1, 0,
                                      1, 1, 1 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 1,
                                      0, 0, 1,
                                      1, 0, 1 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 1, 1,
                                      0, 1, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 0, 1,
                                      1, 0, 0,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 1, 0,
                                      1, 0, 1 }, new float[] { 1, 0 });
            
            inputs.Add(new float[] { 0, 1, 0,
                                      0, 1, 0,
                                      0, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 1, 0,
                                      1, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 0, 0,
                                      0, 0, 0,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 1, 0,
                                      0, 0, 0,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 1,
                                      1, 0, 0,
                                      0, 0, 0 }, new float[] { 0, 1 });
            
            inputs.Add(new float[] { 1, 1, 0,
                                      0, 0, 0,
                                      0, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 0, 0,
                                      1, 1, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 1,
                                      0, 0, 0,
                                      0, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 0, 0,
                                      1, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      1, 0, 1,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 1,
                                      0, 1, 0,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 0, 0,
                                      0, 1, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 1, 1,
                                      0, 0, 1,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 0, 0,
                                      1, 1, 0 }, new float[] { 0, 1 });
            
            inputs.Add(new float[] { 0, 1, 1,
                                      0, 0, 1,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 0, 0,
                                      1, 0, 0,
                                      1, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 0, 1,
                                      1, 0, 0 }, new float[] { 0, 1 });
            
            inputs.Add(new float[] { 1, 0, 1,
                                      0, 0, 0,
                                      0, 1, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 0, 0,
                                      1, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 1, 1,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 0, 0,
                                      1, 1, 0,
                                      0, 1, 0 }, new float[] { 0, 1 });
            
            inputs.Add(new float[] { 0, 1, 1,
                                      0, 1, 1,
                                      0, 0, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 0, 1,
                                      1, 0, 0,
                                      1, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 0, 1,
                                      1, 1, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 1,
                                      0, 1, 0,
                                      0, 1, 0 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 0, 1,
                                      1, 0, 1 }, new float[] { 0, 1 });

            inputs.Add(new float[] { 0, 1, 0,
                                      0, 1, 1,
                                      1, 0, 0 }, new float[] { 0, 1 });
            ////
            inputs.Add(new float[] { 0, 1, 0,
                                      1, 1, 0,
                                      0, 1, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 0, 0,
                                      1, 1, 0,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 1,
                                      1, 1, 0,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 1,
                                      1, 1, 0,
                                      1, 0, 0 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 1, 0, 0,
                                      0, 1, 0,
                                      1, 0, 1 }, new float[] { 1, 0 });

            inputs.Add(new float[] { 0, 0, 1,
                                      1, 0, 1,
                                      0, 0, 1 }, new float[] { 1, 0 });


            VavKavNetwork subject = new VavKavNetwork(layers);

            subject.Train(inputs, 50000); //500000 feasable, 1000000 shure

            /*
            Console.WriteLine(subject.PrintableResult(new float[] { 1, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 0, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 1, 0, 0 }));
            */
            /*
            Console.WriteLine(subject.PrintableResult(new float[] { 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 0, 1 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 50, 25 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 25, 50 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 125, 115 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 115, 125 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 120, 125 }));
            */
            /*
            Console.WriteLine(subject.PrintableResult(new float[] { 0, 1, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 1, 0, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 1, 1, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(new float[] { 1, 0, 1, 1 }));
            */
            
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 0, 0,
                               0, 1, 0,
                               0, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 0, 1,
                               0, 0, 0,
                               0, 0, 1 }));
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 1, 1,
                               0, 0, 0,
                               0, 0, 0 }));
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 0, 0,
                               1, 0, 0,
                               0, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 0, 0,
                               1, 0, 0,
                               1, 1, 0 }));
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 0, 1,
                               0, 0, 0,
                               0, 1, 1 }));
            Console.WriteLine(subject.PrintableResult(
                new float[] { 1, 0, 1,
                               0, 1, 0,
                               0, 0, 1 }));


            Console.ReadLine();
        }

    }
}