using System.Collections.Generic;
using UnityEngine;

namespace Antichess.Pieces
{
    public static class GenericMoveLogic
    {
        private static ListMove MovesInDir(Vector2Int pos, Vector2Int increments, Board boardRef, bool canTake)
        {
            ListMove moveList = new();
            var to = new Vector2Int(pos.x, pos.y) + increments;
            while (to.x < Board.Size.x && to.x >= 0 && to.y < Board.Size.y && to.y >= 0)
            {
                if (boardRef.PieceAt(to) == null)
                {
                    if (!canTake) moveList.MoveList.Add(to);
                }

                else if (boardRef.PieceAt(to).IsWhite != boardRef.PieceAt(pos).IsWhite)
                {
                    moveList.MoveList = new List<Vector2Int> {to};
                    canTake = true;
                    break;
                }

                else
                {
                    break;
                }

                to += increments;
            }

            moveList.CanTake = canTake;
            return moveList;
        }

        public static ListMove GetMovesInStraightDirection(Vector2Int pos,
            IEnumerable<Vector2Int> directions, Board boardRef, bool canTake)
        {
            ListMove moveList = new();

            var couldTake = false;
            foreach (var direction in directions)
            {
                var dirList = MovesInDir(pos, direction, boardRef, canTake);
                canTake = dirList.CanTake;
                if (couldTake != canTake)
                {
                    moveList.MoveList = new List<Vector2Int>();
                    couldTake = true;
                }

                moveList.MoveList.AddRange(dirList.MoveList);
                moveList.CanTake = dirList.CanTake;
            }

            return moveList;
        }

        public static ListMove GetMovesAtOffsets(Vector2Int pos,
            IEnumerable<Vector2Int> directions, Board boardRef, bool canTake)
        {
            ListMove listMove = new();
            var couldTake = false;
            foreach (var direction in directions)
            {
                var dir = pos + direction;
                if (dir.x < 0 || dir.x >= Board.Size.x || dir.y < 0 || dir.y >= Board.Size.y) continue;
                if (boardRef.PieceAt(dir) == null)
                {
                    if (!canTake) listMove.MoveList.Add(dir);
                }

                else if (boardRef.PieceAt(dir).IsWhite != boardRef.PieceAt(pos).IsWhite)
                {
                    canTake = true;

                    if (!couldTake)
                    {
                        listMove.MoveList = new List<Vector2Int>();
                        couldTake = true;
                    }

                    listMove.MoveList.Add(dir);
                }

                listMove.CanTake = canTake;
            }

            return listMove;
        }
    }
}