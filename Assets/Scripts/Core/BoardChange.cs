using Antichess.Core.Pieces;

namespace Antichess.Core
{
    // This class is used for storing all the data that is required to "undo" a move on a
    // chessboard.
    public class BoardChange
    {
        public readonly ushort HalfMoveClock;
        public readonly Move Move;
        public readonly Position OldEnPassantTarget;
        public readonly Piece Taken;

        public BoardChange(
            Move move,
            Piece taken,
            Position oldEnPassantTarget,
            ushort halfMoveClock
        )
        {
            Move = move;
            Taken = taken;
            OldEnPassantTarget = oldEnPassantTarget;
            HalfMoveClock = halfMoveClock;
        }
    }
}
