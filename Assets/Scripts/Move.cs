using UnityEngine;

namespace Antichess
{
    public class Move
    {
        public Vector2Int From, To;

        public Move(Vector2Int from, Vector2Int to)
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