using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(bool isWhite) : base(isWhite) { }
        protected override GameObject BlackModel => ObjectLoader.Instance.bBishop;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wBishop;

        public override void AddMoves(Position pos, BoardLogic boardRef)
        {
            Vector2Int[] directions =
            {
                new(1, 1),
                new(1, -1),
                new(-1, 1),
                new(-1, -1)
            };
            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef);
        }
    }
}
