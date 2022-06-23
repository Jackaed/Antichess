using System;

namespace Antichess.PositionTypes
{
    public class PawnDoubleMovePosition : Position
    {
        public readonly Position MovedThrough;

        public PawnDoubleMovePosition(Position to, Position movedThrough) : base(to.X, to.Y)
        {
            MovedThrough = movedThrough;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + " through " + MovedThrough + ")";
        }

        public override bool Equals(object obj)
        {
            var other = obj as PawnDoubleMovePosition;
            return other != null && base.Equals(obj) && MovedThrough == other.MovedThrough;
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
