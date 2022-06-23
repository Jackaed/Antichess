using System.Collections.Generic;
using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public static class GenericMoveLogic
    {
        // Adds all of the moves that are achieved by going directly in a straight line, until you collide with an enemy
        // piece or one of your own pieces, or the edge of the board. 
        private static void AddMovesInDir(Position pos, Vector2Int increments, Board boardRef,
            Dictionary<Position, List<Position>> legalMoves)
        {
            var to = new Position(pos.x, pos.y) + increments;
            while (to.x < Board.Size.x && to.y < Board.Size.y)
            {
                if (boardRef.PieceAt(to) == null)
                {
                    if (boardRef.CanTake == false) Board.AddLegalMove(new Move(pos, to), boardRef, legalMoves);
                }

                else if (boardRef.PieceAt(to).IsWhite != boardRef.PieceAt(pos).IsWhite)
                {
                    boardRef.CanTake = true;
                    Board.AddLegalMove(new Move(pos, to), boardRef, legalMoves);
                    break;
                }

                else
                {
                    break;
                }

                to += increments;
            }
        }

        // Runs AddMovesInDir, but for every direction given. Used by Rooks, Bishops and Queens.
        public static void AddLegalMovesInDirections(Position pos,
            IEnumerable<Vector2Int> directions, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
        {
            foreach (var direction in directions) AddMovesInDir(pos, direction, boardRef, legalMoves);
        }

        // Basic function that adds a move to the list of legal moves if it either takes a piece or moves into empty
        // space.
        private static void AddMoveAtPosIfLegal(Move move, Board boardRef,
            Dictionary<Position, List<Position>> legalMoves)
        {
            if (boardRef.PieceAt(move.To) == null)
            {
                if (!boardRef.CanTake) Board.AddLegalMove(move, boardRef, legalMoves);
            }

            else if (boardRef.PieceAt(move.To).IsWhite != boardRef.PieceAt(move.From).IsWhite)
            {
                boardRef.CanTake = true;
                Board.AddLegalMove(move, boardRef, legalMoves);
            }
        }

        // Adds moves at offsets from piece's original position, if that move passes the above legality checks.
        public static void AddLegalMovesAtOffsets(Position pos,
            IEnumerable<Vector2Int> directions, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
        {
            foreach (var direction in directions)
            {
                var dir = pos + direction;
                if (dir.x >= Board.Size.x || dir.y >= Board.Size.y) continue;
                AddMoveAtPosIfLegal(new Move(pos, dir), boardRef, legalMoves);
            }
        }
    }
}
