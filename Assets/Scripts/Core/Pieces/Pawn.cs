using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
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

        protected override GameObject WhiteModel => ObjectLoader.Instance.wPawn;
        protected override GameObject BlackModel => ObjectLoader.Instance.bPawn;

        private void AddMoveAndCheckForPromotion(Move move, bool canTake, Board boardRef,
            LegalMoves legalMoves)
        {
            if (move.To.Y == (IsWhite ? ObjectLoader.BoardSize - 1 : 0))
                foreach (var piece in _promotionPieces)
                    legalMoves.Add(new Promotion(move.From, move.To, piece),
                        canTake);
            else
                legalMoves.Add(move, canTake);
        }

        public override void AddMoves(Position pos, Board boardRef, LegalMoves legalMoves)
        {
            var ahead = pos + Position.Ahead(IsWhite);
            if (ahead.Y > ObjectLoader.BoardSize) return;

            Position[] takesPositions = {ahead + new Position(1, 0), ahead + new Position(-1, 0)};

            foreach (var takesPosition in takesPositions)
                if (takesPosition.X < ObjectLoader.BoardSize)
                {
                    var target = boardRef.PieceAt(takesPosition);
                    if (target != null && target.IsWhite != IsWhite)
                    {
                        AddMoveAndCheckForPromotion(new Move(pos, takesPosition), true, boardRef, legalMoves);
                    }
                    else if (target == null && takesPosition == boardRef.EnPassantTargetSquare)
                    {
                        var enPassantTakesSquare = boardRef.EnPassantTargetSquare - Position.Ahead(IsWhite);

                        legalMoves.Add(new Move(pos, takesPosition, Move.Flags.EnPassant),
                            true);
                    }
                }

            if (legalMoves.CanTake || boardRef.PieceAt(ahead) != null) return;

            AddMoveAndCheckForPromotion(new Move(pos, ahead), false, boardRef, legalMoves);

            var aheadAhead = ahead + Position.Ahead(IsWhite);

            if (pos.Y == (IsWhite ? 1 : ObjectLoader.BoardSize - 2) && boardRef.PieceAt(aheadAhead) == null)
                legalMoves.Add(new Move(pos, aheadAhead, Move.Flags.PawnDoubleMove), false);
        }
    }
}