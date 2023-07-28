using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace Hexapawn_2Player
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Hexapawn!");
            HexapawnGame game = new HexapawnGame();

            // lets play for 12 times
            for (int i = 0; i < 12; i++)
            {
                while (!game.IsGameOver)
                {
                    game.DisplayBoard();
                    game.MakeMove();
                }

                game.DisplayBoard(); // show board at end
                game.ResetGame(); // reset board etc.
                Console.ReadLine(); // wait for user to go again..
            }
        }
    }


    public class Move
    {
        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }

        public Move(int fromRow, int fromCol, int toRow, int toCol)
        {
            FromRow = fromRow;
            FromCol = fromCol;
            ToRow = toRow;
            ToCol = toCol;
        }

        public override string ToString()
        {
            string ColumnLabels = "ABCDEF";

            return String.Format("{0}{1}->{2}{3}", ColumnLabels[FromCol], FromRow + 1, ColumnLabels[ToCol], ToRow + 1);
        }
    }

    class HexapawnGame
    {
        bool AIOn = false;  // use AI or just 2 player game
        bool printValidMoves = true;  // print valid moves at each step
        static int DimensionN = 4; // dimensions of Hexapawn, default=3 max 6

        private char[,] board;
        private bool isPlayer1Turn;
        public bool IsGameOver { get; private set; }
        public MatchboxAI mbAI = new MatchboxAI(); // initialize AI

        public HexapawnGame()
        {
            board = new char[DimensionN, DimensionN];
            ResetGame();
        }

        public void ResetGame()
        {
            InitializeBoard();
            isPlayer1Turn = true;
            IsGameOver = false;
        }
        public char GetPositionValue(int row, int col)
        {
            return board[row, col];
        }

        public char[,] GetBoard()
        {
            return board;
        }

        private void InitializeBoard()
        {
            // Player 1's pawns are represented by 'P', Player 2's pawns are represented by 'C', empty squares are represented by '_'.
            for (int i = 0; i < DimensionN; i++)
            {
                board[0, i] = 'P';
                board[DimensionN-1, i] = 'C';
                for (int j = 1;j < DimensionN-1; j++)
                {
                    board[j, i] = '_';
                }
            }
        }

        public void DisplayBoard()
        {
            Console.WriteLine("   A  B  C  D  E  F".Substring(0,DimensionN*3+1));
            for (int row = 0; row < DimensionN; row++)
            {
                Console.Write(row + 1 + " ");
                for (int col = 0; col < DimensionN; col++)
                {
                    Console.Write($" {board[row, col]} ");
                }
                Console.WriteLine();
            }
        }

        public void MakeMove()
        {
            char playerPawn = isPlayer1Turn ? 'P' : 'C';

            Console.WriteLine(isPlayer1Turn ? "Player 1 'P's Turn" : "Player 2 'C's Turn");
            if (printValidMoves) { PrintValidMoves(playerPawn); }

            string fromPosition;
            string toPosition;

            Move move = new Move(0, 0, 0, 0);

            if (!AIOn || isPlayer1Turn)
            {
                Console.Write("Enter the position of the pawn you want to move (e.g., B2): ");
                fromPosition = Console.ReadLine().Trim().ToUpper();

                Console.Write("Enter the target position (e.g., B3): ");
                toPosition = Console.ReadLine().Trim().ToUpper();

                move.FromRow = int.Parse(fromPosition[1].ToString()) - 1;
                move.FromCol = fromPosition[0] - 'A';
                move.ToRow = int.Parse(toPosition[1].ToString()) - 1;
                move.ToCol = toPosition[0] - 'A';
            }
            else
            {
                // insert AI move code here
                // move = mbAI.MakeMatchboxAIMove(this, playerPawn);
            }

            if (IsValidMove(playerPawn, move))
            {
                board[move.ToRow, move.ToCol] = board[move.FromRow, move.FromCol];
                board[move.FromRow, move.FromCol] = '_';
                isPlayer1Turn = !isPlayer1Turn;

                CheckGameStatus();
            }
            else
            {
                WriteColorLine(ConsoleColor.Red, "Invalid move! Try again.");
            }
        }

        public void PrintValidMoves(char playerPawn)
        {
            Console.WriteLine("Possible Moves");
            foreach (Move move in GetValidMoves(playerPawn))
            {
                Console.WriteLine(move.ToString());
            }
        }

        public List<Move> GetValidMoves(char playerPawn)
        {
            List<Move> moves = new List<Move>();
            int vdirection = playerPawn=='P' ? 1 : -1; // if P then positive/down, else (C) negative/up
            for (int row = 0; row < DimensionN; row++)
            {
                for (int col = 0; col < DimensionN; col++)
                {
                    if (board[row, col] == playerPawn)
                    {
                        for (int leftRight = -1; leftRight < 2; leftRight++)
                        {
                            Move tempMove = new Move(row, col, row + vdirection, col + leftRight);
                            if (IsValidMove(playerPawn, tempMove))
                            {
                                moves.Add(tempMove);
                            }
                        }
                    }
                }
            }
            return moves;
        }

        private bool IsValidMove(char playerPawn, Move move)
        {
            if (move.ToCol < 0 || move.ToCol > DimensionN-1 || move.ToRow<0 || move.ToRow> DimensionN-1) { return false; } // if outside board its not a vaild move!

            if (playerPawn == 'P')
            {
                // Player 1 can move one step forward or capture diagonally forward.
                if ((move.ToRow == move.FromRow + 1 && move.ToCol == move.FromCol && board[move.ToRow, move.ToCol] == '_') ||
                    (move.ToRow == move.FromRow + 1 && Math.Abs(move.ToCol - move.FromCol) == 1 && board[move.ToRow, move.ToCol] == 'C'))
                {
                    return true;
                }
            }
            else if (playerPawn == 'C')
            {
                // Player 2 can move one step backward or capture diagonally backward.
                if ((move.ToRow == move.FromRow - 1 && move.ToCol == move.FromCol && board[move.ToRow, move.ToCol] == '_') ||
                    (move.ToRow == move.FromRow - 1 && Math.Abs(move.ToCol - move.FromCol) == 1 && board[move.ToRow, move.ToCol] == 'P'))
                {
                    return true;
                }
            }

            // Invalid move.
            return false;
        }

        private int NumberOfPawnsAlive(char pawn)
        {
            int count = 0;
            for (int row = 0; row < DimensionN; row++)
            {
                for (int col = 0; col < DimensionN; col++)
                {
                    if (board[row, col] == pawn) { count++; }
                }
            }
            return count;
        }

        private void CheckGameStatus()
        {
            bool player1Win = false;
            bool player2Win = false;

            // Check if any pawn of player 1 reached the last row (opponent's side).
            for (int col = 0; col < DimensionN; col++)
            {
                if (board[DimensionN-1, col] == 'P')
                {
                    player1Win = true;
                    break;
                }
            }

            // Check if any pawn of player 2 reached the first row (opponent's side).
            for (int col = 0; col < DimensionN; col++)
            {
                if (board[0, col] == 'C')
                {
                    player2Win = true;
                    break;
                }
            }

            char currentPlayerPawn = isPlayer1Turn ? 'P' : 'C';
            // check if player 1 is out of pawns
            if (NumberOfPawnsAlive('P') == 0 || (isPlayer1Turn && GetValidMoves('P').Count == 0))
            {
                player2Win = true;
            }
            else if (NumberOfPawnsAlive('C') == 0 || (!isPlayer1Turn && GetValidMoves('C').Count==0))
            {
                player1Win = true;
            }

            // Set the game over status.
            if (player1Win)
            {
                IsGameOver = true;
                WriteColorLine(ConsoleColor.Yellow, "Player 1 wins!");
            }
            else if (player2Win)
            {
                IsGameOver = true;
                WriteColorLine(ConsoleColor.Yellow, "Player 2 wins!");
            }
        }

        public void WriteColorLine(ConsoleColor color, string text)
        {
            ConsoleColor oldColor = System.Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }

        public List<Move> GetValidMovesForPlayer(char playerPawn)
        {
            List<Move> validMoves = new List<Move>();

            for (int fromRow = 0; fromRow < DimensionN; fromRow++)
            {
                for (int fromCol = 0; fromCol < DimensionN; fromCol++)
                {
                    if (board[fromRow, fromCol] == playerPawn)
                    {
                        for (int toRow = 0; toRow < DimensionN; toRow++)
                        {
                            for (int toCol = 0; toCol < DimensionN; toCol++)
                            {
                                if (IsValidMove(playerPawn, new Move(fromRow, fromCol, toRow, toCol)))
                                {
                                    validMoves.Add(new Move(fromRow, fromCol, toRow, toCol));
                                }
                            }
                        }
                    }
                }
            }
            return validMoves;
        }
    }

    // MatchboxAI Start
    class MatchboxAI
    {
        public MatchboxAI()
        {
            // constructor
        }
    }
    // MatchboxAI End

}





