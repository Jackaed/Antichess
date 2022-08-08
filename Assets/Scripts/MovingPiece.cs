using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess
{
    public struct MovingPiece
    {
        public readonly Position To;
        public readonly GameObject Piece;

        public MovingPiece(Position to, GameObject piece)
        {
            To = to;
            Piece = piece;
        }
    }
}
