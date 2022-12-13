using System;

namespace Antichess.Core.Pieces
{
    public class Piece
    {
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

        private static readonly IPieceData[] PieceDataArray =
        {
            Pawn.Instance,
            Bishop.Instance,
            Knight.Instance,
            Rook.Instance,
            Queen.Instance,
            King.Instance
        };

        public Piece(bool isWhite, Types type)
        {
            IsWhite = isWhite;
            Type = type;
        }

        protected IPieceData PieceData => Type == Types.None ? null : PieceDataArray[(int)Type];

        public bool IsWhite { get; }

        public Types Type { get; }

        public uint Value => PieceData.Value;

        public uint Index => (uint)Type + (IsWhite ? 0u : 6u);

        public override string ToString()
        {
            return PieceData.ToString();
        }

        /// <summary>
        /// Adds a piece's legal move options to LegalMoves, when given the a reference to the board
        /// and the piece's position.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="boardRef"></param>
        /// <param name="legalMoves"></param>
        /// <param name="onlyCaptures"></param>
        public void AddLegalMoves(
            Position pos,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            PieceData.AddLegalMoves(pos, boardRef, legalMoves, onlyCaptures);
        }

        private bool Equals(Piece other)
        {
            return IsWhite == other.IsWhite && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((Piece)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsWhite, (int)Type);
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
