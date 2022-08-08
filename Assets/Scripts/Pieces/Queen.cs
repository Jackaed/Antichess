using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite) { }
        protected override GameObject BlackModel => Constants.Instance.bQueen;
        protected override GameObject WhiteModel => Constants.Instance.wQueen;
        public override uint Value => 9;
        protected override uint ColourlessIndex => 5;

        public override void AddMoves(Position pos, Board boardRef, List<Move> legalMoves)
        {
            Position[] directions =
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

            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef, legalMoves);
        }
    }
}
