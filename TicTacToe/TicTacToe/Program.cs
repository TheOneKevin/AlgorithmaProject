using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        static bool aiTurn = false;
        static bool game = false;

        static void Main(string[] args)
        {
            Console.WriteLine("This is Kevin Dai's Tic-Tac-Toe application.");
            Console.WriteLine("Row 1, Column 1 is the top-left most spot. All other spots follow left-right and up-down.");
            Console.WriteLine("Please select how you want to proceed (Player vs AI [pva] or AI vs AI [ava]):");
            string uin = Console.ReadLine();
            if (uin == "pva")
            {
                Console.WriteLine("Player vs AI selected!");
                PVA();
            }

            else if (uin == "ava")
            {
                Console.WriteLine("AI vs AI selected!");
            }

            Console.ReadKey();
        }

        static void PVA()
        {
            int[] board = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Console.WriteLine("Do you want to start first? [y/n]");
            if (Console.ReadKey().KeyChar == 'y')
                aiTurn = false;
            else
                aiTurn = true;
            game = true;
            while(game)
            {
                printBoard(board);
                game = false;
                if (aiTurn)
                    board = aiMove(board);
                else
                    board = playerMove(board);
            }
        }

        static void printBoard(int[] board)
        {
            Console.WriteLine("\n[Board]");
            Console.Write("_______\n");
            for (int y = 0; y < 3; y++)
            {
                Console.Write("|");
                for(int x = 0; x < 3; x++)
                {
                    if (board[y * 3 + x] == 1) Console.Write("X|");
                    else if (board[y * 3 + x] == 2) Console.Write("O|");
                    else if (board[y * 3 + x] == 0) Console.Write(" |");
                }
                Console.Write("\n");
            }
        }

        //AI move method
        static int[] aiMove(int[] board)
        {

            return board;
        }

        //Player move method
        static int[] playerMove(int[] board)
        {
            while (true)
            {
                Console.WriteLine("Enter row, then column number, seperated by a space:");
                string[] uin = Console.ReadLine().Split(' ');
                if(uin.Length == 2) //Some checkings
                {
                    int x = 0, y = 0;
                    //Check bounds and validity
                    bool s = int.TryParse(uin[0], out x) && int.TryParse(uin[1], out y) && x > 0 && x < 4 && y > 0 && y < 4;
                    if(s && board[y * 3 + x] == 0) //Check if is valid move
                    {
                        y--; x--; //Make suitable for array
                        board[y * 3 + x] = 2; //Player is id 2 always
                        break; //Exit out of input loop
                    }
                }

                Console.WriteLine("Not a valid move!");
            }

            aiTurn = true;
            printBoard(board);
            return board;
        }

        static int checkWin(int[] b)
        {
            if (b[0] == b[1] && b[1] == b[2]) return 1;
            if (b[3] == b[4] && b[4] == b[5]) return 1;
            if (b[6] == b[7] && b[7] == b[8]) return 1;

            if (b[0] == b[3] && b[3] == b[6]) return 1;
            if (b[1] == b[4] && b[4] == b[7]) return 1;
            if (b[2] == b[5] && b[5] == b[8]) return 1;

            if (b[0] == b[4] && b[4] == b[8]) return 1;
            if (b[2] == b[4] && b[4] == b[6]) return 1;

            return 0;
        }

        static int[] checkHalfWinPos(int[] b, int p)
        {
            int ret = { 0, 0 }; //x, y
            for(int i = 0; i < 9; i++)
            {
                if(b[i] == p)
                {

                }
            }

            return ret;
        }
    }
}
