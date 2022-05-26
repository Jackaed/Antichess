using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(bool isWhite) : base(isWhite) {}
        protected override GameObject WhiteModel => ObjectLoader.Instance.wPawn;
        protected override GameObject BlackModel => ObjectLoader.Instance.bPawn;

        public override void AddMoves(Position pos, Board boardRef)
        {
            var ahead = pos + new Vector2Int(0,IsWhite ? 1 : -1);
            if (ahead.y > Board.Size.y) return;

            Position[] takesPositions = {ahead + new Position(1, 0), ahead + new Vector2Int(-1, 0)};
            
            foreach (var takesPosition in takesPositions)
                if (takesPosition.x > 0 && takesPosition.x < Board.Size.x && boardRef.PieceAt(takesPosition) != null &&
                    boardRef.PieceAt(takesPosition).IsWhite != IsWhite)
                {
                    boardRef.CanTake = true;
                    boardRef.AddLegalMove(new Move(pos, takesPosition));
                }

            if (boardRef.CanTake || boardRef.PieceAt(ahead) != null) return;
            
            boardRef.AddLegalMove(new Move(pos, ahead));
            var aheadAhead = ahead + new Vector2Int(0, IsWhite ? 1 : -1);
            if (pos.y == (IsWhite ? 1 : Board.Size.y - 2) && boardRef.PieceAt(aheadAhead) == null)
                boardRef.AddLegalMove(new Move(pos, aheadAhead));
        }
    }
}