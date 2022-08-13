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
        private readonly Dictionary<Piece, GameObject> _gameObjects = new();
        private readonly List<Piece> _highlightedPieces = new();
        private readonly List<GameObject> _legalMoveIndicators = new();
        private readonly List<MovingPiece> _piecesToMove = new();

        protected override void Create(Piece piece, Position pos)
        {
            if (PieceAt(pos) != null) Destroy(pos);
            base.Create(piece, pos);
            _gameObjects.Add(piece, Object.Instantiate(piece.Model,
                ObjectLoader.GetRealCoords(pos), piece.Model.transform.rotation));
        }

        protected override void UndoMove(BoardChange change)
        {
            _piecesToMove.Add(new MovingPiece(change.Move.From, _gameObjects[PieceAt(change.Move.To)]));
            base.UndoMove(change);
        }

        private void CreateLegalMoveIndicator(Position pos)
        {
            if (PieceAt(pos) != null)
            {
                var highlightedPiece = PieceAt(pos);
                var highlightedGameObject = _gameObjects[highlightedPiece];
                var materialList = highlightedGameObject.GetComponent<MeshRenderer>().materials;
                materialList[1] = highlightedPiece.IsWhite
                    ? ObjectLoader.Instance.wEmissiveMaterial
                    : ObjectLoader.Instance.bEmissiveMaterial;
                highlightedGameObject.GetComponent<MeshRenderer>().materials = materialList;
                _highlightedPieces.Add(highlightedPiece);
            }
            else
            {
                var indicator = Object.Instantiate(ObjectLoader.Instance.legalMoveIndicator);
                indicator.transform.position = ObjectLoader.GetRealCoords(pos) + new Vector3(0, 0.0001f, 0);
                _legalMoveIndicators.Add(indicator);
            }
        }

        public void ClearLegalMoveIndicators()
        {
            foreach (var indicator in _legalMoveIndicators) Object.Destroy(indicator);

            foreach (var highlightedPiece in _highlightedPieces)
            {
                var highlightedGameObject = _gameObjects[highlightedPiece];
                var materialList = highlightedGameObject.GetComponent<MeshRenderer>().materials;
                materialList[1] = highlightedPiece.IsWhite
                    ? ObjectLoader.Instance.wBaseMaterial
                    : ObjectLoader.Instance.bBaseMaterial;
                highlightedGameObject.GetComponent<MeshRenderer>().materials = materialList;
            }

            _legalMoveIndicators.Clear();
            _highlightedPieces.Clear();
        }

        public void ShowLegalMovesFor(Position pos)
        {
            foreach (var move in LegalMoves.List.Where(move => move.From == pos)) CreateLegalMoveIndicator(move.To);
        }

        public void LowerPieceAt(Position pos)
        {
            var gamePos = _gameObjects[PieceAt(pos)].transform.position;
            _gameObjects[PieceAt(pos)].transform.position = new Vector3(gamePos.x, 0.0f, gamePos.z);
        }

        public void LiftPieceAt(Position pos)
        {
            var gamePos = _gameObjects[PieceAt(pos)].transform.position;
            _gameObjects[PieceAt(pos)].transform.position = new Vector3(gamePos.x, 0.25f, gamePos.z);
        }

        protected override void UpdateWinner()
        {
            base.UpdateWinner();
            switch (Winner)
            {
                case WinnerEnum.White:
                    Debug.Log("White Wins");
                    break;
                case WinnerEnum.Black:
                    Debug.Log("Black Wins");
                    break;
                case WinnerEnum.Stalemate:
                    Debug.Log("Stalemate");
                    break;
                case WinnerEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool Move(Move move)
        {
            var wasLegal = base.Move(move);
            if (wasLegal) return true;
            LowerPieceAt(move.From);
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

            var pieceFrom = PieceAt(move.From);
            var pieceTo = PieceAt(move.To);

            base.UnsafeMove(move);

            if (_gameObjects.ContainsKey(pieceFrom))
                _piecesToMove.Add(new MovingPiece(move.To, _gameObjects[PieceAt(move.To)]));

            if (pieceTo == null) return;

            Object.Destroy(_gameObjects[pieceTo]);
            _gameObjects.Remove(pieceTo);
        }

        protected override void Destroy(Position pos)
        {
            if (_gameObjects.ContainsKey(PieceAt(pos))) Object.Destroy(_gameObjects[PieceAt(pos)]);
            base.Destroy(pos);
        }

        public void OnNewFrame()
        {
            for (var x = 0; x < _piecesToMove.Count; x++)
            {
                var pieceToMove = _piecesToMove[x];

                if (pieceToMove.Piece == null ||
                    pieceToMove.Piece.transform.position == ObjectLoader.GetRealCoords(pieceToMove.To))
                    _piecesToMove.RemoveAt(x);
                else
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(pieceToMove.Piece.transform.position,
                        ObjectLoader.GetRealCoords(pieceToMove.To), 25 * Time.deltaTime);
            }
        }
    }
}