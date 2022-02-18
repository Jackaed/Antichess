using UnityEngine;

namespace Assets.Scripts {
    public class BoardRenderer : MonoBehaviour {
        private Board BoardData;
        private GameObject[,] VisualBoard;

        public void LoadVisuals(Board board) {
            for (var x = 0; x < Board.Size.x; x++)
            for (var y = 0; y < Board.Size.y; y++)
                if (board.Data[x, y] != null)
                    VisualBoard[x, y] = Instantiate(board.Data[x, y].Model,
                        new Vector3((x - 3.5f) * 0.6f, 0, (y - 3.5f) * 0.6f),
                        board.Data[x, y].Model.transform.rotation);
        }

        private void Start() {
            BoardData = new Board();
            LoadVisuals(BoardData);
        }
    }
}