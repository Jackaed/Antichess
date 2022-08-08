using System;
using Antichess.PositionTypes;

namespace Antichess
{
    public class Move : IComparable<Move>
    {
        public readonly Position From, To;

        public Move(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public int CompareTo(Move other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            return Constants.Instance.Rand.Next(1) * 2 - 1;

//            if (From.X > other.From.X) return 1;
//
//            if (From.X < other.From.X) return -1;
//            if (From.Y > other.From.Y) return 1;
//
//            if (From.Y < other.From.Y) return -1;
//            if (To.X > other.To.X) return 1;
//
//            if (To.X < other.To.X) return -1;
//            if (To.Y > other.To.Y) return 1;
//
//            if (To.Y < other.To.Y) return -1;
//
//            return 0;
        }

        private bool Equals(Move other)
        {
            return Equals(From, other.From) && Equals(To, other.To);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var other = (Move) obj;
            return From == other.From && To == other.To;
        }

        public override string ToString()
        {
            return From + " , " + To;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }
    }
}
