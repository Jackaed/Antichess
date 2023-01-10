using System;
using System.Collections.Generic;
using System.Linq;
using Antichess.Core.Pieces;
using Antichess.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Antichess.Core
{
    public class RenderedBoard : Board
    {
        private const float TimeToMove = 0.2f;
        private const float MoveSpeed = 1 / TimeToMove;
        private readonly List<RenderedPiece> _highlightedPieces = new();
        private readonly List<GameObject> _legalMoveIndicators = new();
        private readonly List<MovingPiece> _piecesToMove = new();
        public delegate void OnMoveDelegate();
        private readonly List<OnMoveDelegate> OnMove;

        public RenderedBoard(List<OnMoveDelegate> onMoveDelegates)
        {
            OnMove = onMoveDelegates;
        }

        /// <summary>
        /// Override from Board.Create. Similar, except it creates a RenderedPiece, instead of a
        /// regular piece, and creates it’s GameObject.
        /// </summary>
        /// <param name="pos"></param>
        private RenderedPiece RenderedPieceAt(Position pos)
        {
            return PieceAt(pos) as RenderedPiece;
        }

        protected override void Create(Piece piece, Position pos)
        {
            base.Create(RenderedPiece.ToRenderedPiece(piece, GetRealCoords(pos)), pos);
        }

        /// <summary>
        /// Override from Board.UndoMove. Similar, except it moves GameObjects and restores any
        /// GameObjects that were captured in the move.
        /// </summary>
        /// <param name="change"></param>
        protected override void UndoMove(BoardChange change)
        {
            _piecesToMove.Add(
                new MovingPiece(
                    change.Move.From,
                    RenderedPieceAt(change.Move.To).GameObject,
                    change.Move.Distance
                )
            );
            base.UndoMove(change);
        }

        /// <summary>
        /// Override from Board.Create. Similar, except it creates a RenderedPiece, instead of a
        /// regular piece, and creates its GameObject.
        /// </summary>
        /// <param name="pos"></param>
        private void CreateLegalMoveIndicator(Position pos)
        {
            if (PieceAt(pos) != null)
            {
                var highlightedPiece = RenderedPieceAt(pos);
                var highlightedGameObject = highlightedPiece.GameObject;
                highlightedGameObject.GetComponent<MeshRenderer>().material =
                    highlightedPiece.IsWhite
                        ? ObjectLoader.Instance.wEmissiveMaterial
                        : ObjectLoader.Instance.bEmissiveMaterial;
                _highlightedPieces.Add(highlightedPiece);
            }
            else
            {
                var indicator = Object.Instantiate(ObjectLoader.Instance.legalMoveIndicator);
                indicator.transform.position = GetRealCoords(pos) + new Vector3(0, 0.0001f, 0);
                _legalMoveIndicators.Add(indicator);
            }
        }

        public override void StartNewGame()
        {
            base.StartNewGame();
            foreach (OnMoveDelegate del in OnMove)
            {
                del();
            }
        }

        /// <summary>
        /// Tests if a move can promote.
        /// </summary>
        /// <param name="move"></param>
        /// <returns>Returns true if the parameter passed can promote a piece, or false if it
        /// cannot.</returns>
        public bool MoveCanPromote(Move move)
        {
            return LegalMoves.List.Any(
                legalPromotion =>
                    legalPromotion is Promotion
                    && move.From == legalPromotion.From
                    && move.To == legalPromotion.To
            );
        }

        public void ClearLegalMoveIndicators()
        {
            foreach (var indicator in _legalMoveIndicators)
                Object.Destroy(indicator);

            foreach (var highlightedPiece in _highlightedPieces)
            {
                highlightedPiece.GameObject.GetComponent<MeshRenderer>().material =
                    highlightedPiece.IsWhite
                        ? ObjectLoader.Instance.wBaseMaterial
                        : ObjectLoader.Instance.bBaseMaterial;
            }

            _legalMoveIndicators.Clear();
            _highlightedPieces.Clear();
        }

        /// <summary>
        /// Disables the mesh collider for the piece at a given position, allowing ray casts to
        /// travel through the piece. Used when dragging and dropping pieces, to prevent a piece
        /// from constantly traveling closer to the camera when being dragged.
        /// </summary>
        /// <param name="pos"></param>
        public void RemoveMeshCollider(Position pos)
        {
            RenderedPieceAt(pos).GameObject.GetComponent<Collider>().enabled = false;
        }

        /// <summary>
        /// Re-enables the mesh collider for the piece at a given position. Used for when a piece
        /// has stopped being dragged and dropped, and needs to be selectable again.
        /// </summary>
        /// <param name="pos"></param>
        public void EnableMeshCollider(Position pos)
        {
            RenderedPieceAt(pos).GameObject.GetComponent<Collider>().enabled = true;
        }

        public void SnapPieceToCursor(Position originalPos, RaycastHit hit)
        {
            RenderedPieceAt(originalPos).GameObject.transform.position =
                hit.point + new Vector3(0, 0.25f, 0);
        }

        public void SnapPieceToPos(Position pos)
        {
            RenderedPieceAt(pos).GameObject.transform.position = GetRealCoords(pos);
        }

        /// <summary>
        /// Creates Legal Move indicators for the piece at a given location, allowing a user to see
        /// where a selected piece can move to.
        /// </summary>
        /// <param name="pos"></param>
        public void ShowLegalMovesFor(Position pos)
        {
            ClearLegalMoveIndicators();
            foreach (var move in LegalMoves.List.Where(move => move.From == pos))
                CreateLegalMoveIndicator(move.To);
        }

        /// <summary>
        /// Lowers the piece at a given position back down to regular height, allowing you to
        /// “deselect” that piece.
        /// </summary>
        /// <param name="pos"></param>
        public void LowerPieceAt(Position pos)
        {
            var gamePos = RenderedPieceAt(pos).GameObject.transform.position;
            RenderedPieceAt(pos).GameObject.transform.position = new Vector3(
                gamePos.x,
                0,
                gamePos.z
            );
        }

        /// <summary>
        /// Elevates the piece at a given position, raising it vertically to show that the piece has
        /// been “selected” or highlighted in some way.
        /// </summary>
        /// <param name="pos"></param>
        public void LiftPieceAt(Position pos)
        {
            var gamePos = RenderedPieceAt(pos).GameObject.transform.position;
            RenderedPieceAt(pos).GameObject.transform.position = new Vector3(
                gamePos.x,
                0.25f,
                gamePos.z
            );
        }

        protected override void UpdateWinner()
        {
            base.UpdateWinner();
            switch (Winner)
            {
                case Winners.White:
                    Debug.Log("White Wins");
                    break;
                case Winners.Black:
                    Debug.Log("Black Wins");
                    break;
                case Winners.Stalemate:
                    Debug.Log("Stalemate");
                    break;
                case Winners.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Override from Board.Move. Identical, but calls RenderedBoard.UnsafeMove, so GameObjects
        /// get managed, and also deselects any selected pieces upon a move attempt being made
        /// (regardless of legality).
        /// </summary>
        /// <param name="move"></param>
        public override bool Move(Move move)
        {
            Debug.Log(move);
            var wasLegal = base.Move(move);
            if (wasLegal)
                return true;
            LowerPieceAt(move.From);
            RenderedPieceAt(move.From).GameObject.transform.position = GetRealCoords(move.From);
            ClearLegalMoveIndicators();
            return false;
        }

        protected override void UnsafeMove(Move move)
        {
            ClearLegalMoveIndicators();
            if (LegalMoves.CanTake)
                ObjectLoader.Instance.audioSource.PlayOneShot(ObjectLoader.Instance.capture, 1);
            else
                ObjectLoader.Instance.audioSource.PlayOneShot(ObjectLoader.Instance.move, 1);

            var pieceTo = RenderedPieceAt(move.To);

            base.UnsafeMove(move);

            var pieceFrom = RenderedPieceAt(move.To);

            if (pieceFrom != null)
                _piecesToMove.Add(new MovingPiece(move.To, pieceFrom.GameObject, move.Distance));

            foreach (OnMoveDelegate del in OnMove)
            {
                del();
            }
        }

        protected override void Destroy(Position pos)
        {
            if (RenderedPieceAt(pos) != null)
                Object.Destroy(RenderedPieceAt(pos).GameObject);
            base.Destroy(pos);
        }

        /// <summary>
        /// Gets called once per frame by ChessGame. Used to update the locations of pieces smoothly
        /// over a number of frames, to allow for smooth piece movement.
        /// </summary>
        public void Update()
        {
            for (var x = 0; x < _piecesToMove.Count; x++)
            {
                var pieceToMove = _piecesToMove[x];

                if (
                    pieceToMove.Piece == null
                    || pieceToMove.Piece.transform.position == GetRealCoords(pieceToMove.To)
                )
                {
                    _piecesToMove.RemoveAt(x);
                }
                else
                {
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(
                        pieceToMove.Piece.transform.position,
                        GetRealCoords(pieceToMove.To),
                        MoveSpeed * Time.deltaTime * pieceToMove.Distance
                    );
                }
            }
        }

        private static float GetRealCoord(int boardCoord)
        {
            return 0.6f * (boardCoord - 3.5f);
        }

        /// <summary>
        /// Returns the real location in 3d space in Unity from a given 2d board location, by
        /// calling GetRealCoord on each of the axes.
        /// </summary>
        /// <param name="boardCoords"></param>
        public static Vector3 GetRealCoords(Position boardCoords)
        {
            return new Vector3(GetRealCoord(boardCoords.X), 0, GetRealCoord(boardCoords.Y));
        }

        private static sbyte? GetBoardCoord(float num)
        {
            var coord = Math.Round((num / 0.6f) + 3.5f);
            if (coord < 0 || coord > Board.Size - 1)
            {
                return null;
            }
            return (sbyte?)coord;
        }

        /// <summary>
        /// Returns a location on the board from a location in Unity’s 3d space. Returns null if the
        /// location is off of the board.
        /// </summary>
        /// <param name="realCoords"></param>
        public static Position GetBoardCoords(Vector3 realCoords)
        {
            sbyte? x = GetBoardCoord(realCoords.x);
            sbyte? y = GetBoardCoord(realCoords.z);

            if (!x.HasValue || !y.HasValue)
            {
                return null;
            }

            return new Position(
                (sbyte)GetBoardCoord(realCoords.x),
                (sbyte)GetBoardCoord(realCoords.z)
            );
        }
    }
}
