using UnityEngine;

public struct MovingPiece
{
    public Vector2Int To;
    public readonly GameObject Piece;

    public MovingPiece(Vector2Int to, GameObject piece)
    {
        To = to;
        Piece = piece;
    }
}