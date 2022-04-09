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

        public override void MovePiece(Vector2Int from, Vector2Int to)
        {
            if (_gameObjects.ContainsKey(Data[from.x, from.y]))
                PiecesToMove.Add(new MovingPiece(to, _gameObjects[Data[from.x, from.y]]));

            if (Data[to.x, to.y] != null)
            {
                Object.Destroy(_gameObjects[Data[to.x, to.y]]);
                _gameObjects.Remove(Data[to.x, to.y]);
            }

            base.MovePiece(from, to);
        }
    }
}
