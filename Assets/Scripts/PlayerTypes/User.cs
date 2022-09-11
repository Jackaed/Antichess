using Antichess.Core;
using Antichess.Core.Pieces;
using Antichess.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.PlayerTypes
{
    public class User : Player
    {
        private readonly Camera _cam;
        private Position _from;
        private bool _hasFrom;
        private bool _isClickAndDrag;

        private Position _mouseClickPosition;
        private Piece.Types _promotionPiece;
        private GameObject _promotionUI;
        private Position _selectedPiecePos;
        private bool _userTryingToPromote;

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public User(RenderedBoard board, bool isWhite) : base(board, isWhite)
        {
            _cam = Camera.main;
            _promotionPiece = Piece.Types.None;
            _userTryingToPromote = false;
        }

        private RenderedBoard RenderedBoard => BoardRef as RenderedBoard;

        private Move GetPossibleMove(Move move)
        {
            if (BoardRef.PieceAt(move.From).Type != Piece.Types.Pawn) return move;

            //Test if user is attempting to move the pawn forward by two
            if (move.To.Y == move.From.Y + (IsWhite ? 2 : -2))
                return new Move(move.From, move.To, Move.Flags.PawnDoubleMove);

            // Test if user is attempting to make an en passant
            if (BoardRef.PieceAt(move.To) == null && move.From.X != move.To.X)
                return new Move(move.From, move.To, Move.Flags.EnPassant);

            // Test if user is attempting to promote
            if (!RenderedBoard.MoveCanPromote(move)) return move;

            _userTryingToPromote = true;
            _promotionUI = Object.Instantiate(IsWhite
                ? ObjectLoader.Instance.wPromotionUI
                : ObjectLoader.Instance.bPromotionUI);
            Canvas canvas = _promotionUI.GetComponent<Canvas>();
            canvas.worldCamera = _cam;
            RectTransform transform = _promotionUI.GetComponent<RectTransform>();
            transform.position = RenderedBoard.GetRealCoords(move.To) + 0.5f * Vector3.up;
            Button[] promotionUIButtons = _promotionUI.GetComponentsInChildren<Button>();
            promotionUIButtons[0].onClick.AddListener(OnBishopPromoteButtonClick);
            promotionUIButtons[1].onClick.AddListener(OnKnightPromoteButtonClick);
            promotionUIButtons[2].onClick.AddListener(OnQueenPromoteButtonClick);
            promotionUIButtons[3].onClick.AddListener(OnRookPromoteButtonClick);

            return null;
        }

        private Move ChoosePromotionPiece(Move move)
        {
            if (_promotionPiece == Piece.Types.None) return null;
            Debug.Log(_promotionPiece.ToString());
            Piece.Types temp = _promotionPiece;
            _promotionPiece = Piece.Types.None;
            _userTryingToPromote = false;
            Object.Destroy(_promotionUI);
            _promotionUI = null;
            return new Promotion(move.From, move.To, new Piece(IsWhite, temp));
        }

        private void OnKnightPromoteButtonClick()
        {
            _promotionPiece = Piece.Types.Knight;
            Debug.Log("Knight");
        }

        private void OnBishopPromoteButtonClick()
        {
            _promotionPiece = Piece.Types.Bishop;
        }

        private void OnQueenPromoteButtonClick()
        {
            _promotionPiece = Piece.Types.Queen;
        }

        private void OnRookPromoteButtonClick()
        {
            _promotionPiece = Piece.Types.Rook;
        }

        private void SelectPiece(Position pos)
        {
            if (_selectedPiecePos == pos)
            {
                _selectedPiecePos = null;
                DeselectPiece(pos);
                return;
            }

            _selectedPiecePos = pos;
            RenderedBoard.EnableMeshCollider(pos);
            _isClickAndDrag = false;
            _hasFrom = true;
            _from = pos;
            RenderedBoard.SnapPieceToPos(pos);
            RenderedBoard.LiftPieceAt(pos);
            RenderedBoard.ShowLegalMovesFor(pos);
        }

        private void DeselectPiece(Position pos)
        {
            _hasFrom = false;
            RenderedBoard.SnapPieceToPos(pos);
            RenderedBoard.LowerPieceAt(pos);
            RenderedBoard.EnableMeshCollider(pos);
            RenderedBoard.ClearLegalMoveIndicators();
        }

        private Move OnDCSecondClick(Position pos)
        {
            if (_from == pos)
            {
                DeselectPiece(_from);
                return null;
            }

            if (RenderedBoard.PieceAt(pos) != null &&
                RenderedBoard.PieceAt(pos).IsWhite == RenderedBoard.PieceAt(_from).IsWhite)
            {
                DeselectPiece(_from);
                SelectPiece(pos);
                return null;
            }

            DeselectPiece(_from);
            return GetPossibleMove(new Move(_from, pos));
        }

        private void StartDragAndDrop(Position pos)
        {
            RenderedBoard.ShowLegalMovesFor(pos);
            RenderedBoard.RemoveMeshCollider(pos);
            _isClickAndDrag = true;
            _hasFrom = true;
            _from = pos;
        }

        private Move DragAndDropRelease(Position pos)
        {
            _hasFrom = false;

            if (pos == _from)
            {
                SelectPiece(pos);
                return null;
            }

            RenderedBoard.EnableMeshCollider(_from);
            return GetPossibleMove(new Move(_from, pos));
        }

        public override Move SuggestMove()
        {
            if (_userTryingToPromote) return ChoosePromotionPiece(new Move(_from, _mouseClickPosition));

            Ray mouseRay = _cam!.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out RaycastHit hit)) return null;

            _mouseClickPosition = RenderedBoard.GetBoardCoords(hit.point);

            if (_hasFrom)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!_isClickAndDrag) return OnDCSecondClick(_mouseClickPosition);
                    RenderedBoard.SnapPieceToCursor(_from, hit);
                }
                else if (_isClickAndDrag)
                {
                    return DragAndDropRelease(_mouseClickPosition);
                }

                return null;
            }

            if (Input.GetMouseButton(0) && RenderedBoard.PieceAt(_mouseClickPosition) != null &&
                RenderedBoard.PieceAt(_mouseClickPosition).IsWhite == IsWhite)

                StartDragAndDrop(_mouseClickPosition);

            return null;
        }
    }
}