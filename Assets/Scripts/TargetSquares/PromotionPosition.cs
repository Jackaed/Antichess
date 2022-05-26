using Antichess.Pieces;

namespace Antichess.TargetSquares
{
    public class PromotionPosition : Position
    {
        public PromotionPosition (byte x, byte y) : base (x, y) {}
        
        public Piece PromotionPiece;
    }
}