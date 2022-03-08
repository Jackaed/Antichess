using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    private GraphicalBoard _board;

    private Vector2Int _from;
    private bool _hasFrom;

    private void Start()
    {
        _board = new GraphicalBoard();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mouseRay = Camera.main!.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out var hit))
            {
                var pos = GetPosFromRaycast(hit);
                if (!_hasFrom)
                {
                    if (_board.PieceAt(pos) != null)
                    {
                        _from = pos;
                        _hasFrom = true;
                    }
                }
                else
                {
                    if (pos != _from) _board.MovePiece(_from, pos);
                    _hasFrom = false;
                }
            }
        }

        for (var x = 0; x < _board.PiecesToMove.Count; x++)
        {
            var pieceToMove = _board.PiecesToMove[x];
            if (pieceToMove.Piece.transform.position != ObjectLoader.GetRealCoords(pieceToMove.To))
            {
                pieceToMove.Piece.transform.position = Vector3.MoveTowards(pieceToMove.Piece.transform.position,
                        ObjectLoader.GetRealCoords(pieceToMove.To), 25 * Time.deltaTime);
            }
            else _board.PiecesToMove.RemoveAt(x);
        }
    }

    private static Vector2Int GetPosFromRaycast(RaycastHit hit)
    {
        return ObjectLoader.GetBoardCoords(hit.point);
    }
}