using Antichess.Core.Pieces;

namespace Antichess.Core
{
    // This class is used for storing all the data that is required to "undo" a move on a chessboard.
    public class BoardChange
    {
        public readonly ushort LastIrreversibleMove;
        public readonly Move Move;
        public readonly Position OldEnPassantTarget;
        public readonly Piece Taken;

        public BoardChange(Move move, Piece taken, Position oldEnPassantTarget, ushort lastIrreversibleMove)
        {
            Move = move;
            Taken = taken;
            OldEnPassantTarget = oldEnPassantTarget;
            LastIrreversibleMove = lastIrreversibleMove;
        }
    }
}