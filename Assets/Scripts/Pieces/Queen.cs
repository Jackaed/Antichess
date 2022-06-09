using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite) { }
        protected override GameObject BlackModel => ObjectLoader.Instance.bQueen;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wQueen;

        public override void AddMoves(Position pos, BoardLogic boardRef)
        {
            Vector2Int[] directions =
            {
                new(1, 1),
                new(1, -1),
                new(-1, 1),
                new(-1, -1),
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef);
        }
    }
}
