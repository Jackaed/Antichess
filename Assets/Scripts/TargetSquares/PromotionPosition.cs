using Antichess.Pieces;

namespace Antichess.TargetSquares
{
    public class PromotionPosition : Position
    {
        public readonly Piece PromotionPiece;

        public PromotionPosition(byte x, byte y, Piece piece) : base(x, y)
        {
            PromotionPiece = piece;
        }

        private bool Equals(PromotionPosition other)
        {
            return base.Equals(other) && PromotionPiece.GetType() == other.PromotionPiece.GetType();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((PromotionPosition) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return x + ", " + y + " into" + PromotionPiece.GetType();
        }
    }
}
