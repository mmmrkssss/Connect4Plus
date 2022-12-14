using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CognitiveComplexity
// ReSharper disable MissingLinebreak

namespace connect4
{
    internal static class Utils
    {
        private static int _customGameMode;

        internal static int GetGameMode()
            => _customGameMode;

        internal static void ChangePlayer(Player playerX, Player playerO)
        {
            playerX.ChangeActiveStatus();
            playerO.ChangeActiveStatus();
        }

        internal static Player GetActivePlayer(Player playerX, Player playerO)
            => playerX.NextToPlay ? playerX : playerO;

        internal static void MakeNextMove(
            Player playerX,
            Player playerO)
            => Board.PlaceInColumn(GetNextValidMove(playerX, playerO, Board.GetLastColumnPlayed()), playerX, playerO);

        private static int GetNextValidMove(
            Player playerX,
            Player playerO,
            int lastColumnPlayed)
        {
            var toPlay = GetActivePlayer(playerX, playerO);

            if (toPlay.BotPlays)
            {
                return Bot.MakeMove(toPlay);
            }

            while (true)
            {
                Console.Write("   Enter the number of the column you want to play: ");
                var columnPlayed = (Board.GetWidth() < 10)
                    ? Console.ReadKey().KeyChar.ToString()
                    : Console.ReadLine() ?? "F";

                if (IsDigit(columnPlayed, 1, Board.GetWidth()))
                {
                    var column = int.Parse(columnPlayed);

                    if (Board.HasColumnSpace(column))
                    {
                        return column;
                    }

                    Console.Clear();
                    Console.Write(
                        $"   Last play was column {lastColumnPlayed} by ");
                    PrintInCustomColor(
                        GetPlayerByChar(
                            playerX,
                            playerO,
                            Board.GetCharFromBoard(
                                lastColumnPlayed - 1,
                                Board.GetLowestUnoccupiedCellIndex(
                                    lastColumnPlayed)
                                + 1)));
                    Console.WriteLine(".");
                    Console.Write("   ");
                    PrintInCustomColor(toPlay);
                    Console.WriteLine(" is next.");
                    Board.PrintBoard(playerX, playerO);
                    Console.Write(
                        $"   That column won't work, ");
                    PrintInCustomColor(toPlay);
                    Console.WriteLine(" please chose a different column.");
                }
                else
                {
                    Console.Clear();
                    Console.Write(
                        $"   Last play was column {lastColumnPlayed} by ");
                    PrintInCustomColor(
                        GetPlayerByChar(
                            playerX,
                            playerO,
                            Board.GetCharFromBoard(
                                lastColumnPlayed - 1,
                                Board.GetLowestUnoccupiedCellIndex(
                                    lastColumnPlayed)
                                + 1)));
                    Console.WriteLine(".");
                    Console.Write("   ");
                    PrintInCustomColor(toPlay);
                    Console.WriteLine(" is next.");
                    Board.PrintBoard(playerX, playerO);
                    Console.Write(
                        $"   That input won't work, ");
                    PrintInCustomColor(toPlay);
                    Console.WriteLine(" please chose a valid column.");
                }
            }
        }

        internal static bool DidGameEnd(Player playerX, Player playerO)
        {
            if (CheckForMultipleInARow(GetActivePlayer(playerX, playerO)) == "4")
            {
                Console.Clear();
                Board.PrintBoard(playerX, playerO);
                Console.Write("   ");
                PrintInCustomColor($" {GetActivePlayer(playerX, playerO).PlayerChar} ", GetActivePlayerColor(playerX, playerO));
                Console.WriteLine(" won!\n");

                GetActivePlayer(playerX, playerO).Score++;

                return false;
            }

            if (Board.GetMovesMade() < Board.GetHight() * Board.GetWidth())
            {
                return true;
            }

            Console.WriteLine("   Draw!\n");
            playerX.ChangeActiveStatus();
            playerO.ChangeActiveStatus();

            return false;
        }

        internal static string CheckForMultipleInARow(
            Player activePlayer,
            bool extendedInfo = false,
            int columnToCheckAhead = -1)
        {
            var width = Board.GetWidth();
            var height = Board.GetHight();
            var result = "";
            var activePlayerChar = activePlayer.PlayerChar;

            var iDirection = 0;
            var jDirection = 1;

            // verticalCheck
            for (var j = 0; j < height - 3; j++)
            {
                for (var i = 0; i < width; i++)
                {
                    if (extendedInfo)
                    {
                        result += CheckForSameInAny4Cells(i, j, iDirection, jDirection, activePlayerChar);
                    }

                    if (CheckForSameInAny4Cells(i, j, iDirection, jDirection).Equals("4"))
                    {
                        ConvertWinningCharsToLowerCase(i, j, 0, 1);

                        result = "4";
                    }
                }
            }

            iDirection = 1;
            jDirection = 0;

            // horizontalCheck
            for (var i = 0; i < width - 3; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (extendedInfo)
                    {
                        result += CheckForSameInAny4Cells(i, j, iDirection, jDirection, activePlayerChar);
                    }

                    if (CheckForSameInAny4Cells(i, j, iDirection, jDirection).Equals("4"))
                    {
                        ConvertWinningCharsToLowerCase(i, j, 1, 0);

                        result = "4";
                    }
                }
            }

