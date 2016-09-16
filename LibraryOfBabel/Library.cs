#define SHOW_TIME //Comment to not show time elapsed

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntXLib;
using System.Diagnostics;

namespace LibraryOfBabel
{
    public class Library
    {
        #region Variables

        char[] bases = "0123456789abcdefghijklmnopqrstuvwxyz .,".ToCharArray(); //Our custom alphabet
        char[] hexes = "0123456789ABCDEF".ToCharArray(); //Hexadecimal

        const int answerToLifeTheUniverseAndEverything = 42; //Hehe

        public Library()
        {
            IntX.GlobalSettings.ParseMode = ParseMode.Fast;
            IntX.GlobalSettings.MultiplyMode = MultiplyMode.AutoFht;
        }

        #endregion

        #region Encoding/Decoding

        //We encode all the infornmation into one nice hexagon address!
        public string encodeHex(string text)
        {
            string addr1 = Compressor.ByteArrayToString(Compressor.Zip(text)); //Convert text into byte stream into hex
            string addr2 = convertToBase(convertFromBase(text, bases).ToString(), hexes); //Convert text to IntX into hex
            string addr;
            //Which one is shorter? We prefer the shorter one!
            if (addr1.Length <= addr2.Length)
            {
                addr = addr1 + '1'; //Set the last charater to be 1 if compressed
                Console.WriteLine("Hexagon: " + addr);
            }
            else
            {
                addr = addr2 + '0'; //And 0 if not compressed
                Console.WriteLine("Hexagon: " + addr);
            }

            IntX seed = convertFromBase(text, bases); //Convert text into IntX using bases

            //We get the wall, shelf and volume
            RNG.setSeeds(seed, seed % answerToLifeTheUniverseAndEverything);
            IntX wall = RNG.getRRandom(1, 4);
            IntX shelf = RNG.getRRandom(1, 5);
            IntX vol = RNG.getRRandom(1, 32);
            
            //Then get the page and location on the page that the text will appear on
            Random r = new Random((int)RNG.getRand());

            int loc = r.Next(1, 410);
            int locOnPage = r.Next(3200);

            //Display the information
            Console.WriteLine("Wall: "    + wall);
            Console.WriteLine("Shelf: "   + shelf);
            Console.WriteLine("Volume: "  + vol);
            Console.WriteLine("Page: "    + loc);
            Console.WriteLine("Location on Page: " + locOnPage);

            //And return the hexagon address
            return addr;
        }

        //We decode and extract the information from the hexagon address
        public BookStruct decodeHex(string hex)
        {
            string text; //The original user search string
            //We first test the last character if its one,
            //Then it's compressed. Else, its not
            if (hex[hex.Length - 1] == '1')
            {
                text = hex.Substring(0, hex.Length - 1); //Trim the ending
                text = Compressor.Unzip(Compressor.StringToByteArray(text)); //Convert hex into byte array into decompressed text
            }
            else
            {
                text = hex.Substring(0, hex.Length - 1);
                text = convertToBase(convertFromBase(text, hexes).ToString(), bases); //Revert back to original state
            }

            IntX seed = convertFromBase(text, bases); //Seed we used in the encode hex method
            RNG.setSeeds(seed, seed % answerToLifeTheUniverseAndEverything);

            //Because the seed is the same, and unique per user-input, we can
            //expect the same wall, shelf and volume to be outputted as when we
            //generated the values
            IntX e_wall = RNG.getRRandom(1, 4);
            IntX e_shelf = RNG.getRRandom(1, 5);
            IntX e_vol = RNG.getRRandom(1, 32);
            Random r = new Random((int)RNG.getRand());

            //We generated this in the encodehex but here we generate it again
            //and due to the seed being the same, we should get the same results
            int loc = r.Next(1, 410);
            int locOnPage = r.Next(3200);

            //Dump all those information into an object
            BookStruct ret = new BookStruct(e_wall, e_shelf, e_vol, loc, hex)
            {
                expectedLoc = locOnPage,
                searchText = text,
                expectedPage = loc
            };

            return ret;
        }

        #endregion

        #region Generation

