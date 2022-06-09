using Antichess.Pieces;

namespace Antichess.TargetSquares
{
    public class PromotionPosition : Position
    {
        public Piece PromotionPiece;
        public PromotionPosition(byte x, byte y) : base(x, y) { }
    }
}
