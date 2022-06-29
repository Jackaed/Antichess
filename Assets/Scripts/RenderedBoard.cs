using System.Collections.Generic;
using Antichess.Pieces;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess
{
    internal class RenderedBoard : Board
    {
        private readonly Dictionary<Piece, GameObject> _gameObjects = new();
        public readonly List<MovingPiece> PiecesToMove = new();
        
        protected override void AddPiece(Piece piece, Position pos)
        {
            if (PieceAt(pos) != null) RemovePiece(pos);
            base.AddPiece(piece, pos);
            if(piece != null)
                _gameObjects.Add(piece, Object.Instantiate(piece.Model,
                    Constants.GetRealCoords(pos), piece.Model.transform.rotation));
        }

        protected override void UnmakeMove(BoardStateChange move)
        {
            base.UnmakeMove(move);
            PiecesToMove.Add(new MovingPiece(move.From, _gameObjects[PieceAt(move.From)]));
        }

        protected override void MakeMoveWithoutLegalityCheck (Move move)
        {
            var pieceFrom = PieceAt(move.From);
            var pieceTo = PieceAt(move.To);
            
            base.MakeMoveWithoutLegalityCheck(move);

            if (_gameObjects.ContainsKey(pieceFrom))
                PiecesToMove.Add(new MovingPiece(move.To, _gameObjects[PieceAt(move.To)]));

            if (pieceTo == null) return;

            Object.Destroy(_gameObjects[pieceTo]);
            _gameObjects.Remove(pieceTo);
        }

        protected override void RemovePiece(Position pos)
        {
            Debug.Log(pos);
            if (_gameObjects.ContainsKey(PieceAt(pos))) Object.Destroy(_gameObjects[PieceAt(pos)]);
            base.RemovePiece(pos);
        }

        public override void OnNewFrame()
        {
            base.OnNewFrame();
            for (var x = 0; x < PiecesToMove.Count; x++)
            {
                var pieceToMove = PiecesToMove[x];
                var currentPos = pieceToMove.Piece.transform.position;
                if (currentPos != Constants.GetRealCoords(pieceToMove.To))
                    pieceToMove.Piece.transform.position = Vector3.MoveTowards(currentPos,
                        Constants.GetRealCoords(pieceToMove.To), 25 * Time.deltaTime);
                else PiecesToMove.RemoveAt(x);
            }
        }
    }
}
