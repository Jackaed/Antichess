using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Rook : Piece
    {
        public Rook(bool isWhite) : base(isWhite)
        {
        }

        protected override GameObject BlackModel => ObjectLoader.Instance.bRook;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wRook;
        public override uint Value => 5;
        protected override uint ColourlessIndex => 3;

        public override void AddMoves(Position pos, Board boardRef, LegalMoves legalMoves)
        {
            Position[] directions =
            {
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef, legalMoves);
        }
    }
}