namespace Antichess.TargetSquares
{
    public class EnPassantPosition : Position
    {
        public EnPassantPosition (byte x, byte y) : base (x, y) {}
        
        public Position TargetPieceSquare;
    }
}