using System;

namespace Antichess.PositionTypes
{
    // This facilitates En Passant, by adding an extra "target square", containing the location of the pawn to be taken
    // if the move that this position is attatched to is made.
    public class EnPassantPosition : Position
    {
        public readonly Position TargetPieceSquare;

        public EnPassantPosition(Position to, Position targetPieceSquare) : base(to.X, to.Y)
        {
            TargetPieceSquare = targetPieceSquare;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EnPassantPosition;
            return other != null && base.Equals(obj) && TargetPieceSquare == other.TargetPieceSquare;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), TargetPieceSquare);
        }

        public static bool operator ==(EnPassantPosition left, EnPassantPosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EnPassantPosition left, EnPassantPosition right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return base.ToString() + " taking on " + TargetPieceSquare;
        }
    }
}
