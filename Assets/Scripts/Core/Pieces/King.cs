using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite)
        {
        }

        protected override GameObject BlackModel => ObjectLoader.Instance.bKing;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKing;
        public override uint Value => 2;
        protected override uint ColourlessIndex => 4;

        public override void AddMoves(Position pos, Board boardRef, LegalMoves legalMoves)
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