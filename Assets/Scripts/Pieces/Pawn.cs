using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Pawn : Piece
    {
        private readonly Piece[] _promotionPieces;

        public Pawn(bool isWhite) : base(isWhite)
        {
            _promotionPieces = new Piece[]
            {
                new Bishop(IsWhite),
                new Queen(IsWhite),
                new Rook(IsWhite),
                new Knight(IsWhite)
            };
        }
        protected override GameObject WhiteModel => ObjectLoader.Instance.wPawn;
        protected override GameObject BlackModel => ObjectLoader.Instance.bPawn;

        public override void AddMoves(Position pos, BoardLogic boardRef)
        {
            var ahead = pos + new Vector2Int(0, IsWhite ? 1 : -1);
            if (ahead.y > BoardLogic.Size.y) return;

            Position[] takesPositions = {ahead + new Position(1, 0), ahead + new Vector2Int(-1, 0)};

            foreach (var takesPosition in takesPositions)
                if (takesPosition.x < BoardLogic.Size.x)
                {
                    var target = boardRef.PieceAt(takesPosition);
                    if (target != null && target.IsWhite != IsWhite)
                    {
                        boardRef.CanTake = true;
                        boardRef.AddLegalMove(new Move(pos, takesPosition));
                    }
                    else if (target == null && takesPosition == boardRef.EnPassantTargettableSquare)
                    {
                        boardRef.CanTake = true;
                        var enPassantTakesSquare = boardRef.EnPassantTargettableSquare
                                                   - new Vector2Int(0, IsWhite ? 1 : -1);

                        boardRef.AddLegalMove(new Move(pos,
                            new EnPassantPosition(takesPosition, enPassantTakesSquare)));
                    }
                }

            if (boardRef.CanTake || boardRef.PieceAt(ahead) != null) return;

            if (ahead.y == (IsWhite ? BoardLogic.Size.y - 1 : 0))
            {
                foreach (Piece piece in _promotionPieces)
                {
                    boardRef.AddLegalMove(new Move(pos, new PromotionPosition(ahead.x, ahead.y, piece)));
                }
            }
            else
            {
                boardRef.AddLegalMove(new Move(pos, ahead));
            }
           
            var aheadAhead = new PawnDoubleMovePosition(
                ahead + new Vector2Int(0, IsWhite ? 1 : -1), ahead);
            if (pos.y == (IsWhite ? 1 : BoardLogic.Size.y - 2) && boardRef.PieceAt(aheadAhead) == null)
                boardRef.AddLegalMove(new Move(pos, aheadAhead));
        }
    }
}
