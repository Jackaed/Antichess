using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite) { }
        protected override GameObject BlackModel => Constants.Instance.bKing;
        protected override GameObject WhiteModel => Constants.Instance.wKing;
        public override uint Value => 2;

        public override void AddMoves(Position pos, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
        {
            // The offsets of a King's potential movement options, from his current position "pos"
            Position[] offsets =
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
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, offsets, boardRef, legalMoves);
        }
    }
}
