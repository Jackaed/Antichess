using System.Collections.Generic;
using UnityEngine;

namespace Antichess
{
    public class ListMove
    {
        public bool CanTake;
        public List<Vector2Int> MoveList;

        public ListMove(bool canTake, List<Vector2Int> moveList)
        {
            CanTake = canTake;
            MoveList = moveList;
        }

        public ListMove()
        {
            CanTake = false;
            MoveList = new();
        }
    }
}