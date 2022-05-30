using System;

namespace Antichess.TargetSquares
{
    public class PawnDoubleMovePosition : Position
    {
        public readonly Position MovedThrough;
        
        public PawnDoubleMovePosition(Position to, Position movedThrough) : base(to.x, to.y)
        {
            MovedThrough = movedThrough;
        }
        
        public override string ToString()
        {
            return "(" + x + ", " + y + " through " + MovedThrough + ")";
        }

        private bool Equals(PawnDoubleMovePosition other)
        {
            return base.Equals(other) && Equals(MovedThrough, other.MovedThrough);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PawnDoubleMovePosition) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), MovedThrough);
        }

        public static bool operator ==(PawnDoubleMovePosition left, PawnDoubleMovePosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PawnDoubleMovePosition left, PawnDoubleMovePosition right)
        {
            return !Equals(left, right);
        }
    }
}