using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public abstract class Piece
    {
        protected Piece(bool isWhite)
        {
            IsWhite = isWhite;
        }

        public bool IsWhite { get; }

        protected abstract GameObject BlackModel { get; }
        protected abstract GameObject WhiteModel { get; }

        public GameObject Model => IsWhite ? WhiteModel : BlackModel;

        public virtual void AddMoves(Position pos, Board boardRef,
            Dictionary<Position, List<Position>> legalMoves) { }
    }
}
