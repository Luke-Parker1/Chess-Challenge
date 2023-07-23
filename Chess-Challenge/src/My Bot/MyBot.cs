using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();

        // Tables for where the pieces should be         
        int[,] knightTable =
        {
            {-50, -40, -30, -30, -30, -30, -40, -50},
            {-40, -20, 0, 0, 0, 0, -20, -40},
            {-30, 0, 10, 15, 15, 10, 0, -30},
            {-30, 5, 15, 20, 20, 15, 5, -30},
            {-30, 5, 15, 20, 20, 15, 5, -30},
            {-30, 0, 10, 15, 15, 10, 0, -30},
            {-40, -20, 0, 0, 0, 0, -20, -40},
            {-50, -40, -30, -30, -30, -30, -40, -50}
        };
        
        int[,] bishopTable =
        {
            {-20, -10, -10, -10, -10, -10, -10, -20},
            {-10, 5, 0, 0, 0, 0, 5, -10},
            {-10, 10, 10, 10, 10, 10, 10, -10},
            {-10, 0, 10, 10, 10, 10, 0, -10},
            {-10, 5, 5, 10, 10, 5, 5, -10},
            {-10, 0, 5, 10, 10, 5, 0, -10},
            {-10, 0, 0, 0, 0, 0, 0, -10},
            {-20, -10, -10, -10, -10, -10, -10, -20}
        };

        int[,] pawnTable = 
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {5, 10, 10, -20, -20, 10, 10, 5},
            {5, -5, -10, 0, 0, -10, -5, 5},
            {0, 0, 0, 20, 20, 0, 0, 0},
            {5, 5, 10, 25, 25, 10, 5, 5},
            {10, 10, 20, 30, 30, 20, 10, 10},
            {50, 50, 50, 50, 50, 50, 50, 50},
            {75, 75, 75, 75, 75, 75, 75, 75}
        };

        int[,] rookTable = 
        {
            {0, 0, 0, 5, 5, 0, 0, 0},
            {-5, 0, 0, 0, 0, 0, 0, -5},
            {-5, 0, 0, 0, 0, 0, 0, -5},
            {-5, 0, 0, 0, 0, 0, 0, -5},
            {-5, 0, 0, 0, 0, 0, 0, -5},
            {-5, 0, 0, 0, 0, 0, 0, -5},
            {5, 10, 10, 10, 10, 10, 10, 5},
            {0, 0, 0, 0, 0, 0, 0, 0}
        };
        
        // Find the best move with piece square tables and captures. If multiple moves have the same value, a random move is chosen
        Random rng = new();
        List<Move> bestMove = new List<Move>();
        int highestValueMove = -50;

        foreach (Move move in moves)
        {
            // Always play checkmate in one
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }

            // Find highest value capture
            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            int moveValue = pieceValues[(int)capturedPiece.PieceType];

            if (Convert.ToInt16(move.MovePieceType) == 1)
            {
                moveValue += pawnTable[move.TargetSquare.File, move.TargetSquare.Rank];
            }
            else if(Convert.ToInt16(move.MovePieceType) == 2)
            {
                moveValue += knightTable[move.TargetSquare.File, move.TargetSquare.Rank];
            }
            else if(Convert.ToInt16(move.MovePieceType) == 3)
            {
                moveValue += bishopTable[move.TargetSquare.File, move.TargetSquare.Rank];
            }
            else if(Convert.ToInt16(move.MovePieceType) == 4)
            {
                moveValue += rookTable[move.TargetSquare.File, move.TargetSquare.Rank];
            }

            if (moveValue == highestValueMove)
            {
                bestMove.Add(move);
            }
            else if (moveValue > highestValueMove)
            {
                bestMove.Clear();
                bestMove.Add(move);
                highestValueMove = moveValue;
            }
        }
        // int result = rng.Next(bestMove.Count);
        // Console.WriteLine(highestValueMove);
        // Console.WriteLine(bestMove.Count);
        // Console.WriteLine(result);
        return bestMove[rng.Next(bestMove.Count)];
    }

    // Test if this move gives checkmate
    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }

}