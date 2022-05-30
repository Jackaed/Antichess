using System.Collections.Generic;
using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public static class GenericMoveLogic
    {
        // Adds all of the moves that are achieved by going directly in a straight line, until you collide with an enemy
        // piece or one of your own pieces, or the edge of the board. 
        private static void AddMovesInDir(Position pos, Vector2Int increments, BoardLogic boardRef)
        {
            var to = new Position(pos.x, pos.y) + increments;
            while (to.x < BoardLogic.Size.x && to.y < BoardLogic.Size.y)
            {
                if (boardRef.PieceAt(to) == null)
                {
                    if (boardRef.CanTake == false)
                    {
                        boardRef.AddLegalMove(new Move(pos, to));
                    }
                }

                else if (boardRef.PieceAt(to).IsWhite != boardRef.PieceAt(pos).IsWhite)
                {
                    boardRef.CanTake = true;
                    boardRef.AddLegalMove(new Move(pos, to));
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
            IEnumerable<Vector2Int> directions, BoardLogic boardRef)
        {
            foreach(var direction in directions)
            {
                AddMovesInDir(pos, direction, boardRef);
            }
        }
        
        // Basic function that adds a move to the list of legal moves if it either takes a piece or moves into empty
        // space.
        private static void AddMoveAtPosIfLegal(Move move, BoardLogic boardRef)
        {
            if (boardRef.PieceAt(move.To) == null)
            {
                if (!boardRef.CanTake)
                {
                    boardRef.AddLegalMove(move);
                }
            }

            else if (boardRef.PieceAt(move.To).IsWhite != boardRef.PieceAt(move.From).IsWhite)
            {
                boardRef.CanTake = true;
                boardRef.AddLegalMove(move);
            }
        } 
        
        // Adds moves at offsets from piece's original position, if that move passes the above legality checks.
        public static void AddLegalMovesAtOffsets(Position pos,
            IEnumerable<Vector2Int> directions, BoardLogic boardRef)
        {
            foreach (var direction in directions)
            {
                var dir = pos + direction;
                if (dir.x >= BoardLogic.Size.x || dir.y >= BoardLogic.Size.y) continue;
                AddMoveAtPosIfLegal(new Move(pos, dir), boardRef);
            }
        }
    }
}