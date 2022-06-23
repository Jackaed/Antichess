using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite) { }
        protected override GameObject BlackModel => ObjectLoader.Instance.bKing;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKing;

        public override void AddMoves(Position pos, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
        {
            Position[] directions =
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
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, directions, boardRef, legalMoves);
        }
    }
}
