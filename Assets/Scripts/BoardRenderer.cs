using UnityEngine;

namespace Antichess
{
    public class BoardRenderer : MonoBehaviour
    {
        private GraphicalBoard _board;
        private Camera _cam;
        private Vector2Int _from;
        private bool _hasFrom;

        private void Start()
        {
            _board = new GraphicalBoard();
            _cam = Camera.main;
        }

        private void Update()
        {
            GetInputs();
            MovePieces();
        }

        private void MovePieces ()
        {
            for (var x = 0; x < _board.PiecesToMove.Count; x++)
            {
                var pieceToMove = _board.PiecesToMove[x];
                var currentPos = pieceToMove.Piece.transform.position;
                if (currentPos != ObjectLoader.GetRealCoords(pieceToMove.To))
                {
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(currentPos,
                        ObjectLoader.GetRealCoords(pieceToMove.To), 25 * Time.deltaTime);
                }
                else _board.PiecesToMove.RemoveAt(x);
            }
        }

        private void GetInputs()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var mouseRay = _cam!.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out var hit)) return;
            var pos = GetPosFromRaycast(hit);
            if (_hasFrom)
            {
                _board.MovePiece(new Move(_from, pos));
                _hasFrom = false;
            }
            else
            {
                if (_board.PieceAt(pos) == null) return;
                _from = pos;
                _hasFrom = true;
            }
        }
        
        private static Vector2Int GetPosFromRaycast(RaycastHit hit)
        {
            return ObjectLoader.GetBoardCoords(hit.point);
        }
    }
}
