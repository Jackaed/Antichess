using UnityEngine;

namespace Assets.Scripts {
    public interface IPiece {
        public bool IsWhite { get; }
        public GameObject Model { get; }
    }
}