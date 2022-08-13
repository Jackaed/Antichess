using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(bool isWhite) : base(isWhite)
        {
        }

        protected override GameObject BlackModel => ObjectLoader.Instance.bBishop;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wBishop;
        public override uint Value => 3;

        protected override uint ColourlessIndex => 2;

        public override void AddMoves(Position pos, Board boardRef, LegalMoves legalMoves)
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