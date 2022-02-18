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

        private void Awake() {
            Instance = this;
        }
    }
}