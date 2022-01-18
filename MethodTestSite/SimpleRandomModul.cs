using System;

namespace MethodTestSite
{
    public enum RockPaperScissors { Rock, Paper, Scissors }
    public enum GameResult { Win, Draw, Lose }

    class RPSModule
    {
        static Random rnd = new Random();

        public static GameResult RPSEvaluator(RockPaperScissors OponenetsMove, RockPaperScissors MyMove)
        {
            if (MyMove == OponenetsMove) { return GameResult.Draw; }
            if (MyMove == RockPaperScissors.Rock && OponenetsMove == RockPaperScissors.Scissors) { return GameResult.Win; }
            if (MyMove == RockPaperScissors.Scissors && OponenetsMove == RockPaperScissors.Rock) { return GameResult.Lose; }
            return MyMove > OponenetsMove ? GameResult.Win : GameResult.Lose;
        }
        public static RockPaperScissors RPSGenerate()
        {
            return (RockPaperScissors)rnd.Next(0, 2);
        }
    }

    class RandomModule
    {
        static Random rnd = new Random();
        public static GameResult GuessToTen(int Guess)
        {
            return Guess == rnd.Next(1, 11) ? GameResult.Win : GameResult.Lose;
        }
        public static double RandomNumber(double Min, double Max, bool WholeNumber = true)
        {
            if (Max < Min) { throw new ArgumentException("Maximum must be greater than minimum."); }
            int whole = rnd.Next((int)Min + 1, (int)Max + 1);
            double dcml = 0;

            if (!WholeNumber)
            {
                do
                {
                    dcml = rnd.NextDouble();
                } while (whole + dcml < Min || whole + dcml > Max);
            }

            return whole + dcml;
        }
    }
}
