using UnityEngine;

namespace Antichess.Pieces
{
    public class Bishop : IPiece
    {
        public Bishop(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        public GameObject Model => IsWhite ? ObjectLoader.Instance.wBishop : ObjectLoader.Instance.bBishop;
    }
}
