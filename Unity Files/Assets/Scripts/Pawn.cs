﻿using UnityEngine;

namespace Assets.Scripts {
    public class Pawn : IPiece {
        public Pawn(bool isWhite) {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        public GameObject Model => IsWhite ? ObjectLoader.Instance.WPawn : ObjectLoader.Instance.BPawn;
    }
}