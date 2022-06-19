using System;
using System.Linq;
using Antichess.Pieces;
using Antichess.TargetSquares;
using Unity.VisualScripting;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;

namespace Antichess.PlayerTypes
{
    public class User : Player
    {
        private readonly Camera _cam;
        private Position _from;
        private Type _promotionPiece;
        private GameObject _promotionUI;
        private bool _userTryingToPromote;
        private bool _hasFrom;

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
            if (move.To.y == move.From.y + (IsWhite ? 2 : -2))
                return new Move(_from, new PawnDoubleMovePosition(move.To,
                    move.To - (IsWhite ? Vector2Int.up : Vector2Int.down)));

            // Test if user is attempting to make an en passant
            if (BoardRef.PieceAt(move.To) == null && move.From.x != move.To.x)
                return new Move(_from, new EnPassantPosition(move.To,
                    new Position(move.To.x, (byte) (move.To.y - (IsWhite ? 1 : -1)))));

            // Test if user is attempting to promote
            if (move.To.y == (IsWhite ? 7 : 0))
            {
                _userTryingToPromote = true;
                _promotionUI = Object.Instantiate(IsWhite
                    ? ObjectLoader.Instance.wPromotionUI
                    : ObjectLoader.Instance.bPromotionUI);
                var promotionUIButtons = _promotionUI.GetComponentsInChildren<UnityEngine.UI.Button>();
                promotionUIButtons[0].onClick.AddListener(OnBishopPromoteButtonClick);
                promotionUIButtons[1].onClick.AddListener(OnKnightPromoteButtonClick);
                promotionUIButtons[2].onClick.AddListener(OnQueenPromoteButtonClick);
                promotionUIButtons[3].onClick.AddListener(OnRookPromoteButtonClick);

                return null;
            }
            return move;
        }

        Move ChoosePromotionPiece(Move move)
        {
            if (_promotionPiece == null) return null;
            Debug.Log(_promotionPiece.ToString());
            var temp = _promotionPiece;
            _promotionPiece = null;
            _userTryingToPromote = false;
            object[] param = (new[] {IsWhite}).Cast<object>().ToArray();
            Object.Destroy(_promotionUI);
            _promotionUI = null;
            return new Move(move.From, new PromotionPosition(move.To.x, move.To.y,
                (Piece)Activator.CreateInstance(temp, param)));
        }

        void OnKnightPromoteButtonClick()
        {
            _promotionPiece = typeof(Knight);
            Debug.Log("Knight");
        }
        void OnBishopPromoteButtonClick() {_promotionPiece = typeof(Bishop); }
        void OnQueenPromoteButtonClick() {_promotionPiece = typeof(Queen); }
        void OnRookPromoteButtonClick() {_promotionPiece = typeof(Rook); }


        private Position _mouseClickPosition;
        public override Move SuggestMove()
        {
            if (_userTryingToPromote)
            {
                return ChoosePromotionPiece(new Move(_from, _mouseClickPosition));
            }
            
            if (!Input.GetMouseButtonDown(0)) return null;
            
            var mouseRay = _cam!.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out var hit)) return null;

            _mouseClickPosition = ObjectLoader.GetBoardCoords(hit.point);
            if (_hasFrom)
            {
                Move move = GetPossibleMove(new Move(_from, _mouseClickPosition));
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
