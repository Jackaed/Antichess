using System;

namespace Antichess.PositionTypes
{
    public class Position
    {
        public static readonly Position Up = new(0, 1);
        public static readonly Position Down = new(0, -1);
        private byte _data;

        public Position(sbyte x, sbyte y)
        {
            X = x;
            Y = y;
        }

        public sbyte X
        {
            get => (sbyte) (_data & 0x0F);
            private set => _data = (byte) ((value & 0x0F) + Y);
        }

        public sbyte Y
        {
            get => (sbyte) ((_data & 0xF0) >> 4);
            private set => _data = (byte) (((value & 0x0F) << 4) + X);
        }

        public static Position Ahead(bool isWhite)
        {
            return isWhite ? Up : Down;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position((sbyte) (a.X + b.X), (sbyte) (a.Y + b.Y));
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position((sbyte) (a.X - b.X), (sbyte) (a.Y - b.Y));
        }

        public override bool Equals(object obj)
        {
            var other = (Position) obj;
            return other != null && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
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
            return "(" + X + ", " + Y + ")";
        }
    }
}
