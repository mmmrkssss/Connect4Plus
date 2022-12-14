using System;
using System.Threading;

namespace connect4
{
    public sealed class Board
    {
        private static int _width;
        private static int _height;
        private static char[,] _gameBoard;
        private static int _movesMade;
        private static int _lastColumnPlayed;
        private static int _msPerTick = 100;

        public Board(int width, int height)
        {
            _width = width;
            _height = height;
            _gameBoard = new char[_width, _height];
        }

        private static void resizeBoard()
            => _gameBoard = new char[_width, _height];

        internal static int GetWidth()
            => _width;

        internal static void SetWidth(int newWidth)
        {
            _width = newWidth;
            resizeBoard();
        }

        internal static int GetHight()
            => _height;

        internal static void SetHight(int newHight)
        {
            _height = newHight;
            resizeBoard();
        }

        internal static int GetMsPerTick()
            => _msPerTick;

        internal static int SetMsPerTick(int newMsPerTick)
            => _msPerTick = newMsPerTick;

        internal static int GetMovesMade()
            => _movesMade;

        internal static int GetLastColumnPlayed()
            => _lastColumnPlayed;

        internal static char[,] GetBoard()
            => _gameBoard;

        internal static void PrintBoard(
        Player playerX,
            Player playerO,
            int specialPrint = -1
        )
        {
            Console.WriteLine();
            Console.Write("   ");

            for (var i = 1; i <= _width; i++)
            {
                Console.Write(i < 10 ? $"  {i} " : $"  {i}");
            }

            Console.WriteLine();
            Console.Write("   ");

            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    Console.Write("|");
                    Utils.PrintCharColoredIfWinning(j, i, _gameBoard, playerX, playerO);
                }

                Console.WriteLine("|");
                Console.Write("   ");
            }

            Console.Write("¯");

            for (var i = 0; i < _width; i++)
            {
                Console.Write("¯¯¯¯");
            }

            Console.WriteLine();
        }

        internal static void PrintCell(
            int widthIndex,
            int heightIndex,
            char[,] board,
            ConsoleColor customColor,
        Player playerX,
            Player playerO)
        {
            if (customColor != ConsoleColor.Black)
            {
                Utils.PrintInCustomColor($" {board[widthIndex, heightIndex].ToString().ToUpper()} ", customColor);
            }
            else if (board[widthIndex, heightIndex] == 'X'
                     || board[widthIndex, heightIndex] == 'O')
            {
                Utils.PrintInCustomColor($" {board[widthIndex, heightIndex].ToString().ToUpper()} ", Utils.GetPlayerColorByChar(playerX, playerO, board[widthIndex, heightIndex]));
            }
            else
            {
                Console.Write($" {board[widthIndex, heightIndex]} ");
            }
        }

        internal static void ResetField()
        {
            _movesMade = 0;

            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    _gameBoard[j, i] = ' ';
                }
            }
        }

        internal static bool HasColumnSpace(
            int column)
            => _gameBoard[column - 1, 0] == ' ';

        internal static void ManualPlaceInColumn(
        int column,
            Player player)
            => _gameBoard[column - 1, GetLowestUnoccupiedCellIndex(column)] = player.PlayerChar;

        internal static void RemoveHighestPieceInColumn(
            int column)
            => _gameBoard[column - 1, GetLowestUnoccupiedCellIndex(column) + 1] = ' ';

        internal static void PlaceInColumn(
        int column,
            Player playerX,
            Player playerO)
        {
            AnimatedPlacing(column, playerX, playerO);
            _gameBoard[column - 1, GetLowestUnoccupiedCellIndex(column)] = Utils.GetActivePlayer(playerX, playerO).PlayerChar;
            _movesMade++;
            _lastColumnPlayed = column;
        }

        internal static int GetLowestUnoccupiedCellIndex(int column)
        {
            for (var i = _height - 1; i >= 0; i--)
            {
                if (_gameBoard[column - 1, i] == ' ')
                {
                    return i;
                }
            }

            return -1;
        }

        internal static bool IsCellPlayable(int widthIndex, int heightIndex)
        {
            if (!_gameBoard[widthIndex, heightIndex].Equals(' '))
            {
                return false;
            }

            if (heightIndex == Board._height - 1)
            {
                return true;
            }

            return !_gameBoard[widthIndex, heightIndex + 1].Equals(' ');
        }

        internal static char GetCharFromBoard(int columnIndex, int rowIndex)
            => (char)_gameBoard.GetValue(columnIndex, rowIndex)!;

        private static void AnimatedPlacing(int column, Player playerX, Player playerO)
        {
            for (var i = 0; _gameBoard[column - 1, i] == ' ' && i < _height - 1; i++)
            {
                Console.Clear();
                _gameBoard[column - 1, i] = Utils.GetActivePlayer(playerX, playerO).PlayerChar;
                PrintBoard(playerX, playerO);
                Thread.Sleep(_msPerTick * 2);
                Console.Clear();
                _gameBoard[column - 1, i] = ' ';
            }
        }

        private static void AnimatedRemoveRow(Player playerX, Player playerO)
        {
            _movesMade -= _width;
            Console.Clear();

            for (var i = 0; i <= _width; i += 2)
            {
                Utils.MakeCenterCellsLower(i);
                PrintBoard(playerX, playerO);
                Thread.Sleep(_msPerTick);
                Console.Clear();
            }

            for (var i = 0; i < _width; i++)
            {
                _gameBoard[i, _height - 1] = ' ';
            }

            PrintBoard(playerX, playerO);
            Thread.Sleep(_msPerTick * 2);
            Console.Clear();

            for (var j = _height - 1; j >= NumberOfEmptyRows(); j--)
            {
                for (var i = 0; i < _width; i++)
                {
                    _gameBoard[i, j] = _gameBoard[i, j - 1];
                    _gameBoard[i, j - 1] = ' ';
                }

                PrintBoard(playerX, playerO);
                Thread.Sleep(_msPerTick * 2);
                Console.Clear();
            }

            for (var i = 0; i < _width; i++)
            {
                _gameBoard[i, 0] = ' ';
            }
        }
        internal static void RemoveLastRow(Player playerX, Player playerO)
        => AnimatedRemoveRow(playerX, playerO);

        private static int NumberOfEmptyRows()
        {
            var result = 0;

            for (var j = 0; j < _height; j++)
            {
                var currentRowEmptySoFar = true;

                for (var i = 0; i < _width; i++)
                {
                    if (_gameBoard[i, j] != ' ')
                    {
                        currentRowEmptySoFar = false;
                    }
                }

                if (currentRowEmptySoFar)
                {
                    result++;
                }
            }

            return result;
        }

        internal static bool IsLastRowFull()
        {
            for (var i = 0; i < _width; i++)
            {
                if (_gameBoard[i, _height - 1] == ' ')
                {
                    return false;
                }
            }

            return true;
        }
    }
}
