using System.Collections.Generic;
using Antichess.PositionTypes;
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

        public override uint Value => 1;
        protected override uint ColourlessIndex => 0;

        protected override GameObject WhiteModel => Constants.Instance.wPawn;
        protected override GameObject BlackModel => Constants.Instance.bPawn;

        private void AddMoveAndCheckForPromotion(Move move, bool canTake, Board boardRef,
            List<Move> legalMoves)
        {
            if (move.To.Y == (IsWhite ? Constants.BoardSize - 1 : 0))
                foreach (var piece in _promotionPieces)
                    Board.AddLegalMove(new Move(move.From, new PromotionPosition(move.To.X, move.To.Y, piece)),
                        boardRef, legalMoves, canTake);
            else
                Board.AddLegalMove(move, boardRef, legalMoves, canTake);
        }

        public override void AddMoves(Position pos, Board boardRef, List<Move> legalMoves)
        {
            var ahead = pos + Position.Ahead(IsWhite);
            if (ahead.Y > Constants.BoardSize) return;

            Position[] takesPositions = {ahead + new Position(1, 0), ahead + new Position(-1, 0)};

            foreach (var takesPosition in takesPositions)
                if (takesPosition.X < Constants.BoardSize)
                {
                    var target = boardRef.PieceAt(takesPosition);
                    if (target != null && target.IsWhite != IsWhite)
                    {
                        AddMoveAndCheckForPromotion(new Move(pos, takesPosition), true, boardRef, legalMoves);
                    }
                    else if (target == null && takesPosition == boardRef.EnPassantTargetSquare)
                    {
                        var enPassantTakesSquare = boardRef.EnPassantTargetSquare - Position.Ahead(IsWhite);

                        Board.AddLegalMove(new Move(pos, new EnPassantPosition(takesPosition, enPassantTakesSquare)),
                            boardRef, legalMoves, true);
                    }
                }

            if (boardRef.CanTake || boardRef.PieceAt(ahead) != null) return;

            AddMoveAndCheckForPromotion(new Move(pos, ahead), false, boardRef, legalMoves);

            var aheadAhead = new PawnDoubleMovePosition(
                ahead + Position.Ahead(IsWhite), ahead);

            if (pos.Y == (IsWhite ? 1 : Constants.BoardSize - 2) && boardRef.PieceAt(aheadAhead) == null)
                Board.AddLegalMove(new Move(pos, aheadAhead), boardRef, legalMoves, false);
        }
    }
}
