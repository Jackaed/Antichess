using Antichess.Core.Pieces;

namespace Antichess.Core
{
    public class Promotion : Move
    {
        public readonly Piece PromotionPiece;

        public Promotion(Position from, Position to, Piece promotionPiece) : base(from, to)
        {
            PromotionPiece = promotionPiece;
        }
    }
}