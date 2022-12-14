/*
 ============================================================================
 Name        : connect4.c
 Author      : Markus Haller
 Version     : 2.0
 Copyright   : Free for use
 Description : Console connect4 for 2 in C sharp
 ============================================================================
 */

// ReSharper disable CognitiveComplexity

using System;
using System.Numerics;

namespace connect4
{
    public static class Program
    {
        private static void Main()
        {
            // 7 by 6 is classic connect 4
            const int width = 7;
            const int height = 6;
            _ = new Board(width, height);
            _ = new Bot();
            var playerX = new Player();
            var playerO = new Player();

            var notExited = true;
            playerX.ChangeActiveStatus();

            Utils.OpenMenu();

            playerX.Initialize('X');
            playerO.Initialize('O');

            while (notExited)
            {
                Board.ResetField();
                Console.Clear();

                Board.PrintBoard(playerX, playerO);
                var isGameOngoing = true;

                while (isGameOngoing)
                {
                    Utils.ChangePlayer(playerX, playerO);

                    if (Utils.GetGameMode() != 1)
                    {
                        Console.WriteLine($"   Moves made: {Board.GetMovesMade()}");
                    }

                    Console.Write("   ");
                    Utils.PrintInCustomColor($" {Utils.GetActivePlayer(playerX, playerO).PlayerChar} ", Utils.GetActivePlayerColor(playerX, playerO));
                    Console.WriteLine(" to play.");
                    Utils.MakeNextMove(playerX, playerO);

                    Console.Clear();
                    Board.PrintBoard(playerX, playerO);

                    isGameOngoing = Utils.DidGameEnd(playerX, playerO);

                    if (Utils.GetGameMode() > 0
                        && isGameOngoing)
                    {
                        if (Board.IsLastRowFull())
                        {
                            Board.RemoveLastRow(playerX, playerO);
                        }

                        Console.Clear();
                        Board.PrintBoard(playerX, playerO);
                    }

                    Console.Write("   ");
                    Utils.PrintInCustomColor(playerX);
                    Console.Write(" ");
                    Utils.PrintInCustomColor(playerO);
                    Console.WriteLine();
                    Console.WriteLine($"    {playerX.Score} : {playerO.Score}");
                    Console.WriteLine();
                }

                notExited = Utils.AskIfKeepPlayingResetOrExit(playerX, playerO);
            }
        }
    }
}
