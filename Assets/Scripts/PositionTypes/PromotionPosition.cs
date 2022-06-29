using System;
using Antichess.Pieces;

namespace Antichess.PositionTypes
{
    // A simple version of a Position, which facilitates the storage of a piece for a pawn to promote to.
    public class PromotionPosition : Position
    {
        public readonly Piece PromotionPiece;

        public PromotionPosition(sbyte x, sbyte y, Piece piece) : base(x, y)
        {
            PromotionPiece = piece;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PromotionPosition;
            return other != null && base.Equals(obj) && PromotionPiece.GetType() == other.PromotionPiece.GetType();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), PromotionPiece);
        }

        public override string ToString()
        {
            return X + ", " + Y + " into" + PromotionPiece.GetType();
        }
    }
}
