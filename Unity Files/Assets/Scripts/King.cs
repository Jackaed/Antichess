using UnityEngine;

namespace Assets.Scripts {
    public class King : IPiece {
        public King(bool isWhite) {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        public GameObject Model => IsWhite ? ObjectLoader.Instance.WKing : ObjectLoader.Instance.BKing;
    }
}