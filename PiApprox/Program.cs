using System;
using System.IO;
using Deveel.Math;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Welcome to Kevin's PI APPROXIMATOR! Using Ramanujan's Pi Approximation 2 (1914)");
            Console.WriteLine("By: Kevin Dai (2017)");

            int iterations = 0;
            if(args.Length < 1) CalcPi(1);
            else if(int.TryParse(args[0], out iterations))
                CalcPi(iterations);
            else
                Console.WriteLine("Invalid Argument(s)!");

            sw.Stop();
            Console.WriteLine("Elapsed Time: " + sw.ElapsedMilliseconds + "ms");
        }

        static Object l0ck = new Object();
        static int acc = 0;
        static void CalcPi(int t)
        {
            Console.WriteLine("You have entered " + t + " iterations");
            Console.WriteLine("Calculating constant...");

            acc = t * 20;
            BigDecimal c = new BigDecimal(1);
            c = c.Divide(882, acc, RoundingMode.HalfUp);

            Console.WriteLine("Calculating summation...");
            Console.ForegroundColor = ConsoleColor.Blue;

            BigDecimal sumTerm = new BigDecimal(0, acc);
            Parallel.For(0, t, i =>
            {
                BigDecimal a = CalcTerm(i);

                lock(l0ck)
                {
                    sumTerm = sumTerm.Add(a);
                }
            });

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('\n');

            c = c.Multiply(sumTerm);
            
            BigDecimal pi1 = new BigDecimal(1);
            pi1 = pi1.Divide(c, acc, RoundingMode.HalfUp);
            pi1 = pi1.Multiply(4);

            string pi = pi1.ToString();

            Console.WriteLine("Printing reliable digits...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("π≈");
            //Check digits of pi against source
            int idx = 0; int digits = 1;
            StreamReader sr = new StreamReader(File.OpenRead("pi.txt"));
            while(!sr.EndOfStream && (idx < pi.Length))
            {
                char ch = (char) sr.Read();
                if(ch == pi[idx])
                {
                    Console.Write(pi[idx]);
                    digits++;
                }

                idx++;
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('\n');

            Console.WriteLine("Calculated " + digits + " digits of pi.");
        }

        static BigDecimal CalcTerm(int n)
        {
            Console.Write('.');
            BigDecimal neg1 = new BigDecimal(-1);
            neg1 = neg1.Pow(n);
            neg1 = neg1.Multiply(fact(4 * n));

            BigDecimal t1b = new BigDecimal(4);
            t1b = t1b.Pow(n);
            t1b = t1b.Multiply(fact(n));
            t1b = t1b.Pow(4);

            neg1 = neg1.Divide(t1b, acc, RoundingMode.HalfUp);

            BigDecimal t2 = new BigDecimal(21460);
            t2 = t2.Multiply(n);
            t2 = t2.Add(1123);

            BigDecimal t2b = new BigDecimal(882);
            t2b = t2b.Pow(2 * n);

            t2 = t2.Divide(t2b, acc, RoundingMode.HalfUp);

            return neg1.Multiply(t2);
        }

        static BigInteger fact(BigInteger aIn)
        {
            BigInteger ret = 1;
            if(aIn == 0) return ret;

            while(aIn > 0)
            {
                ret = ret.Multiply(aIn);
                aIn--;
            }

            return ret;
        }
    }
}
