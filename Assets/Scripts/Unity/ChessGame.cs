using System;
using System.Collections.Generic;
using Antichess.Core;
using Antichess.PlayerTypes;
using Antichess.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.Unity
{
    public class ChessGame : MonoBehaviour
    {
        private RenderedBoard _board;
        private GameObject _gameOverUI;
        private GameObject _mainMenu;
        private State _state;
        private Player _white, _black;
        private MainMenu MainMenuComponent => _mainMenu.GetComponent<MainMenu>();

        private void ControlCamera()
        {
            Debug.Log(typeof(User));
            if ((_board.WhitesMove ? _white : _black).GetType() == typeof(User))
            {
                Transform desiredTransform = _board.WhitesMove ? ObjectLoader.Instance.WhiteCameraTransform : ObjectLoader.Instance.BlackCameraTransform;
                ObjectLoader.Instance.cam.transform.SetPositionAndRotation(desiredTransform.position, desiredTransform.rotation);
            }
        }

        /// <summary>
        /// Unity function, gets called before the first frame is drawn. Used to initialize the chess game (is effectively a constructor).
        /// </summary>
        private void Start()
        {
            InitMainMenu();
        }

        /// <summary>
        /// Gets called once per frame. Used for collection of user input.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void Update()
        {
            if (_state != State.MainMenu)
                _board.Update();
            switch (_state)
            {
                case State.MainMenu:
                case State.GameOver:
                    return;
                case State.InGame:
                    if (_board.Winner != Board.Winners.None)
                    {
                        _gameOverUI = Instantiate(ObjectLoader.Instance.gameOverUI);
                        _gameOverUI.GetComponentInChildren<Button>().onClick.AddListener(OnNewGameButtonPress);
                        _gameOverUI.GetComponentInChildren<TMP_Text>().text = _board.Winner == Board.Winners.Stalemate
                            ? "Stalemate"
                            : (_board.Winner == Board.Winners.White ? "White" : "Black") + " Wins";
                        _state = State.GameOver;
                    }

                    var currentPlayer = _board.WhitesMove ? _white : _black;
                    var attemptedMove = currentPlayer.SuggestMove();

                    if (attemptedMove == null) return;
                    Debug.Log(attemptedMove);
                    _board.Move(attemptedMove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Initializes main menu
        /// </summary>
        private void InitMainMenu()
        {
            _state = State.MainMenu;
            _board = new RenderedBoard(new List<RenderedBoard.OnMoveDelegate>() { ControlCamera });
            _mainMenu = Instantiate(ObjectLoader.Instance.mainMenuUI);
            MainMenuComponent.startButton.onClick.AddListener(OnStartButtonPress);
        }

        /// <summary>
        /// Function gets called once the Main Menu start button gets pressed. Initializes the board.
        /// </summary>
        private void OnStartButtonPress()
        {
            _white = MainMenuComponent.GetWhitePlayer(_board);
            _black = MainMenuComponent.GetBlackPlayer(_board);
            _board.StartNewGame();
            _state = State.InGame;
            Destroy(_mainMenu);
        }

        /// <summary>
        /// Function gets called when “New Game” is pressed in the game over UI.
        /// </summary>
        private void OnNewGameButtonPress()
        {
            Destroy(_gameOverUI);
            _board.Destroy();
            InitMainMenu();
        }

        private enum State
        {
            MainMenu,
            InGame,
            GameOver
        }
    }
}