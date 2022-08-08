using System;
using System.Linq;
using Antichess.Pieces;
using Antichess.PositionTypes;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Antichess.PlayerTypes
{
    public class User : Player
    {
        private readonly Camera _cam;
        private Position _from;
        private bool _hasFrom;


        private Position _mouseClickPosition;
        private Type _promotionPiece;
        private GameObject _promotionUI;
        private bool _userTryingToPromote;

        public User(Board board, bool isWhite) : base(board, isWhite)
        {
            _cam = Camera.main;
            _promotionPiece = null;
            _userTryingToPromote = false;
        }

        private Move GetPossibleMove(Move move)
        {
            if (BoardRef.PieceAt(move.From).GetType() != typeof(Pawn)) return move;

            //Test if user is attempting to move the pawn forward by two
            if (move.To.Y == move.From.Y + (IsWhite ? 2 : -2))
                return new Move(_from, new PawnDoubleMovePosition(move.To,
                    move.To - (IsWhite ? Position.Up : Position.Down)));

            // Test if user is attempting to make an en passant
            if (BoardRef.PieceAt(move.To) == null && move.From.X != move.To.X)
                return new Move(_from, new EnPassantPosition(move.To,
                    move.To + (IsWhite ? Position.Down : Position.Up)));

            // Test if user is attempting to promote
            if (move.To.Y == (IsWhite ? 7 : 0))
            {
                _userTryingToPromote = true;
                _promotionUI = Object.Instantiate(IsWhite
                    ? Constants.Instance.wPromotionUI
                    : Constants.Instance.bPromotionUI);
                var canvas = _promotionUI.GetComponent<Canvas>();
                canvas.worldCamera = _cam;
                var transform = _promotionUI.GetComponent<RectTransform>();
                transform.position = Constants.GetRealCoords(move.To) + 0.5f * Vector3.up;
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
            return new Move(move.From, new PromotionPosition(move.To.X, move.To.Y,
                (Piece) Activator.CreateInstance(temp, param)));
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

            _mouseClickPosition = Constants.GetBoardCoords(hit.point);
            if (_hasFrom)
            {
                var move = GetPossibleMove(new Move(_from, _mouseClickPosition));
                _hasFrom = false;
                return move;
            }

            if (BoardRef.PieceAt(_mouseClickPosition) == null) return null;
            _from = _mouseClickPosition;
            _hasFrom = true;
            return null;
        }
    }
}
