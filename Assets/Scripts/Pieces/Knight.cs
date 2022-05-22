using UnityEngine;

namespace Antichess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite) {}
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKnight;
        protected override GameObject BlackModel => ObjectLoader.Instance.bKnight;
        public override ListMove GetMoves(Vector2Int pos, Board boardRef, bool canTake)
        {

            Vector2Int[] directions =
            {
                new(2, 1),
                new(2, -1),
                new(1, 2),
                new(1, -2),
                new(-1, 2),
                new(-1, -2),
                new(-2, 1),
                new(-2, -1)
            };
            return GenericMoveLogic.GetMovesAtOffsets(pos, directions, boardRef, canTake);
        }
    }
}
