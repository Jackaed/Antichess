using UnityEngine;

namespace Assets.Scripts {
    public class Bishop : IPiece {
        public Bishop(bool isWhite) {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        public GameObject Model => IsWhite ? ObjectLoader.Instance.WBishop : ObjectLoader.Instance.BBishop;
    }
}