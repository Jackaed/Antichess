using System;
using UnityEngine;

namespace Antichess.Core
{
    public class Move
    {
        public enum Flags : byte
        {
            None,
            EnPassant,
            PawnDoubleMove
        }

        public readonly Flags Flag;
        public readonly Position From;
        public readonly Position To;

        public Move(Position from, Position to, Flags flag = Flags.None)
        {
            From = from;
            To = to;
            Flag = flag;
        }

        public float Distance =>
            Mathf.Sqrt(Mathf.Pow(To.X - From.X, 2) + Mathf.Pow(To.Y - From.Y, 2));

        protected bool Equals(Move other)
        {
            return Equals(Flag, other.Flag) && Equals(From, other.From) && Equals(To, other.To);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj.GetType() == GetType() && Equals((Move)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Flag, From, To);
        }

        public static bool operator ==(Move left, Move right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return From + " => " + To;
        }
    }
}
