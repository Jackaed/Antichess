using UnityEngine;
using UnityEngine.UI;
using Antichess.Core;
using Antichess.Unity.UIMonoBehaviour;
using Antichess.Unity;

namespace Antichess.UI
{
    /// <summary>
    /// Represents an abstraction of the Game Over UI menu, allowing a user to create and destroy a
    /// game over UI menu without manually managing the MonoBehaviour script attatched to the GameObject.
    /// </summary>
    public class GameOverUI
    {
        private readonly GameOverMB _gameOverMB;

        /// <summary>
        /// Sets the game over UI text to the appropriate winner of the game.
        /// </summary>
        /// <param name="winner"></param>
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
