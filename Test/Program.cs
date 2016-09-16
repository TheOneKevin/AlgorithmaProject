using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryOfBabel;

namespace Test
{
    class Program
    {
        static Library lib;
        static void Main(string[] args)
        {
            lib = new Library();
            Console.WriteLine("Probram inited!");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("PERFORMANCE CHECK FOR COMPRESSOR");
            Console.ForegroundColor = ConsoleColor.White;
            Compressor.performanceCheck();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("==================================");
            Console.ForegroundColor = ConsoleColor.White;

            if (args.Length == 0)
            {
                while (true)
                {
                    searchLibrary();
                    getBookInput();
                }
            }

            else
            {
                foreach(string s in args)
                {
                    if (s == "--search")
                        searchLibrary();
                    else if (s == "--generate")
                        getBookInput();
                    else
                        Console.WriteLine("Error: Invalid paramenter " + s);
                }
            }

            //Console.ReadKey();
        }

        static void searchLibrary()
        {
            Console.Write("\nEnter your search string: ");
            string s = Console.ReadLine();
            string addr = lib.encodeHex(s);
            Console.WriteLine("Decoding hex:");
            Console.WriteLine("================================================");
            lib.generateBook(lib.decodeHex(addr), true);
        }

        static void getBookInput()
        {
            Console.Write("\nEnter hexagon: ");
            string addr = Console.ReadLine(); Console.Write('\n');

            Console.Write("Enter wall: ");
            int wall; int.TryParse(Console.ReadLine(), out wall);
            Console.Write('\n');

            Console.Write("Enter shelf: ");
            int shelf; int.TryParse(Console.ReadLine(), out shelf);
            Console.Write('\n');

            Console.Write("Enter volume: ");
            int vol; int.TryParse(Console.ReadLine(), out vol);
            Console.Write('\n');

            Console.Write("Enter page: ");
            int page; int.TryParse(Console.ReadLine(), out page);
            Console.Write('\n');

            Console.WriteLine("================================================");

            BookStruct book = lib.decodeHex(addr);
            book.wall = wall; book.shelf = shelf; book.vol = vol; book.page = page;
            lib.generateBook(book, false);
        }
    }
}
