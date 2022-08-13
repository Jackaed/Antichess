using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite)
        {
        }

        protected override GameObject WhiteModel => ObjectLoader.Instance.wKnight;
        protected override GameObject BlackModel => ObjectLoader.Instance.bKnight;
        public override uint Value => 3;
        protected override uint ColourlessIndex => 1;

        public override void AddMoves(Position pos, Board boardRef, LegalMoves legalMoves)
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