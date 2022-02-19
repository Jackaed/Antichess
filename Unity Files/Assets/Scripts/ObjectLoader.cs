using UnityEngine;

namespace Assets.Scripts {
    public class ObjectLoader : MonoBehaviour {
        public GameObject BPawn,
            BBishop,
            BKnight,
            BRook,
            BQueen,
            BKing,
            WPawn,
            WBishop,
            WKnight,
            WRook,
            WQueen,
            WKing,
            Board;

        public static ObjectLoader Instance { get; private set; }

        public static Vector3 GetRealCoords(Vector2Int BoardCoords)
        {
            return new Vector3((BoardCoords.x - 3.5f) * 0.6f, 0, (BoardCoords.y - 3.5f) * 0.6f);
        }

        private void Awake() {
            Instance = this;
        }
    }
}