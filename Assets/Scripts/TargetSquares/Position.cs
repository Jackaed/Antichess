using System;
using UnityEngine;

namespace Antichess.TargetSquares
{
    public class Position
    {

        // ReSharper disable InconsistentNaming
        public readonly byte x, y;
        // ReSharper restore InconsistentNaming
        
        public Position(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position((byte) (a.x + b.x), (byte)(a.y + b.y));
        }
        
        public static Position operator +(Position a, Vector2Int b)
        {
            return new Position((byte) (a.x + b.x), (byte)(a.y + b.y));
        }
        
        public static Position operator -(Position a, Vector2Int b)
        {
            return new Position((byte) (a.x - b.x), (byte)(a.y - b.y));
        }


        private bool Equals(Position other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((Position) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}