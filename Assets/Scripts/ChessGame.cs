using Antichess.PlayerTypes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Antichess
{
    public class ChessGame : MonoBehaviour
    {
        private Board _board;
        private bool _renderBoard;
        private Player _white, _black;

        private void Start()
        {
            _board = new RenderedBoard();
            _white = new User(_board, true);
            _black = new AIPlayer(_board, false);
        }

        private void Update()
        {
            var currentPlayer = _board.WhitesMove ? _white : _black;
            var attemptedMove = currentPlayer.SuggestMove();

            if (attemptedMove != null)
            {
                Debug.Log(attemptedMove);
                _board.MovePiece(attemptedMove);
            }

            if (Input.GetKeyDown("q"))
            {
                _board.UndoLastMove();
            }

            _board.OnNewFrame();
        }
    }
}
