using System;
using Antichess.Core;
using Antichess.PlayerTypes;
using Antichess.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Antichess.Unity
{
    public class ChessGame : MonoBehaviour
    {
        private RenderedBoard _board;
        private GameObject _gameOverUI;
        private GameObject _mainMenu;
        private bool _renderBoard;
        private State _state;
        private Player _white, _black;

        private MainMenu MainMenuComponent => _mainMenu.GetComponent<MainMenu>();

        private void Start()
        {
            _board = new RenderedBoard(new Vector3(0, 0, 0));
            _state = State.MainMenu;
            InitMainMenu();
        }

        private void Update()
        {
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

                    Player currentPlayer = _board.WhitesMove ? _white : _black;
                    Move attemptedMove = currentPlayer.SuggestMove();

                    if (attemptedMove == null) return;
                    Debug.Log(attemptedMove);
                    _board.Move(attemptedMove);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FixedUpdate()
        {
            if (_state != State.MainMenu)
                _board.FixedUpdate();
        }

        private void InitMainMenu()
        {
            _mainMenu = Object.Instantiate(ObjectLoader.Instance.mainMenuUI);
            MainMenuComponent.startButton.onClick.AddListener(OnStartButtonPress);
        }

        private void OnStartButtonPress()
        {
            _board.StartNewGame();
            _white = MainMenuComponent.GetWhitePlayer(_board);
            _black = MainMenuComponent.GetBlackPlayer(_board);
            _state = State.InGame;
            Destroy(_mainMenu);
        }

        private void OnNewGameButtonPress()
        {
            Destroy(_gameOverUI);
            _board.Destroy();
            _board = new RenderedBoard(new Vector3(0, 0, 0));
            _state = State.MainMenu;
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