using Antichess.Core;
using Antichess.Core.Pieces;
using Antichess.UI;
using Antichess.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.PlayerTypes
{
    /// <summary>
    /// The Player type for a user, i.e. someone actually interacting with the program.
    /// </summary>
    public class User : Player
    {
        private readonly Camera _cam;
        private Position _from;
        private bool _hasFrom;
        private bool _isClickAndDrag;

        private Position _mouseClickPosition;
        private PromotionUI _promotionUI;
        private Position _selectedPiecePos;
        private bool _userTryingToPromote;

        public User(RenderedBoard board, bool isWhite) : base(board, isWhite)
        {
            _cam = Camera.main;
            _userTryingToPromote = false;
        }

        /// <summary>
        /// Since we know that the board reference has been passed as a rendered board (in the
        /// constructor), this is always a valid conversion, allowing us to get rendering
        /// information about the board.
        /// </summary>
        private RenderedBoard RenderedBoard => BoardRef as RenderedBoard;

        /// <summary>
        /// Attempts to interpret user input into a possible move that they could make. For example,
        /// if the user has suggested pushing a pawn onto the final rank, this interprets this as
        /// the user attempting to promote.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private Move GetPossibleMove(Move move)
        {
            if (BoardRef.PieceAt(move.From).Type != Piece.Types.Pawn)
                return move;

            // Test if user is attempting to move the pawn forward by two
            if (move.To.Y == move.From.Y + (IsWhite ? 2 : -2))
                return new Move(move.From, move.To, Move.Flags.PawnDoubleMove);

            // Test if user is attempting to make an en passant
            if (BoardRef.PieceAt(move.To) == null && move.From.X != move.To.X)
                return new Move(move.From, move.To, Move.Flags.EnPassant);

            // Test if user is attempting to promote
            if (!RenderedBoard.MoveCanPromote(move))
                return move;

            _userTryingToPromote = true;
            RenderedBoard.SnapPieceToPos(_from);
            _promotionUI = new PromotionUI(IsWhite, _cam, move);
            return null;
        }

        /// <summary>
        /// Returns null if the user has not selected a piece to promote to, or returns the
        /// promotion move if the user has made that selection.
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        private Move ChoosePromotionPiece(Move move)
        {
            // If x button is pressed, promotion UI is destroyed, so null comparison passes.
            if (_promotionUI.IsCancelled)
            {
                CancelledPromotion();
                return null;
            }

            Piece.Types selection = _promotionUI.Selection;
            if (selection == Piece.Types.None)
                return null;

            _userTryingToPromote = false;
            _promotionUI = null;
            return new Core.Promotion(move.From, move.To, new Piece(IsWhite, selection));
        }

        /// <summary>
        /// Called when the user presses the "cancel" button when promoting.
        /// </summary>
        private void CancelledPromotion()
        {
            _userTryingToPromote = false;
            DeselectPiece(_from);
            _from = null;
        }

        /// <summary>
        /// Visually selects a piece, lifting it up and showing it's legal moves.
        /// </summary>
        /// <param name="pos"></param>
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

        /// <summary>
        /// Unselects the piece at the given position. Does the opposite of SelectPiece.
        /// </summary>
        /// <param name="pos"></param>
        private void DeselectPiece(Position pos)
        {
            _isClickAndDrag = false;
            _hasFrom = false;
            RenderedBoard.SnapPieceToPos(pos);
            RenderedBoard.LowerPieceAt(pos);
            RenderedBoard.EnableMeshCollider(pos);
            RenderedBoard.ClearLegalMoveIndicators();
        }

        /// <summary>
        /// Gets called whenever the user clicks for the second time when moving a piece by clicking twice.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Move OnDCSecondClick(Position pos)
        {
            if (pos is null || _from == pos)
            {
                DeselectPiece(_from);
                return null;
            }

            if (
                RenderedBoard.PieceAt(pos) != null
                && RenderedBoard.PieceAt(pos).IsWhite == RenderedBoard.PieceAt(_from).IsWhite
            )
            {
                DeselectPiece(_from);
                SelectPiece(pos);
                return null;
            }

            DeselectPiece(_from);
            return GetPossibleMove(new Move(_from, pos));
        }

        /// <summary>
        /// Gets called whenever the user begins a drag and drop piece movement.
        /// </summary>
        /// <param name="pos"></param>
        private void StartDragAndDrop(Position pos)
        {
            RenderedBoard.ShowLegalMovesFor(pos);
            RenderedBoard.RemoveMeshCollider(pos);
            _isClickAndDrag = true;
            _hasFrom = true;
            _from = pos;
        }

        /// <summary>
        /// Gets called whenever the user finishes dragging and dropping a piece.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private Move DragAndDropRelease(Position pos)
        {
            if (pos is null)
            {
                if (_hasFrom)
                {
                    DeselectPiece(_from);
                }

                return null;
            }

            _hasFrom = false;

            if (pos == _from)
            {
                SelectPiece(pos);
                return null;
            }

            RenderedBoard.EnableMeshCollider(_from);
            return GetPossibleMove(new Move(_from, pos));
        }

        /// <summary>
        /// The actual function override for a player, which returns null until the player has
        /// actually finished suggesting a move, at which point it returns the move that player suggested.
        /// </summary>
        /// <returns></returns>
        public override Move SuggestMove()
        {
            if (_userTryingToPromote)
                return ChoosePromotionPiece(new Move(_from, _mouseClickPosition));

            var mouseRay = _cam!.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out var hit))
                return null;

            _mouseClickPosition = RenderedBoard.GetBoardCoords(hit.point);

            if (_hasFrom)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!_isClickAndDrag)
                        return OnDCSecondClick(_mouseClickPosition);

                    RenderedBoard.SnapPieceToCursor(_from, hit);
                }
                else if (_isClickAndDrag)
                {
                    return DragAndDropRelease(_mouseClickPosition);
                }

                return null;
            }

            if (_mouseClickPosition is null)
                return null;

            if (
                Input.GetMouseButton(0)
                && RenderedBoard.PieceAt(_mouseClickPosition) != null
                && RenderedBoard.PieceAt(_mouseClickPosition).IsWhite == IsWhite
            )
            {
                StartDragAndDrop(_mouseClickPosition);
            }

            return null;
        }
    }
}
