using System.Collections.Generic;
using Antichess.Pieces;
using UnityEngine;
using Antichess.TargetSquares;

namespace Antichess
{
    internal class RenderedBoardLogic : BoardLogic
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
            if (!base.MovePiece(move)) return false;

            if (_gameObjects.ContainsKey(pieceFrom))
                PiecesToMove.Add(new MovingPiece(move.To, _gameObjects[PieceAt(move.To)]));

            if (pieceTo == null) return true;

            Object.Destroy(_gameObjects[pieceTo]);
            _gameObjects.Remove(pieceTo);
            return true;
        }

        protected override void RemovePiece(Position pos)
        {
            Debug.Log(pos);
            if (_gameObjects.ContainsKey(PieceAt(pos)))
            {
                Object.Destroy(_gameObjects[PieceAt(pos)]);
            }
            base.RemovePiece(pos);
        }

        public override void OnNewFrame()
        {
            base.OnNewFrame();
            for (var x = 0; x < PiecesToMove.Count; x++)
            {
                var pieceToMove = PiecesToMove[x];
                var currentPos = pieceToMove.Piece.transform.position;
                if (currentPos != ObjectLoader.GetRealCoords(pieceToMove.To))
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(currentPos,
                        ObjectLoader.GetRealCoords(pieceToMove.To), 25 * Time.deltaTime);
                else PiecesToMove.RemoveAt(x);
            }
        } 
    }
}