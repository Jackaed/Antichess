using System.Collections.Generic;
using UnityEngine;

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
        if (_gameObjects.ContainsKey(Data[move.from.x, move.from.y]))
            PiecesToMove.Add(new MovingPiece(move.to, _gameObjects[Data[move.from.x, move.from.y]]));

        if (Data[move.to.x, move.to.y] != null)
        {
            Object.Destroy(_gameObjects[Data[move.to.x, move.to.y]]);
            _gameObjects.Remove(Data[move.to.x, move.to.y]);
        }

        base.MovePiece(move);
    }
}
