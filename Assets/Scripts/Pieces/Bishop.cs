using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(bool isWhite) : base(isWhite) { }
        protected override GameObject BlackModel => Constants.Instance.bBishop;
        protected override GameObject WhiteModel => Constants.Instance.wBishop;
        public override uint Value => 3;

        protected override uint ColourlessIndex => 2;

        public override void AddMoves(Position pos, Board boardRef, List<Move> legalMoves)
        {
            Position[] directions =
            {
                new(1, 1),
                new(1, -1),
                new(-1, 1),
                new(-1, -1)
            };
            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef, legalMoves);
        }
    }
}
