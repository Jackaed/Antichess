using Antichess.Pieces;

namespace Antichess.PositionTypes
{
    public class BoardChange
    {
        public readonly Move Move;
        public readonly Piece Taken;
        public readonly Position OldEnPassantTarget;
        
        public BoardChange(Move move, Piece taken, Position oldEnPassantTarget)
        {
            Move = move;
            Taken = taken;
            OldEnPassantTarget = oldEnPassantTarget;
        }
    }
}
