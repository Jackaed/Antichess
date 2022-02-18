using UnityEngine;

namespace Assets.Scripts {
    public class Queen : IPiece {
        public Queen(bool isWhite) {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }
        public GameObject Model => IsWhite ? ObjectLoader.Instance.WQueen : ObjectLoader.Instance.BQueen;
    }
}