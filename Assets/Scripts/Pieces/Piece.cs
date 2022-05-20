using System.Collections.Generic;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Piece
    {
        protected Piece(bool isWhite)
        {
            IsWhite = isWhite;
        }
        public bool IsWhite { get; protected set; }

        protected virtual GameObject BlackModel => null;
        protected virtual GameObject WhiteModel => null;

        public GameObject Model => IsWhite ? WhiteModel : BlackModel;

        public virtual List<Move> GetMoves(Vector2Int pos, Board boardRef)
        {
            return new List<Move>();
        }
    }
}
