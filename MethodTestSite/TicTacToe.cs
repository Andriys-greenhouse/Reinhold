using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MethodTestSite
{
    public enum WinningParty { Player1, Player2, Draw, None }
    public delegate void WriteMethod(string s);
    class TicTacToe
    {
        public event WriteMethod Outputing; //used to write out to the user

        char[,] Field;
        bool ended = false;
        bool P1Move = true;
        static string alphabet = "abcdefghijklmnopqrstuvwxyz";
        static char Player1 = 'X';
        static char Player2 = '0';
        int numberToWin;

        public TicTacToe(int Width, int Height, int InLineToWin, WriteMethod OutputMethod)
        {
            if (Width < 1 || Height < 1) { throw new ArgumentException("Width and height must be greather than 0."); }
            if (Width > alphabet.Length) { throw new ArgumentException($"Width must be smaller tahn {alphabet.Length}."); }
            Field = new char[Width, Height];

            for (int a = 0; a < Field.GetLength(0); a++)
            {
                for (int b = 0; b < Field.GetLength(1); b++)
                {
                    Field[a, b] = '☐';
                }
            }

            numberToWin = InLineToWin;

            Outputing += OutputMethod;
            Outputing.Invoke(this.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(" ");

            for (int i = 0; i < Field.GetLength(0); i++)
            {
                sb.Append(alphabet[i]);
            }
            sb.Append('\n');

            for (int a = 0; a < Field.GetLength(1); a++)
            {
                sb.Append(a + 1);

                for (int b = 0; b < Field.GetLength(0); b++)
                {
                    sb.Append(Field[a, b]);
                }

                if (a != Field.GetLength(1) - 1) sb.Append('\n');
            }

            return sb.ToString();
            //return base.ToString();
        }

        public WinningParty EvaluateGame()
        {
            List<char> diagonalL = new List<char>();
            List<char> diagonalR = new List<char>();
            char lastChar = '☐';
            int count = 0;

            //rows
            for (int y = 0; y < Field.GetLength(1); y++)
            {
                for (int x = 0; x < Field.GetLength(0); x++)
                {
                    if (lastChar == Field[x, y])
                    {
                        count++;
                        if (count == numberToWin - 1 && (lastChar == Player1 || lastChar == Player2)) { return lastChar == Player1 ? WinningParty.Player1 : WinningParty.Player2; }
                    }
                    else
                    {
                        lastChar = Field[x, y];
                        count = 0;
                    }
                }

                count = 0;
                lastChar = '☐';
            }

            //columns
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (lastChar == Field[x, y])
                    {
                        count++;
                        if (count == numberToWin - 1 && (lastChar == Player1 || lastChar == Player2)) { return lastChar == Player1 ? WinningParty.Player1 : WinningParty.Player2; }
                    }
                    else
                    {
                        lastChar = Field[x, y];
                        count = 0;
                    }
                }

                count = 0;
                lastChar = '☐';
            }

            //diagonals
            for (int x = 0; x < Field.GetLength(0); x++) //determins starting point
            {
                for (int i = 0; i < (Field.GetLength(0) < Field.GetLength(1) ? Field.GetLength(0) : Field.GetLength(1)); i++)
                {
                    //goes like / down from the point
                    try
                    {
                        diagonalL.Add(Field[x - i, i]);
                        diagonalR.Add(Field[x + i, i]);
                    }
                    catch (IndexOutOfRangeException) { }
                }

                foreach (char item in diagonalL)
                {
                    if (lastChar == item)
                    {
                        count++;
                        if (count == numberToWin && (lastChar == Player1 || lastChar == Player2)) { return lastChar == Player1 ? WinningParty.Player1 : WinningParty.Player2; }
                    }
                    else
                    {
                        lastChar = item;
                        count = 0;
                    }
                }

                foreach (char item in diagonalR)
                {
                    if (lastChar == item)
                    {
                        count++;
                        if (count == numberToWin && (lastChar == Player1 || lastChar == Player2)) { return lastChar == Player1 ? WinningParty.Player1 : WinningParty.Player2; }
                    }
                    else
                    {
                        lastChar = item;
                        count = 0;
                    }
                }

                count = 0;
                lastChar = '☐';
                diagonalL.Clear();
                diagonalR.Clear();
            }

            for (int y = 1; y < Field.GetLength(1); y++) //determins starting point
            {
                for (int i = 0; i < (Field.GetLength(0) < Field.GetLength(1) ? Field.GetLength(0) : Field.GetLength(1)); i++)
                {
                    try
                    {
                        diagonalR.Add(Field[Field.GetLength(0) - i, y + i]);
                        diagonalL.Add(Field[i, y + i]);
                    }
                    catch (IndexOutOfRangeException) { }
                }

                foreach (char item in diagonalL)
                {
                    if (lastChar == item)
                    {
                        count++;
                        if (count == numberToWin && (lastChar == Player1 || lastChar == Player2)) { return lastChar == Player1 ? WinningParty.Player1 : WinningParty.Player2; }
                    }
                    else
                    {
                        lastChar = item;
                        count = 0;
                    }
                }

                foreach (char item in diagonalR)
                {
                    if (lastChar == item)
                    {
                        count++;
                        if (count == numberToWin && (lastChar == Player1 || lastChar == Player2)) { return lastChar == Player1 ? WinningParty.Player1 : WinningParty.Player2; }
                    }
                    else
                    {
                        lastChar = item;
                        count = 0;
                    }
                }

                count = 0;
                lastChar = '☐';
                diagonalL.Clear();
                diagonalR.Clear();
            }

            //is there any space left?
            bool noMoreSpace = true;
            foreach (char tile in Field)
            {
                if (tile == '☐') { noMoreSpace = false; }
            }

            return noMoreSpace ? WinningParty.Draw : WinningParty.None;
        }

        public WinningParty MakeMove(string Coordinates) //format: a1; A1
        {
            if (!ended)
            {
                Coordinates = Coordinates.ToLower();
                Regex rx = new Regex(@"^\w\d*$");
                if (!rx.IsMatch(Coordinates)) { throw new ArgumentException("Coordinates have to be formated either like this A15 or like this c7."); }
                if (!alphabet.Contains(Coordinates[0].ToString())) { throw new ArgumentException("Hm, I can't make up, what coordinate the first character represents. Please try again."); }

                int x = alphabet.IndexOf(Coordinates[0]);
                int y = int.Parse(Coordinates.Substring(1)) - 1;
                if (y > Field.GetLength(1) || x > Field.GetLength(0)) { throw new ArgumentException("Coordinate is too high."); }

                if (Field[x, y] == '☐') { Field[x, y] = P1Move ? Player1 : Player2; }
                P1Move = !P1Move;
                Outputing.Invoke(this.ToString());
            }

            return EvaluateGame();
        }
    }
}
