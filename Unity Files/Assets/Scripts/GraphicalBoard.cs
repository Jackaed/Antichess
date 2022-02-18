using UnityEngine;

namespace Assets.Scripts {
    internal class GraphicalBoard : Board {
        public GameObject[,] BoardGameObjects;

        public new void AddPiece(IPiece piece, Vector2Int pos) {
            base.AddPiece(piece, pos);
            BoardGameObjects[pos.x, pos.y] = Object.Instantiate(piece.Model,
                new Vector3((pos.x - 3.5f) * 0.6f, 0, (pos.y - 3.5f) * 0.6f), piece.Model.transform.rotation);
        }

        public new void MovePiece(Vector2Int from, Vector2Int to) {
            base.MovePiece(from, to);
            if (BoardGameObjects[to.x, to.y] != null) Object.Destroy(BoardGameObjects[to.x, to.y]);

            BoardGameObjects[to.x, to.y] = BoardGameObjects[from.x, from.y];
            BoardGameObjects[to.x, to.y].transform.position =
                new Vector3((to.x - 3.5f) * 0.6f, 0, (to.y - 3.5f) * 0.6f);
            if (BoardGameObjects[from.x, from.y] != null) Object.Destroy(BoardGameObjects[from.x, from.y]);
        }
    }
}