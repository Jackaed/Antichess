using UnityEngine;

namespace Antichess.Pieces
{
    public class Pawn : IPiece
    {
        public Pawn(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        public GameObject Model => IsWhite ? ObjectLoader.Instance.wPawn : ObjectLoader.Instance.bPawn;
    }
}
