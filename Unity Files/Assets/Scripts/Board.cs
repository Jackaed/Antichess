using UnityEngine;

namespace Assets.Scripts {
    public class Board {
        public static readonly Vector2Int Size = new(8, 8);
        public IPiece[,] Data;
        public bool MoveMade = false;

        public Board() {
            Data = new IPiece[Size.x, Size.y];
        }

        public void AddPiece(IPiece piece, Vector2Int pos) {
            Data[pos.x, pos.y] = piece;
        }

        public void MovePiece(Vector2Int from, Vector2Int to) {
            Data[to.x, to.y] = Data[from.x, from.y];
            Data[from.x, from.y] = null;
        }
    }
}