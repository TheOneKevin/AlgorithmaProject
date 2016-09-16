using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntXLib;

namespace LibraryOfBabel
{
    public class RNG
    {
        static IntX seed1, seed2;
        static RNG()
        {
            seed1 = 123454321;
            seed2 = 345654321;
        }

        public static IntX getRand()
        {
            seed2 = 36969 * (seed2 & 65535) + (seed2 >> 16);
            seed1 = 18000 * (seed1 & 65535) + (seed1 >> 16);
            return (seed2 << 16) + seed1;
        }

        public static void flushSeeds(bool zeroFlush)
        {
            if(zeroFlush)
            {
                seed1 = 0;
                seed2 = 0;
            }
            else
            {
                seed1 = 123454321;
                seed2 = 345654321;
            }
        }

        public static void setSeeds(IntX u, IntX v) { seed1 = u; seed2 = v; }
        public static void setSeed1(IntX seed) { seed1 = seed; }
        public static void setSeed2(IntX seed) { seed2 = seed; }

        public static IntX getRRandom(int max, int min)
        {
            return min + getRand() % (max - min);
        }
    }
}
