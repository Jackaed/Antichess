using UnityEngine;

namespace Antichess.Pieces
{
    public class Rook : IPiece
    {
        public Rook(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        public GameObject Model => IsWhite ? ObjectLoader.Instance.wRook : ObjectLoader.Instance.bRook;
    }
}
