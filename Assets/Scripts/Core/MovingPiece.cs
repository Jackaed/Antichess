using UnityEngine;

namespace Antichess.Core
{
    /// <summary>
    /// To allow pieces to move gradually over time rather than instantaneously, this struct allows
    /// the board to store pieces that are currently moving between two locations on the chessboard.
    /// </summary>
    public readonly struct MovingPiece
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
