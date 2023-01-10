using UnityEngine;
using UnityEngine.UI;
using Antichess.Core;
using Antichess.Unity.UIMonoBehaviour;
using Antichess.Unity;

namespace Antichess.UI
{
    public class GameOverUI
    {
        private readonly GameOverMB _gameOverMB;

        public GameOverUI(Board.Winners winner)
        {
            _gameOverMB = Object
                .Instantiate(ObjectLoader.Instance.gameOverUI)
                .GetComponent<GameOverMB>();
            _gameOverMB.GameOverText.text =
                winner == Board.Winners.Stalemate
                    ? "Stalemate"
                    : (winner == Board.Winners.White ? "White" : "Black") + " Wins";
        }

        public Button.ButtonClickedEvent PlayAgain => _gameOverMB.PlayAgain.onClick;
    }
}
