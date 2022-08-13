using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite)
        {
        }

        protected override GameObject BlackModel => ObjectLoader.Instance.bQueen;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wQueen;
        public override uint Value => 9;
        protected override uint ColourlessIndex => 5;

        public override void AddMoves(Position pos, Board boardRef, LegalMoves legalMoves)
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