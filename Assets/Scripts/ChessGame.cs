using Antichess.PlayerTypes;
using UnityEngine;

namespace Antichess
{
    public class ChessGame : MonoBehaviour
    {
        private Player _white, _black;
        private Board _board;
        private bool _renderBoard;

        private void Start()
        {
            _board = new Board(true);
            _white = new User(_board, true);
            _black = new User(_board, false);
        }

        private void Update()
        {
            var currentPlayer = _board.WhitesMove ? _white : _black;
            var attemptedMove = currentPlayer.SuggestMove();
            if (attemptedMove != null)
            {
                _board.TryMove(attemptedMove);
            }
            _board.OnNewFrame();
        }
    }
}