﻿using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        
        // Order the moves by finding the value of a capture and subtracting the value of the opponents best possible capture after that move
        // If multiple moves have the same value, one is chosen at random
        Random rng = new();
        List<Move> bestMove = new List<Move>();
        int highestMoveValue = -23500;

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

            // Check if a pawn is being promoted, but not to queen
            if (move.IsPromotion && Convert.ToInt16(move.PromotionPieceType) != 5)
            {
                moveValue -= 7500;
            }

            // Check if move is a draw
            board.MakeMove(move);
            if (board.IsDraw())
            {
                moveValue -= 8000;
            }
            board.UndoMove(move);

            moveValue -= GetOpponentMoveValue(board, move);

            if (moveValue == highestMoveValue)
            {
                bestMove.Add(move);
            }
            else if (moveValue > highestMoveValue)
            {
                bestMove.Clear();
                bestMove.Add(move);
                highestMoveValue = moveValue;
            }
        }
        
        return bestMove[rng.Next(bestMove.Count)];
    }

    int GetOpponentMoveValue(Board board, Move currentMove)
    {
        board.MakeMove(currentMove);
        Move[] moves = board.GetLegalMoves();
        int highestMoveValue = 0;

        foreach(Move move in moves)
        {
            if (MoveIsCheckmate(board, move))
            {
                board.UndoMove(currentMove);
                return 10000;
            }

            // Find highest value capture
            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            int moveValue = pieceValues[(int)capturedPiece.PieceType];

            if (moveValue > highestMoveValue)
            {
                highestMoveValue = moveValue;
            }

        }

        board.UndoMove(currentMove);

        return highestMoveValue;
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