using System.Collections.Generic;
using Antichess.Unity;

namespace Antichess.Core.Pieces
{
    public static class GenericMoveLogic
    {
        // Adds all of the moves that are achieved by going directly in a straight line, until you
        // collide with an enemy piece or one of your own pieces, or the edge of the board.
        private static void AddMovesInDir(
            Position pos,
            Position increments,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            var to = new Position(pos.X, pos.Y) + increments;
            while (to.X < ObjectLoader.BoardSize && to.Y < ObjectLoader.BoardSize)
            {
                if (boardRef.PieceAt(to) == null)
                {
                    if (!onlyCaptures)
                        legalMoves.Add(new Move(pos, to));
                }
                else if (boardRef.PieceAt(to).IsWhite != boardRef.PieceAt(pos).IsWhite)
                {
                    legalMoves.Add(new Move(pos, to));
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
        public static void AddLegalMovesInDirections(
            Position pos,
            IEnumerable<Position> directions,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            foreach (var direction in directions)
                AddMovesInDir(pos, direction, boardRef, legalMoves, onlyCaptures);
        }

        // Basic function that adds a move to the list of legal moves if it either takes a piece or
        // moves into empty space.
        private static void AddMoveAtPosIfLegal(
            Move move,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            if (boardRef.PieceAt(move.To) == null)
            {
                if (!onlyCaptures)
                    legalMoves.Add(move);
            }
            else if (boardRef.PieceAt(move.To).IsWhite != boardRef.PieceAt(move.From).IsWhite)
            {
                legalMoves.Add(move);
            }
        }

        // Adds moves at offsets from piece's original position, if that move passes the above
        // legality checks.
        public static void AddLegalMovesAtOffsets(
            Position pos,
            IEnumerable<Position> directions,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            foreach (var direction in directions)
            {
                var dir = pos + direction;
                if (dir.X >= ObjectLoader.BoardSize || dir.Y >= ObjectLoader.BoardSize)
                    continue;
                AddMoveAtPosIfLegal(new Move(pos, dir), boardRef, legalMoves, onlyCaptures);
            }
        }
    }
}
