using System;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Piece
    {
        private static readonly IPieceData[] PieceDataArray = new IPieceData[]
            {Pawn.Instance, Bishop.Instance, Knight.Instance, Rook.Instance, Queen.Instance, King.Instance};
        
        public Piece(bool isWhite, Types type)
        {
            IsWhite = isWhite;
            Type = type;
        }

        public enum Types : byte
        {
            Pawn,
            Bishop,
            Knight,
            Rook,
            Queen,
            King,
            None
        }

        protected IPieceData PieceData => Type == Types.None ? null : PieceDataArray[(int) Type];

        public bool IsWhite { get; }
        
        public Types Type { get; }

        public uint Value => PieceData.Value;

        public override string ToString()
        {
            return PieceData.ToString();
        }

        // Adds a piece's legal move options to LegalMoves, when given the a reference to the board and the piece's
        // position.
        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures)
        {
            PieceData.AddLegalMoves(pos, boardRef, legalMoves, onlyCaptures);
        }

        private bool Equals(Piece other)
        {
            return IsWhite == other.IsWhite && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Piece) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsWhite, (int) Type);
        }

        public static bool operator ==(Piece left, Piece right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Piece left, Piece right)
        {
            return !Equals(left, right);
        }
    }
}