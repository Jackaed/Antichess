using Antichess.Core.Pieces;

namespace Antichess.Core
{
    /// <summary>
    /// Used to store all data required to undo a move that has been made, such as the piece that
    /// was taken in the given move, the move itself, what the halfmove clock was set to, etc.
    /// </summary>
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
