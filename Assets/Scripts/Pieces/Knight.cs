using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite) { }
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKnight;
        protected override GameObject BlackModel => ObjectLoader.Instance.bKnight;

        public override void AddMoves(Position pos, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
        {
            Position[] directions =
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
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, directions, boardRef, legalMoves);
        }
    }
}
