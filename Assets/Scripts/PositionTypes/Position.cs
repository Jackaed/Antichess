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
        
        // Since a chessboard is only 8 by 8, X and Y only need to be able to contain the values 0 through 7, everything
        // else is a bonus. Therefore, X and Y are simply nibbles, stored collectively in a single byte "_data". X and Y
        // are signed, to facilitate addition of positions where one of the positions is negative, e.g. in storing
        // offsets for a piece's movement.
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

        // Heavily used in pawns.
        public static Position Ahead(bool isWhite)
        {
            return isWhite ? Up : Down;
        }
        
        // The following is operator overloads, facilitating the addition, subtraction and comparison of Positions.
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

        public Position Clone()
        {
            return (Position)MemberwiseClone();
        }
    }
}
