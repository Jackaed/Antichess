using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Rook : Piece
    {
        public Rook(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bRook;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wRook;

        public override void AddMoves(Position pos, Board boardRef)
        {
            Vector2Int[] directions =
            {
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef);
        }
    }
}