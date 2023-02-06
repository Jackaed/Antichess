using System;
using Antichess.Core.Pieces;

namespace Antichess.Core
{
    /// <summary>
    /// An override of the move class which represents a pawn promotion move.
    /// </summary>
    public class Promotion : Move
    {
        public readonly Piece PromotionPiece;

        public Promotion(Position from, Position to, Piece promotionPiece) : base(from, to)
        {
            PromotionPiece = promotionPiece;
        }

        private bool Equals(Promotion other)
        {
            return base.Equals(other) && Equals(PromotionPiece, other.PromotionPiece);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj.GetType() == GetType() && Equals((Promotion)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), PromotionPiece);
        }

        public static bool operator ==(Promotion left, Promotion right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Promotion left, Promotion right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return base.ToString() + " into " + PromotionPiece;
        }
    }
}
