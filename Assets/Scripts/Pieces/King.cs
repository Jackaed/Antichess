using UnityEngine;

namespace Antichess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bKing;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKing;

        public override ListMove GetMoves(Vector2Int pos, Board boardRef, bool canTake)
        {
            Vector2Int[] directions =
            {
                new(0, 1),
                new(1, 1),
                new(1, 0),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 0),
                new(-1, 1)
            };
            return GenericMoveLogic.GetMovesAtOffsets(pos, directions, boardRef, canTake);
        }
    }
}