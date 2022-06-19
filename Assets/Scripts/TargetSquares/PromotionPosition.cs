using System;
using Antichess.Pieces;
using Unity.VisualScripting;

namespace Antichess.TargetSquares
{
    public class PromotionPosition : Position
    {
        public readonly Piece PromotionPiece;

        public PromotionPosition(byte x, byte y, Piece piece) : base(x, y)
        {
            PromotionPiece = piece;
        }

        bool Equals(PromotionPosition other)
        {
            return base.Equals(other) && PromotionPiece.GetType() == other.PromotionPiece.GetType();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((PromotionPosition) obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return x + ", " + y + " into" + PromotionPiece.GetType().ToString();
        }
    }
}
