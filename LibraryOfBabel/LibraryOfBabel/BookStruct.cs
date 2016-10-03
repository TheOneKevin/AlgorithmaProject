using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntXLib;

namespace LibraryOfBabel
{
    public class BookStruct
    {
        //This is the string that belongs at expected*
        public string searchText = "";
        //These are the locations of the text that belongs on a page of a book
        public IntX expectedLoc = 0, expectedPage = 0;

        //These variables below are the ones that represent the pages we want to display
        //These are the locations of the book and page we want to DISPLAY
        public IntX wall, shelf, vol, page;
        //This is the hexagon address
        public string hexagonAddr;

        public BookStruct(IntX wall, IntX shelf, IntX vol, IntX page, string hex)
        {
            this.wall = wall;
            this.shelf = shelf;
            this.vol = vol;
            this.page = page;
            hexagonAddr = hex;
        }
    }
}
