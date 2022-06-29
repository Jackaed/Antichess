using Antichess.Pieces;

namespace Antichess.PositionTypes
{
    public class BoardStateChange : Move
    {
        public readonly Piece Taken;
        public readonly Position OldEnPassantTarget;
        
        public BoardStateChange(Position from, Position to, Piece taken, Position oldEnPassantTarget) : base(from, to)
        {
            Taken = taken;
            OldEnPassantTarget = oldEnPassantTarget;
        }
    }
}
