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
        private readonly Vector3 _origin;
        private readonly List<MovingPiece> _piecesToMove = new();

        public RenderedBoard(Vector3 origin)
        {
            _origin = origin;
        }

        private RenderedPiece RenderedPieceAt(Position pos)
        {
            return PieceAt(pos) as RenderedPiece;
        }

        protected override void Create(Piece piece, Position pos)
        {
            base.Create(RenderedPiece.ToRenderedPiece(piece, GetRealCoords(pos)), pos);
        }

        protected override void UndoMove(BoardChange change)
        {
            _piecesToMove.Add(new MovingPiece(change.Move.From, RenderedPieceAt(change.Move.To).GameObject,
                change.Move.Distance));
            base.UndoMove(change);
        }

        private void CreateLegalMoveIndicator(Position pos)
        {
            if (PieceAt(pos) != null)
            {
                RenderedPiece highlightedPiece = RenderedPieceAt(pos);
                GameObject highlightedGameObject = highlightedPiece.GameObject;
                highlightedGameObject.GetComponent<MeshRenderer>().material = highlightedPiece.IsWhite
                    ? ObjectLoader.Instance.wEmissiveMaterial
                    : ObjectLoader.Instance.bEmissiveMaterial;
                _highlightedPieces.Add(highlightedPiece);
            }
            else
            {
                GameObject indicator = Object.Instantiate(ObjectLoader.Instance.legalMoveIndicator);
                indicator.transform.position = GetRealCoords(pos) + new Vector3(0, _origin.y + 0.0001f, 0);
                _legalMoveIndicators.Add(indicator);
            }
        }

        public bool MoveCanPromote(Move move)
        {
            return LegalMoves.List.Where(legalPromotion => legalPromotion is Promotion).Any
                (legalPromotion => move.From == legalPromotion.From && move.To == legalPromotion.To);
        }

        public void ClearLegalMoveIndicators()
        {
            foreach (GameObject indicator in _legalMoveIndicators) Object.Destroy(indicator);

            foreach (RenderedPiece highlightedPiece in _highlightedPieces)
                highlightedPiece.GameObject.GetComponent<MeshRenderer>().material = highlightedPiece.IsWhite
                    ? ObjectLoader.Instance.wBaseMaterial
                    : ObjectLoader.Instance.bBaseMaterial;

            _legalMoveIndicators.Clear();
            _highlightedPieces.Clear();
        }

        public void RemoveMeshCollider(Position pos)
        {
            RenderedPieceAt(pos).GameObject.GetComponent<Collider>().enabled = false;
        }

        public void EnableMeshCollider(Position pos)
        {
            RenderedPieceAt(pos).GameObject.GetComponent<Collider>().enabled = true;
        }

        public void SnapPieceToCursor(Position originalPos, RaycastHit hit)
        {
            RenderedPieceAt(originalPos).GameObject.transform.position =
                hit.point + new Vector3(0, _origin.y + 0.25f, 0);
        }

        public void SnapPieceToPos(Position pos)
        {
            RenderedPieceAt(pos).GameObject.transform.position = GetRealCoords(pos);
        }

        public void ShowLegalMovesFor(Position pos)
        {
            ClearLegalMoveIndicators();
            foreach (Move move in LegalMoves.List.Where(move => move.From == pos)) CreateLegalMoveIndicator(move.To);
        }

        public void LowerPieceAt(Position pos)
        {
            Vector3 gamePos = RenderedPieceAt(pos).GameObject.transform.position;
            RenderedPieceAt(pos).GameObject.transform.position = new Vector3(gamePos.x, _origin.y, gamePos.z);
        }

        public void LiftPieceAt(Position pos)
        {
            Vector3 gamePos = RenderedPieceAt(pos).GameObject.transform.position;
            RenderedPieceAt(pos).GameObject.transform.position = new Vector3(gamePos.x, _origin.y + 0.25f, gamePos.z);
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

        public override bool Move(Move move)
        {
            Debug.Log(LegalMoves);
            bool wasLegal = base.Move(move);
            if (wasLegal) return true;
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

            RenderedPiece pieceTo = RenderedPieceAt(move.To);

            base.UnsafeMove(move);

            RenderedPiece pieceFrom = RenderedPieceAt(move.To);

            if (pieceFrom != null)
                _piecesToMove.Add(new MovingPiece(move.To, pieceFrom.GameObject, move.Distance));

            if (pieceTo == null) return;
        }

        protected override void Destroy(Position pos)
        {
            if (RenderedPieceAt(pos) != null) Object.Destroy(RenderedPieceAt(pos).GameObject);
            base.Destroy(pos);
        }

        public void FixedUpdate()
        {
            for (int x = 0; x < _piecesToMove.Count; x++)
            {
                MovingPiece pieceToMove = _piecesToMove[x];

                if (pieceToMove.Piece == null ||
                    pieceToMove.Piece.transform.position == GetRealCoords(pieceToMove.To))
                    _piecesToMove.RemoveAt(x);
                else
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(pieceToMove.Piece.transform.position,
                        GetRealCoords(pieceToMove.To),
                        MoveSpeed * Time.deltaTime * pieceToMove.Distance);
            }
        }

        private static float GetRealCoord(int boardCoord)
        {
            return 0.6f * (boardCoord - 3.5f);
        }

        public Vector3 GetRealCoords(Position boardCoords)
        {
            return new Vector3(GetRealCoord(boardCoords.X), 0, GetRealCoord(boardCoords.Y)) + _origin;
        }

        private static sbyte GetBoardCoord(float num)
        {
            double coord = Math.Round(num / 0.6f + 3.5f);
            return (sbyte) Math.Clamp(coord, 0, Size - 1);
        }

        public Position GetBoardCoords(Vector3 realCoords)
        {
            realCoords -= _origin;
            return new Position(GetBoardCoord(realCoords.x), GetBoardCoord(realCoords.z));
        }
    }
}