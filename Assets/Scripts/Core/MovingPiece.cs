using UnityEngine;

namespace Antichess.Core
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