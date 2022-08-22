using UnityEngine;

namespace Antichess.Core
{
    public struct MovingPiece
    {
        public readonly Position To;
        public readonly GameObject Piece;
        public readonly float Distance;

        public MovingPiece(Position to, GameObject piece, float distance)
        {
            To = to;
            Piece = piece;
            Distance = distance;
        }
    }
}