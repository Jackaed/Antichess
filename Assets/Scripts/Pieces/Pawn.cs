using System.Collections.Generic;
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

        public override void AddMoves(Position pos, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
        {
            var ahead = pos + new Vector2Int(0, IsWhite ? 1 : -1);
            if (ahead.y > Board.Size.y) return;

            Position[] takesPositions = {ahead + new Position(1, 0), ahead + new Vector2Int(-1, 0)};

            foreach (var takesPosition in takesPositions)
                if (takesPosition.x < Board.Size.x)
                {
                    var target = boardRef.PieceAt(takesPosition);
                    if (target != null && target.IsWhite != IsWhite)
                    {
                        boardRef.CanTake = true;
                        Board.AddLegalMove(new Move(pos, takesPosition), boardRef, legalMoves);
                    }
                    else if (target == null && takesPosition == boardRef.EnPassantTargettableSquare)
                    {
                        boardRef.CanTake = true;
                        var enPassantTakesSquare = boardRef.EnPassantTargettableSquare
                                                   - new Vector2Int(0, IsWhite ? 1 : -1);

                        Board.AddLegalMove(new Move(pos,
                            new EnPassantPosition(takesPosition, enPassantTakesSquare)), boardRef, legalMoves);
                    }
                }

            if (boardRef.CanTake || boardRef.PieceAt(ahead) != null) return;

            if (ahead.y == (IsWhite ? Board.Size.y - 1 : 0))
                foreach (var piece in _promotionPieces)
                    Board.AddLegalMove(new Move(pos, new PromotionPosition(ahead.x, ahead.y, piece)), boardRef,
                        legalMoves);
            else
                Board.AddLegalMove(new Move(pos, ahead), boardRef, legalMoves);

            var aheadAhead = new PawnDoubleMovePosition(
                ahead + new Vector2Int(0, IsWhite ? 1 : -1), ahead);
            if (pos.y == (IsWhite ? 1 : Board.Size.y - 2) && boardRef.PieceAt(aheadAhead) == null)
                Board.AddLegalMove(new Move(pos, aheadAhead), boardRef, legalMoves);
        }
    }
}
