using System.Collections.Generic;
using Antichess.Pieces;
using UnityEngine;
using Antichess.TargetSquares;

namespace Antichess
{
    internal class GraphicalBoard : Board
    {
        private readonly Dictionary<Piece, GameObject> _gameObjects = new();
        public readonly List<MovingPiece> PiecesToMove = new();

        protected override void AddPiece(Piece piece, Position pos)
        {
            base.AddPiece(piece, pos);
            _gameObjects.Add(piece, Object.Instantiate(piece.Model,
                ObjectLoader.GetRealCoords(pos), piece.Model.transform.rotation));
        }

        public override bool MovePiece(Move move)
        {
            var pieceFrom = PieceAt(move.From);
            var pieceTo = PieceAt(move.To);
            Debug.Log(move);
            if (!base.MovePiece(move)) return false;

            if (_gameObjects.ContainsKey(pieceFrom))
                PiecesToMove.Add(new MovingPiece(move.To, _gameObjects[PieceAt(move.To)]));

            if (pieceTo == null) return true;

            Object.Destroy(_gameObjects[pieceTo]);
            _gameObjects.Remove(pieceTo);
            return true;
        }
    }
}