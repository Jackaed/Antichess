using System.Collections.Generic;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(bool isWhite) : base(isWhite) {}
        protected override GameObject WhiteModel => ObjectLoader.Instance.wPawn;
        protected override GameObject BlackModel => ObjectLoader.Instance.bPawn;

        public override ListMove GetMoves(Vector2Int pos, Board boardRef, bool canTake)
        {
            Vector2Int ahead = pos + new Vector2Int(0, IsWhite ? 1 : -1);

            if (ahead.y > Board.Size.y || ahead.y < 0) return new ListMove();

            ListMove listMove = new ListMove();
            Vector2Int[] takesPositions = {ahead + new Vector2Int(1, 0), ahead + new Vector2Int(-1, 0)};
            foreach(var takesPosition in takesPositions)
            {
                if (takesPosition.x > 0 && takesPosition.x < Board.Size.x && boardRef.PieceAt(takesPosition) != null &&
                    boardRef.PieceAt(takesPosition).IsWhite != IsWhite)
                {
                    listMove.MoveList.Add(takesPosition);
                    canTake = true;
                }
            }
            
            if (!canTake && boardRef.PieceAt(ahead) == null)
            {
                listMove.MoveList = new List<Vector2Int> {ahead};
                Vector2Int aheadahead = ahead + new Vector2Int(0, IsWhite ? 1 : -1);
                if (pos.y == (IsWhite ? 1 : Board.Size.y - 2) && boardRef.PieceAt(aheadahead) == null)
                {
                    listMove.MoveList.Add(aheadahead);
                }
            }

            listMove.CanTake = canTake;
            return listMove;
        }
    }
}
