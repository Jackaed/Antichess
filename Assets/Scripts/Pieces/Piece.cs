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

        // The visual representation of the piece is stored in Unity as a "GameObject"
        public GameObject Model => IsWhite ? WhiteModel : BlackModel;

        public abstract uint Value { get; }
        
        // Adds a piece's legal move options to LegalMoves, when given the a reference to the board and the piece's
        // position.
        public virtual void AddMoves(Position pos, Board boardRef,
            Dictionary<Position, List<Position>> legalMoves) { }
    }
}
