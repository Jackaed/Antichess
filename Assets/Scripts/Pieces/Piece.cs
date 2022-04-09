using UnityEngine;

namespace Antichess.Pieces
{
    public interface IPiece
    {
        public bool IsWhite { get; }
        public GameObject Model { get; }
    }
}
