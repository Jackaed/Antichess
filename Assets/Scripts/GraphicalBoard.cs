using System.Collections.Generic;
using Antichess.Pieces;
using UnityEngine;

namespace Antichess
{
    internal class GraphicalBoard : Board 
    {
        private readonly Dictionary<IPiece, GameObject> _gameObjects = new();
        public readonly List<MovingPiece> PiecesToMove = new();
        
        protected override void AddPiece(IPiece piece, Vector2Int pos)
        {
            base.AddPiece(piece, pos);
            _gameObjects.Add(piece, Object.Instantiate(piece.Model,
                ObjectLoader.GetRealCoords(pos), piece.Model.transform.rotation));
        }

        public override bool MovePiece(Move move)
        {
            if (!base.MovePiece(move)) return false;
            if (_gameObjects.ContainsKey(PieceAt(move.From)))
                PiecesToMove.Add(new MovingPiece(move.To, _gameObjects[PieceAt(move.From)]));

            if (PieceAt(move.To) != null)
            {
                Object.Destroy(_gameObjects[PieceAt(move.To)]);
                _gameObjects.Remove(PieceAt(move.To));
            }

            return true;

        }
    }
}
