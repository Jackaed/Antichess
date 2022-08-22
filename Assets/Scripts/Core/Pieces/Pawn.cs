using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Pawn : IPieceData
    {
        private static readonly Piece.Types[] PromotionPieces = new[]
        {
            Piece.Types.Bishop,
            Piece.Types.King,
            Piece.Types.Knight,
            Piece.Types.Pawn,
            Piece.Types.Rook,
            Piece.Types.Queen
        };

        private Pawn() { }

        public uint Value => 1;

        public GameObject WhiteModel => ObjectLoader.Instance.wPawn;
        public GameObject BlackModel => ObjectLoader.Instance.bPawn;

        private void AddMoveAndCheckForPromotion(Move move, Board boardRef, LegalMoves legalMoves)
        {
            var isWhite = boardRef.PieceAt(move.From).IsWhite;
            if (move.To.Y == (isWhite ? ObjectLoader.BoardSize - 1 : 0))
                foreach (var piece in PromotionPieces)
                    legalMoves.Add(new Promotion(move.From, move.To, new Piece(isWhite, piece)));
            else
                legalMoves.Add(move);
        }

        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures)
        {
            var isWhite = boardRef.PieceAt(pos).IsWhite;
            var ahead = pos + Position.Ahead(isWhite);
            if (ahead.Y > ObjectLoader.BoardSize) return;

            Position[] takesPositions = {ahead + new Position(1, 0), ahead + new Position(-1, 0)};

            foreach (var takesPosition in takesPositions)
                if (takesPosition.X < ObjectLoader.BoardSize)
                {
                    var target = boardRef.PieceAt(takesPosition);
                    if (target != null && target.IsWhite != isWhite)
                    {
                        AddMoveAndCheckForPromotion(new Move(pos, takesPosition), boardRef, legalMoves);
                    }
                    else if (target == null && takesPosition == boardRef.EnPassantTargetSquare)
                    {
                        var enPassantTakesSquare = boardRef.EnPassantTargetSquare - Position.Ahead(isWhite);

                        legalMoves.Add(new Move(pos, takesPosition, Move.Flags.EnPassant));
                    }
                }

            if (onlyCaptures || boardRef.PieceAt(ahead) != null) return;

            AddMoveAndCheckForPromotion(new Move(pos, ahead), boardRef, legalMoves);

            var aheadAhead = ahead + Position.Ahead(isWhite);

            if (pos.Y == (isWhite ? 1 : ObjectLoader.BoardSize - 2) && boardRef.PieceAt(aheadAhead) == null)
                legalMoves.Add(new Move(pos, aheadAhead, Move.Flags.PawnDoubleMove));
        }
        
        public override string ToString() => "Pawn";
        
        private static Pawn _instance = null;
        
        public static Pawn Instance => _instance ??= new Pawn();
    }
}