            iDirection = -1;
            jDirection = 1;

            // ascendingDiagonalCheck
            for (var i = 3; i < width; i++)
            {
                for (var j = 0; j < height - 3; j++)
                {
                    if (extendedInfo)
                    {
                        result += CheckForSameInAny4Cells(i, j, iDirection, jDirection, activePlayerChar);
                    }

                    if (CheckForSameInAny4Cells(i, j, iDirection, jDirection).Equals("4"))
                    {
                        ConvertWinningCharsToLowerCase(i, j, -1, 1);

                        result = "4";
                    }
                }
            }

            iDirection = -1;
            jDirection = -1;

            // descendingDiagonalCheck
            for (var i = 3; i < width; i++)
            {
                for (var j = 3; j < height; j++)
                {
                    if (extendedInfo)
                    {
                        result += CheckForSameInAny4Cells(i, j, iDirection, jDirection, activePlayerChar);
                    }

                    if (CheckForSameInAny4Cells(i, j, iDirection, jDirection).Equals("4"))
                    {
                        ConvertWinningCharsToLowerCase(i, j, -1, -1);

                        result = "4";
                    }
                }
            }

            if (extendedInfo && !String.IsNullOrEmpty(result))
            {
                result = result.Remove(result.Length - 1);
            }

            return result;
        }

        private static string CheckForSameInAny4Cells(int widthIndex, int heightIndex, int iDirection, int jDirection, char? prio = null)
        {
            var board = Board.GetBoard();
            var c = char.ToUpper(board[widthIndex, heightIndex]);
            var output = 0.0;
            string extendedOutput = null;

            for (var i = 0; i <= 3; i++)
            {
                if (c == ' ')
                {
                    c = char.ToUpper(board[widthIndex + iDirection * i, heightIndex + jDirection * i]);
                }

                if (char.ToUpper(board[widthIndex + iDirection * i, heightIndex + jDirection * i])
                    == c)
                {
                    if (!c.Equals(' '))
                    {
                        output++;
                    }
                }
                else
                {
                    if (!board[widthIndex + iDirection * i, heightIndex + jDirection * i].Equals(' '))
                    {
                        output = -10;
                    }
                }
            }

            if (c.Equals(' ')
                || output < 0)
            {
                output = 0;
            }

            if (prio.Equals(c))
            {
                output += .5;
            }

            if (output >= 0)
            {
                extendedOutput =
                    $"{output:F1}/{widthIndex}/{heightIndex}/{iDirection}/{jDirection};";
            }

            return prio.HasValue ? extendedOutput : output.ToString("F0");
        }

        private static void ConvertWinningCharsToLowerCase(
            int i,
            int j,
            int iDirection,
            int jDirection)
        {
            var board = Board.GetBoard();
            board[i, j] = char.ToLower(board[i, j]);
            board[i + iDirection, j + jDirection] = char.ToLower(board[i + iDirection, j + jDirection]);
            board[i + (iDirection * 2), j + (jDirection * 2)] = char.ToLower(board[i + (iDirection * 2), j + (jDirection * 2)]);
            board[i + (iDirection * 3), j + (jDirection * 3)] = char.ToLower(board[i + (iDirection * 3), j + (jDirection * 3)]);
        }

        internal static void MakeCenterCellsLower(int cellsToLower)
        {
            var width = Board.GetWidth();
            var height = Board.GetHight();
            var board = Board.GetBoard();
            var i = 0;

            if (width % 2 == 0) // 8 -> index 3,4;  7 -> 3
            {
                while (cellsToLower > 0)
                {
                    board[width / 2 + i, height - 1] = char.ToLower(
                        board[width / 2 + i, height - 1]);
                    board[width / 2 - 1 - i, height - 1] = char.ToLower(
                        board[width / 2 - 1 - i, height - 1]);
                    i++;
                    cellsToLower -= 2;
                }
            }
            else
            {
                cellsToLower++;

                while (cellsToLower > 0)
                {
                    board[width / 2 + i, height - 1] = char.ToLower(
                        board[width / 2 + i, height - 1]);
                    board[width / 2 - i, height - 1] = char.ToLower(
                        board[width / 2 - i, height - 1]);
                    i++;
                    cellsToLower -= 2;
                }
            }
        }

