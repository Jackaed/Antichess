using System.Collections.Generic;
using Antichess.Pieces;
using Antichess.PositionTypes;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace Antichess
{
    internal class RenderedBoard : Board
    {
        private readonly Dictionary<Piece, GameObject> _gameObjects = new();
        public readonly List<MovingPiece> PiecesToMove = new();
        
        protected override void Create(Piece piece, Position pos)
        {
            if (PieceAt(pos) != null) Destroy(pos);
            base.Create(piece, pos);
            _gameObjects.Add(piece, Object.Instantiate(piece.Model,
                Constants.GetRealCoords(pos), piece.Model.transform.rotation));
        }

        protected override void UndoMove(BoardChange change)
        {
            PiecesToMove.Add(new MovingPiece(change.Move.From, _gameObjects[PieceAt(change.Move.To)]));
            base.UndoMove(change);
        }

        protected override void UnsafeMove (Move move)
        {
            var pieceFrom = PieceAt(move.From);
            var pieceTo = PieceAt(move.To);
            
            base.UnsafeMove(move);

            if (_gameObjects.ContainsKey(pieceFrom))
                PiecesToMove.Add(new MovingPiece(move.To, _gameObjects[PieceAt(move.To)]));

            if (pieceTo == null) return;

            Object.Destroy(_gameObjects[pieceTo]);
            _gameObjects.Remove(pieceTo);
        }

        protected override void Destroy(Position pos)
        {
            Debug.Log(pos);
            if (_gameObjects.ContainsKey(PieceAt(pos))) Object.Destroy(_gameObjects[PieceAt(pos)]);
            base.Destroy(pos);
        }

        public override void OnNewFrame()
        {
            base.OnNewFrame();
            for (var x = 0; x < PiecesToMove.Count; x++)
            {
                var pieceToMove = PiecesToMove[x];

                if (pieceToMove.Piece == null ||
                    pieceToMove.Piece.transform.position == Constants.GetRealCoords(pieceToMove.To))
                {
                    PiecesToMove.RemoveAt(x);
                }
                else
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(pieceToMove.Piece.transform.position,
                        Constants.GetRealCoords(pieceToMove.To), 25 * Time.deltaTime);
            }
        }
    }
}
