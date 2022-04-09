using UnityEngine;

public class King : IPiece
{
    public King(bool isWhite)
    {
        IsWhite = isWhite;
    }

    public bool IsWhite { get; }

    public GameObject Model => IsWhite ? ObjectLoader.Instance.wKing : ObjectLoader.Instance.bKing;
}
