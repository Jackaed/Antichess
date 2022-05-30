using UnityEngine;
using Antichess.TargetSquares;

namespace Antichess
{
    public class Move
    {
        public Position From, To;

        public Move(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public override string ToString()
        {
            return From + " , " + To;
        }
    }
}