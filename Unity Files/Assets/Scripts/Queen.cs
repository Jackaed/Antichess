using UnityEngine;

public class Queen : IPiece
{
    public Queen(bool isWhite)
    {
        IsWhite = isWhite;
    }

    public bool IsWhite { get; }
    public GameObject Model => IsWhite ? ObjectLoader.Instance.wQueen : ObjectLoader.Instance.bQueen;
}
