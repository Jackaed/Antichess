using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bKing;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKing;

        public override void AddMoves(Position pos, BoardLogic boardRef)
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
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, directions, boardRef);
        }
    }
}