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

        public override void MovePiece(Move move)
        {
            if (_gameObjects.ContainsKey(Data[move.From.x, move.From.y]))
                PiecesToMove.Add(new MovingPiece(move.To, _gameObjects[Data[move.From.x, move.From.y]]));

            if (Data[move.To.x, move.To.y] != null)
            {
                Object.Destroy(_gameObjects[Data[move.To.x, move.To.y]]);
                _gameObjects.Remove(Data[move.To.x, move.To.y]);
            }

            base.MovePiece(move);
        }
    }
}