        internal static void PrintCharColoredIfWinning(
            int widthIndex,
            int heightIndex,
            char[,] field,
            Player playerX,
            Player playerO)
        {
            if (IsLowerCase(field[widthIndex, heightIndex]))
            {
                PrintInCustomColor(
                    field,
                    widthIndex,
                    heightIndex,
                    null,
                    PickHighlightColor(playerX.Color, playerO.Color),
                    playerX,
                    playerO);
            }
            else
            {
                Board.PrintCell(widthIndex, heightIndex, field, ConsoleColor.Black, playerX, playerO);
            }
        }

        private static bool IsLowerCase(char c)
            => c.ToString().ToLower().Equals(c.ToString()) && c != ' ';

        internal static bool AskIfKeepPlayingResetOrExit(
            Player playerX,
            Player playerO)
        {
            Console.WriteLine("   Play again?");
            Console.WriteLine("   Yes (y) With new settings (s) No (n) Main menu (m)");

            while (true)
            {
                Console.Write("   ");

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:

                        return true;
                    case ConsoleKey.S:
                        OpenGameModeMenu();

                        return true;
                    case ConsoleKey.N:

                        return false;
                    case ConsoleKey.M:
                        OpenMenu();
                        playerX.ResetScore();
                        playerO.ResetScore();

                        return true;
                    default:
                        Console.WriteLine();

                        break;
                }
            }
        }

        private static void PrintHeader()
        {
            Console.Clear();
            var output =
                @"
 %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    %%%%%%    Mmrksss presents:                            %%%
  %%                                              %%      %%%%
 %%         %%%%  %% %%%  %% %%%   %%%%   %%%%% %%%%%%   %% %%
 %%        %%  %% %%%  %% %%%  %% %%  %% %%       %%    %%  %%     %%
 %%        %%  %% %%   %% %%   %% %%%%%  %%       %%   %%%%%%%%% %%%%%%
  %%       %%  %% %%   %% %%   %% %%     %%       %%        %%     %%
    %%%%%%  %%%%  %%   %% %%   %%  %%%%   %%%%%    %%%      %%

 %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

";

            foreach (var c in output)
            {
                if (c.Equals('%'))
                {
                    PrintInCustomColor(" ", ConsoleColor.Red);
                }
                else
                {
                    PrintInCustomColor(c.ToString(), ConsoleColor.Black);
                }
            }

            Console.WriteLine();
        }

        internal static void OpenMenu()
        {
            PrintHeader();
            Console.WriteLine("                     - Press any key to start -");

            Console.ReadKey();
            Console.Clear();
            OpenGameModeMenu();
        }

        private static void OpenGameModeMenu()
        {
            PrintHeader();
            Console.WriteLine("                     - (N)  Normal  mode  -");
            Console.WriteLine("                     - (t)  Tetris  mode  - \n");
            Console.WriteLine("                     - (s) Base  settings -");

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.T:  // TETRIS
                    _customGameMode = 1;

                    break;

                case ConsoleKey.S:
                    Console.Clear();
                    OpenSettingMenu();

                    break;

                default:            // NORMAL
                    _customGameMode = 0;

                    break;
            }

            Console.Clear();
        }

        private static void OpenSettingMenu()
        {
            PrintHeader();
            Console.WriteLine("                     - (s)   Board Size   -");
            Console.WriteLine("                     - (t)   Tickspeed    - \n");
            Console.WriteLine("                     - (M)   Main menu    -");

            var choice = Console.ReadKey().Key;
            Console.Clear();
            PrintHeader();

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (choice)
            {
                case ConsoleKey.S:
                    ChangeSize();

                    break;

                case ConsoleKey.T:
                    ChangeTickSpeed();

                    break;

                default:
                    OpenGameModeMenu();

                    break;
            }
        }

        private static void ChangeTickSpeed()
        {
            Console.WriteLine(
                $"   Current tickspeed is {1000 / Board.GetMsPerTick()} t/s");
            Console.WriteLine("   Standart is 10 t/s.");
            Console.WriteLine(
                "   Enter new values or choose (s) for standard / (C) to cancel");
            Console.Write("   New tickspeed: ");
            var input = Console.ReadLine();
            Console.WriteLine();

            if (IsDigit(input, 1))
            {
                Board.SetMsPerTick(1000 / int.Parse(input!));
            }
            else if (input!.ToLower().Equals("s"))
            {
                Board.SetMsPerTick(100);
            }

            Console.Clear();
            OpenSettingMenu();
        }

        private static void ChangeSize()
        {
            var oldWidth = Board.GetWidth();
            Console.WriteLine(
                $"   Current size is {oldWidth} by {Board.GetHight()} (width / height)");
            Console.WriteLine("   Standart size is 7 by 6.");
            Console.WriteLine(
                "   Enter new values or choose (s) for standard / (C) to cancel");
            Console.Write("   New width: ");
            var inputWidth = Console.ReadLine();

            if (IsDigit(inputWidth, 1))
            {
                Board.SetWidth(int.Parse(inputWidth!));
                Console.Write("   New hight: ");
                var inputHeight = Console.ReadLine();

                if (IsDigit(inputHeight, 1))
                {
                    if (int.Parse(inputHeight!) == 1
                        || int.Parse(inputWidth) == 1)
                    {
                        Console.WriteLine(
                            "\n   Less than 2 in either dimensions will not work.");
                    }

                    if (int.Parse(inputHeight!) < 4 && int.Parse(inputWidth) < 4)
                    {
                        Console.WriteLine(
                            "\n   Less than 4 in both dimensions will not work.");
                        Console.ReadKey();
                        Console.Clear();
                        OpenSettingMenu();
                    }

                    Board.SetHight(int.Parse(inputHeight!));
                }
                else if (inputHeight!.ToLower().Equals("s"))
                {
                    Board.SetHight(6);
                }
                else
                {
                    Board.SetWidth(oldWidth);
                }
            }
            else if (inputWidth!.ToLower().Equals("s"))
            {
                Board.SetWidth(7);
                Board.SetHight(6);
            }

            Console.Clear();
            OpenSettingMenu();
        }

        private static bool IsDigit(string input, int rangeFrom = int.MinValue, int rangeTo = int.MaxValue)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (input.Any(c => !char.IsDigit(c)))
            {
                return false;
            }

            return int.Parse(input) >= rangeFrom
                   && int.Parse(input) <= rangeTo;
        }

        internal static void PrintColorSample(ConsoleColor color)
        {
            var list = new List<ConsoleColor>() { color };
            PrintColorSample(list);
        }

        internal static void PrintColorSample(List<ConsoleColor> colors)
        {
            var background = Console.BackgroundColor;
            Console.Write("|");

            foreach (var color in colors)
            {
                Console.BackgroundColor = color;
                Console.Write("   ");
                Console.BackgroundColor = background;
                Console.Write("|");
            }
        }

        internal static void PrintInCustomColor(Player p)
            => PrintInCustomColor($" {p.PlayerChar} ", p.Color);

        internal static void PrintInCustomColor(
            string customText,
            ConsoleColor customColor)
            => PrintInCustomColor(null, 0, 0, customText, customColor, null, null);

        private static void PrintInCustomColor(
            char[,] field,
            int width,
            int height,
            string customText,
            ConsoleColor customColor,
            Player playerX,
            Player playerO)
        {
            var background = Console.BackgroundColor;
            var foreground = Console.ForegroundColor;
            Console.BackgroundColor = customColor;
            Console.ForegroundColor = customColor.Equals(ConsoleColor.Black)
                ? ConsoleColor.White
                : ConsoleColor.Black;

            if (string.IsNullOrEmpty(customText))
            {
                Board.PrintCell(width, height, field, customColor, playerX, playerO);
            }
            else
            {
                Console.Write(customText);
            }

            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
        }

        private static ConsoleColor PickHighlightColor(
            ConsoleColor colorX,
            ConsoleColor colorO)
        {
            if (!colorX.Equals(ConsoleColor.Red)
                && !colorO.Equals(ConsoleColor.Red))
            {
                return ConsoleColor.Red;
            }

            if (!colorX.Equals(ConsoleColor.Blue)
                && !colorO.Equals(ConsoleColor.Blue))
            {
                return ConsoleColor.Blue;
            }

            return ConsoleColor.Green;
        }

        internal static ConsoleColor GetActivePlayerColor(
            Player playerX,
            Player playerO)
            => GetActivePlayer(playerX, playerO).Color;

        private static Player GetPlayerByChar(
            Player playerX,
            Player playerO,
            char c)
        {
            if (playerX.PlayerChar.Equals(c))
            {
                return playerX;
            }

            if (playerO.PlayerChar.Equals(c))
            {
                return playerO;
            }

            return null;
        }

        internal static ConsoleColor GetPlayerColorByChar(
            Player playerX,
            Player playerO,
            char c)
            => GetPlayerByChar(playerX, playerO, c).Equals(null)
                ? ConsoleColor.Black
                : GetPlayerByChar(playerX, playerO, c).Color;
    }
}
