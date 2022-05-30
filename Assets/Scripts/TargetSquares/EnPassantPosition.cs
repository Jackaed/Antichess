using System;

namespace Antichess.TargetSquares
{
    public class EnPassantPosition : Position
    {
        public EnPassantPosition(Position to, Position targetPieceSquare) : base(to.x, to.y)
        {
            TargetPieceSquare = targetPieceSquare;
        }
        
        public readonly Position TargetPieceSquare;

        private bool Equals(EnPassantPosition other)
        {
            return base.Equals(other) && Equals(TargetPieceSquare, other.TargetPieceSquare);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EnPassantPosition) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), TargetPieceSquare);
        }

        public static bool operator ==(EnPassantPosition left, EnPassantPosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EnPassantPosition left, EnPassantPosition right)
        {
            return !Equals(left, right);
        }
    }
}