using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite) { }
        protected override GameObject WhiteModel => Constants.Instance.wKnight;
        protected override GameObject BlackModel => Constants.Instance.bKnight;
        public override uint Value => 3;
        protected override uint ColourlessIndex => 1;

        public override void AddMoves(Position pos, Board boardRef, List<Move> legalMoves)
        {
            Position[] offsets =
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
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, offsets, boardRef, legalMoves);
        }
    }
}
