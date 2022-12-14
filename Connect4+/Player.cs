using System;
using System.Collections.Generic;

namespace connect4
{
    internal sealed class Player
    {
        internal char PlayerChar;
        internal ConsoleColor Color;
        internal int Score;
        internal bool BotPlays;
        internal bool NextToPlay;

        private readonly List<ConsoleColor> _pickableColors = new List<ConsoleColor>
            {
                ConsoleColor.Magenta,
                ConsoleColor.Red,
                ConsoleColor.Yellow,
                ConsoleColor.Green,
                ConsoleColor.Blue,
                ConsoleColor.Cyan,
                ConsoleColor.White,
                ConsoleColor.Gray,
                ConsoleColor.Black
            };

        public void Initialize(char playerChar, ConsoleColor color = ConsoleColor.Black)
        {
            PlayerChar = playerChar;
            Color = color;
            Score = 0;
            Console.Clear();
            Console.Write($"   {PlayerChar} was ");
            Utils.PrintColorSample(Color);
            Console.WriteLine(" Choose a new color:");
            Console.Write("   ");

            for (var i = 1; i <= 9; i++)
            {
                Console.Write($"  {i} ");
            }

            Console.WriteLine();

            Console.Write("   ");
            Utils.PrintColorSample(_pickableColors);

            Console.WriteLine();
            Console.Write("   ");

            Color = Console.ReadKey().KeyChar switch
            {
                '1' => ConsoleColor.Magenta,
                '2' => ConsoleColor.Red,
                '3' => ConsoleColor.Yellow,
                '4' => ConsoleColor.Green,
                '5' => ConsoleColor.Blue,
                '6' => ConsoleColor.Cyan,
                '7' => ConsoleColor.White,
                '8' => ConsoleColor.Gray,
                '9' => ConsoleColor.Black,
                _ => Color
            };
            Console.WriteLine();

            if (Bot.IsImplemented())
            {
                Console.WriteLine($"   Enable bot for {PlayerChar}? y/N");

                BotPlays = Console.ReadKey().KeyChar.ToString().ToUpper() == "Y";

                Console.WriteLine();
                Console.WriteLine();
            }
        }

        public void ResetScore()
            => Initialize(PlayerChar, Color);

        public void ChangeActiveStatus()
            => NextToPlay = !NextToPlay;
    }
}
