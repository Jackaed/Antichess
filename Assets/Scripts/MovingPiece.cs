using UnityEngine;
using Antichess.TargetSquares;

namespace Antichess
{
    public struct MovingPiece
    {
        public Position To;
        public readonly GameObject Piece;

        public MovingPiece(Position to, GameObject piece)
        {
            To = to;
            Piece = piece;
        }
    }
}