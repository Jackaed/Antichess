using UnityEngine;

public struct Move
{
    public Vector2Int from, to;

    public Move(Vector2Int from, Vector2Int to)
    {
        this.from = from;
        this.to = to;
    }
}
