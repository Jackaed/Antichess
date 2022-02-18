using UnityEngine;

namespace Assets.Scripts {
    internal class Move {
        public Move(Vector2Int from, Vector2Int to) {
            From = from;
            To = to;
        }

        private Vector2Int From { get; }
        private Vector2Int To { get; }
    }
}