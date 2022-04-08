using UnityEngine;

public class Knight : IPiece
{
    public Knight(bool isWhite)
    {
        IsWhite = isWhite;
    }

    public bool IsWhite { get; }

    public GameObject Model => IsWhite ? ObjectLoader.Instance.wKnight : ObjectLoader.Instance.bKnight;
}