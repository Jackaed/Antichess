using System.Collections.Generic;
using UnityEngine;

namespace Antichess
{
    public class ListMove
    {
        public bool CanTake;
        public List<Vector2Int> MoveList;

        public ListMove()
        {
            CanTake = false;
            MoveList = new List<Vector2Int>();
        }
    }
}