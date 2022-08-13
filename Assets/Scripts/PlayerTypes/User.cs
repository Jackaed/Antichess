using System;
using System.Linq;
using Antichess.Core;
using Antichess.Core.Pieces;
using Antichess.Unity;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Antichess.PlayerTypes
{
    public class User : Player
    {
        private readonly Camera _cam;
        private readonly RenderedBoard _renderedBoard;
        private Position _from;
        private bool _hasFrom;

        private Position _mouseClickPosition;
        private Type _promotionPiece;
        private GameObject _promotionUI;
        private bool _userTryingToPromote;

        public User(RenderedBoard board, bool isWhite) : base(board, isWhite)
        {
            _renderedBoard = board;
            _cam = Camera.main;
            _promotionPiece = null;
            _userTryingToPromote = false;
        }

        private Move GetPossibleMove(Move move)
        {
            if (BoardRef.PieceAt(move.From).GetType() != typeof(Pawn)) return move;

            //Test if user is attempting to move the pawn forward by two
            if (move.To.Y == move.From.Y + (IsWhite ? 2 : -2))
                return new Move(move.From, move.To, Move.Flags.PawnDoubleMove);

            // Test if user is attempting to make an en passant
            if (BoardRef.PieceAt(move.To) == null && move.From.X != move.To.X)
                return new Move(move.From, move.To, Move.Flags.EnPassant);

            // Test if user is attempting to promote
            if (move.To.Y == (IsWhite ? 7 : 0))
            {
                _userTryingToPromote = true;
                _promotionUI = Object.Instantiate(IsWhite
                    ? ObjectLoader.Instance.wPromotionUI
                    : ObjectLoader.Instance.bPromotionUI);
                var canvas = _promotionUI.GetComponent<Canvas>();
                canvas.worldCamera = _cam;
                var transform = _promotionUI.GetComponent<RectTransform>();
                transform.position = ObjectLoader.GetRealCoords(move.To) + 0.5f * Vector3.up;
                var promotionUIButtons = _promotionUI.GetComponentsInChildren<Button>();
                promotionUIButtons[0].onClick.AddListener(OnBishopPromoteButtonClick);
                promotionUIButtons[1].onClick.AddListener(OnKnightPromoteButtonClick);
                promotionUIButtons[2].onClick.AddListener(OnQueenPromoteButtonClick);
                promotionUIButtons[3].onClick.AddListener(OnRookPromoteButtonClick);

                return null;
            }

            return move;
        }

        private Move ChoosePromotionPiece(Move move)
        {
            if (_promotionPiece == null) return null;
            Debug.Log(_promotionPiece.ToString());
            var temp = _promotionPiece;
            _promotionPiece = null;
            _userTryingToPromote = false;
            var param = new[] {IsWhite}.Cast<object>().ToArray();
            Object.Destroy(_promotionUI);
            _promotionUI = null;
            return new Promotion(move.From, move.To, (Piece) Activator.CreateInstance(temp, param));
        }

        private void OnKnightPromoteButtonClick()
        {
            _promotionPiece = typeof(Knight);
            Debug.Log("Knight");
        }

        private void OnBishopPromoteButtonClick()
        {
            _promotionPiece = typeof(Bishop);
        }

        private void OnQueenPromoteButtonClick()
        {
            _promotionPiece = typeof(Queen);
        }

        private void OnRookPromoteButtonClick()
        {
            _promotionPiece = typeof(Rook);
        }

        public override Move SuggestMove()
        {
            if (_userTryingToPromote) return ChoosePromotionPiece(new Move(_from, _mouseClickPosition));

            if (!Input.GetMouseButtonDown(0)) return null;

            var mouseRay = _cam!.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out var hit)) return null;

            _mouseClickPosition = ObjectLoader.GetBoardCoords(hit.point);
            if (_hasFrom)
            {
                var move = GetPossibleMove(new Move(_from, _mouseClickPosition));
                _hasFrom = false;
                if (_from != _mouseClickPosition)
                    return move;
                _renderedBoard.LowerPieceAt(_from);
                _renderedBoard.ClearLegalMoveIndicators();
                return null;
            }

            if (BoardRef.PieceAt(_mouseClickPosition) == null) return null;

            _renderedBoard.LiftPieceAt(_mouseClickPosition);
            _renderedBoard.ShowLegalMovesFor(_mouseClickPosition);
            _from = _mouseClickPosition;
            _hasFrom = true;
            return null;
        }
    }
}