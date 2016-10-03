using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryOfBabel;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class TestLibrary
    {
        [TestMethod]
        public void TestGenSuc()
        {
            string input = "1";
            Library lib = new Library();
            BookStruct page = lib.decodeHex(lib.encodeHex(input));
            Assert.AreEqual(page.searchText, input); //See if extraction is a success
            
            BookStruct page2 = lib.decodeHex(lib.encodeHex(input));
            //Assert.AreEqual(page, page2);

            string text = lib.generateBook(page, false);
            page2.page += 1;
            string text1 = lib.generateBook(page2, false);
            Assert.AreNotSame(text, text1);
            StringAssert.Contains(text, page.searchText);
        }

        [TestMethod]
        public void TestCompressSuc()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string compress = "StringStringStringStringStringStringStringStringStringStringStringStringStringString";
            string after = Compressor.ByteArrayToString(Compressor.Zip(compress));
            string unzip = Compressor.Unzip(Compressor.StringToByteArray(after));
            Assert.AreEqual(compress, unzip);
            sw.Stop();
            Console.WriteLine("Time elapsed: " + sw.Elapsed);
        }
    }
}