        //Method to generate the book
        public string generateBook(BookStruct bookS, bool highlight)
        {
            //So here we extract the information from the BookStruct object
            IntX seed = convertFromBase(bookS.searchText, bases);

            //We make a seed based on the location and page
            int seed2 = (int)(bookS.wall * bookS.shelf * bookS.vol * bookS.page * (seed % 10));
            Random r = new Random(seed2); //And set this random to use this seed

            //Let's make a new array for the one page. We know each page is 3200 chars long beforehand
            char[] page = new char[3200];
            bool flag = false;
            for (int i = 0; i < 3200;)
            {
                if ((i == bookS.expectedLoc) && (bookS.page == bookS.expectedPage))
                    flag = true; //Here, we raise the flag, saying that the page and location needs the user search text!
                if (flag)
                {
                    if ((i - bookS.expectedLoc) >= bookS.searchText.Length)
                        flag = false;
                    else
                    {
                        //So, we be all sneaky and replace the randomly generated text with the user search text
                        page[i] = bookS.searchText[i - (int)bookS.expectedLoc];
                        if(highlight)
                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(page[i]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                else
                {
                    //Generate random text from our alphabet
                    page[i] = bases[r.Next(bases.Length)];
                    Console.Write(page[i]);
                }
                i++;
            }

            return new string(page); //And return the page
        }

        #endregion

        #region Base Conversions

        //I added <summaries> because I get confused which one is which :P

        /// <summary>
        /// This will convert our input into our custom base format.
        /// </summary>
        /// <param name="input">The input will be assumed to be an IntX number. Or else this will return null.</param>
        /// <param name="baseAlpha">The custom alphabet for conversion</param>
        /// <returns>Null if input was improperly formatted. Else, returns the converted output string.</returns>
        public string convertToBase(string input, char[] baseAlpha)
        {

#if (SHOW_TIME)
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            try
            {
                IntX x = IntX.Parse(input); //We'll deal with ridiculously large numbers
                StringBuilder sb = new StringBuilder();
                int targetBase = baseAlpha.Length;
                do
                {
                    sb.Append(baseAlpha[(int)(x % targetBase)]);
                    x /= targetBase;
                } while (x > 0);
                char[] garbage = sb.ToString().ToCharArray();
                Array.Reverse(garbage);
#if (SHOW_TIME)
                sw.Stop();
                Debug.WriteLine("Time elapsed: " + sw.ElapsedMilliseconds + " ms");
#endif
                return new string(garbage);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected IntX parse format! (" + e.Message + ")");
                return null;
            }
        }

        /// <summary>
        /// This will convert a string into a base-10 number.
        /// </summary>
        /// <param name="input">A string that uses our custom base.</param>
        /// <param name="alphaBase">Custom base alphabet</param>
        /// <returns>-1 when input was improper. Else, returns the converted number.</returns>
        public IntX convertFromBase(string input, char[] alphaBase)
        {

#if (SHOW_TIME)
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            //Convert to base 10 by: (char val at n * base length ^ n-1)
            //So convert abc into base 10 is 12 * 39 ^ 0 + 11 * 39 ^ 1 + 10 * 39 ^ 2
            //Because c is 12, b is 11, a is 10 in our custom base
            //if (input.Length > 3200) { Console.WriteLine("Input too large!");  return null; }
            try
            {
                IntX x = 0;
                char[] chars = input.ToCharArray();
                Array.Reverse(chars); //We need to reverse the array, so we work from the first digit up
                int targetBase = alphaBase.Length;
                uint i = 0;
                foreach (char c in chars)
                {
                    if(Array.IndexOf(alphaBase, c) >= 0)
                        x += Array.IndexOf(alphaBase, c) * (IntX.Pow(targetBase, i));
                    i++;
                }
#if (SHOW_TIME)
                sw.Stop();
                Debug.WriteLine("Time elapsed: " + sw.ElapsedMilliseconds + " ms");
#endif
                return x;
            }
            catch (Exception e)
            {
                Console.WriteLine("Uh oh! Something bad happened. " + e.Message);
                return null;
            }
        }

        #endregion
    }
}
