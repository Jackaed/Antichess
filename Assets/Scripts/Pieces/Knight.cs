using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite) {}
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKnight;
        protected override GameObject BlackModel => ObjectLoader.Instance.bKnight;

        public override void AddMoves(Position pos, BoardLogic boardRef)
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
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, directions, boardRef);
        }
    }
}