using System;
using System.Globalization;
using System.Linq;

namespace connect4
{
    internal sealed class Bot
    {
        internal static int MakeMove(Player botPlayer)
        {
            // input format: #of occurences in a row(prio), i(widthIndex), j(heightIndex), iDirection, jDirection
            var input = Utils.CheckForMultipleInARow(botPlayer, true).
                              Split(';').
                              Select(x => x.Split('/')).
                              ToArray();

            input = Shuffle(input);

            var inputSortedByPrio = input.OrderByDescending(x => x.First());

            foreach (var i in inputSortedByPrio)
            {
                if (string.IsNullOrEmpty(i[0]))
                {
                    break;
                }

                if (i[0] == "0,0"
                    && Board.HasColumnSpace(Board.GetWidth() / 2 + 1))
                {
                    return Board.GetWidth() / 2 + 1;
                }

                for (var j = 3; j >= 0; j--)
                {
                    var widthIndex = int.Parse(i[1]) + int.Parse(i[3]) * j;

                    if (Board.IsCellPlayable(
                            widthIndex,
                            int.Parse(i[2]) + int.Parse(i[4]) * j)
                        && (IsMoveGood(botPlayer, widthIndex + 1)
                            || double.Parse(i[0], new CultureInfo("de-DE")) == 3.5))
                    {
                        return widthIndex + 1;   // converting from index to column number
                    }
                }
            }

            for (var i = 1; i <= Board.GetWidth(); i++)
            {
                if (Board.HasColumnSpace(i))
                {
                    return i;
                }
            }

            Console.WriteLine("   Bot.exe flipped the table!!!");

            return -1;
        }

        // TODO: Next Step -> don't play if makes 3 but other can block the 4th but do anyway if bot would play on the 2s anyway
        private static bool IsMoveGood(Player botPlayer, int testColumn)
        {
            var isGood = true;
            Board.ManualPlaceInColumn(testColumn, botPlayer);
            var input = Utils.CheckForMultipleInARow(botPlayer, true).
                              Split(';').
                              Select(x => x.Split('/')).
                              ToArray();

            foreach (var i in input)
            {
                if (double.Parse(i[0]) < 3)
                {
                    continue;
                }
                
                for (var j = 3; j >= 0; j--)
                {
                    var widthIndex = int.Parse(i[1]) + int.Parse(i[3]) * j;

                    if (Board.IsCellPlayable(
                            widthIndex,
                            int.Parse(i[2]) + int.Parse(i[4]) * j))
                    {
                        isGood = false;
                    }
                }
            }

            Board.RemoveHighestPieceInColumn(testColumn);

            return isGood;
        }

        private static string[][] Shuffle(string[][] list)
        {
            var rng = new Random();
            var n = 0;

            while (list[0][0] == list[n][0])
            {
                n++;

                if (n == list.Count())
                {
                    break;
                }
            }

            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }

        internal static bool IsImplemented()
            => true;
    }
}
