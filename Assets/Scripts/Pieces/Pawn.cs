using System.Collections.Generic;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Pawn : Piece
    {
        private readonly Piece[] _promotionPieces;
        public override uint Value => 1;

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

        protected override GameObject WhiteModel => Constants.Instance.wPawn;
        protected override GameObject BlackModel => Constants.Instance.bPawn;

        public override void AddMoves(Position pos, Board boardRef, Dictionary<Position, List<Position>> legalMoves)
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
                        Board.AddLegalMove(new Move(pos, takesPosition), boardRef, legalMoves, true);
                    }
                    else if (target == null && takesPosition == boardRef.EnPassantTargetSquare)
                    {
                        var enPassantTakesSquare = boardRef.EnPassantTargetSquare - Position.Ahead(IsWhite);

                        Board.AddLegalMove(new Move(pos, new EnPassantPosition(takesPosition, enPassantTakesSquare)),
                            boardRef, legalMoves, true);
                    }
                }

            if (boardRef.CanTake || boardRef.PieceAt(ahead) != null) return;

            if (ahead.Y == (IsWhite ? Constants.BoardSize - 1 : 0))
                foreach (var piece in _promotionPieces)
                    Board.AddLegalMove(new Move(pos, new PromotionPosition(ahead.X, ahead.Y, piece)), boardRef,
                        legalMoves, false);
            else
                Board.AddLegalMove(new Move(pos, ahead), boardRef, legalMoves, false);

            var aheadAhead = new PawnDoubleMovePosition(
                ahead + Position.Ahead(IsWhite), ahead);

            if (pos.Y == (IsWhite ? 1 : Constants.BoardSize - 2) && boardRef.PieceAt(aheadAhead) == null)
                Board.AddLegalMove(new Move(pos, aheadAhead), boardRef, legalMoves, false);
        }
    }
}
