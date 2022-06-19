using Antichess.TargetSquares;
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

        protected bool Equals(Piece other)
        {
            return IsWhite == other.IsWhite;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Piece) obj);
        }

        public override int GetHashCode()
        {
            return IsWhite.GetHashCode();
        }

        public virtual void OnMove(BoardLogic board, Position pos, Move move) { }

        public virtual void AddMoves(Position pos, BoardLogic boardRef) { }
        
        
    }
}
