using UnityEngine;

namespace Antichess.Pieces
{
    public class Rook : Piece
    {
        public Rook(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bRook;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wRook;

        public override ListMove GetMoves(Vector2Int pos, Board boardRef, bool canTake)
        {
            Vector2Int[] directions =
            {
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            return GenericMoveLogic.GetMovesInStraightDirection(pos, directions, boardRef, canTake);
        }
    }
}