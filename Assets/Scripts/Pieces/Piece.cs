using UnityEngine;

namespace Antichess.Pieces
{
    public class Piece
    {
        protected Piece(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        protected virtual GameObject BlackModel => null;
        protected virtual GameObject WhiteModel => null;

        public GameObject Model => IsWhite ? WhiteModel : BlackModel;

        public virtual ListMove GetMoves(Vector2Int pos, Board boardRef, bool canTake)
        {
            return new ListMove();
        }
    }
}