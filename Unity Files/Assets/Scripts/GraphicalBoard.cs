using System.Collections.Generic;
using UnityEngine;

internal class GraphicalBoard : Board
{
    private readonly Dictionary<IPiece, GameObject> GameObjects = new();
    public List<MovingPiece> PiecesToMove = new();

    public override void AddPiece(IPiece piece, Vector2Int pos)
    {
        base.AddPiece(piece, pos);
        GameObjects.Add(piece, Object.Instantiate(piece.Model,
            ObjectLoader.GetRealCoords(pos), piece.Model.transform.rotation));
    }

    public override void MovePiece(Vector2Int from, Vector2Int to)
    {
        if (GameObjects.ContainsKey(Data[from.x, from.y]))
            PiecesToMove.Add(new MovingPiece(to, GameObjects[Data[from.x, from.y]]));

        if (Data[to.x, to.y] != null)
        {
            Object.Destroy(GameObjects[Data[to.x, to.y]]);
            GameObjects.Remove(Data[to.x, to.y]);
        }

        base.MovePiece(from, to);
    }
}