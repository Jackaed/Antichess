using UnityEngine;

namespace Antichess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bQueen;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wQueen;
        
        public override ListMove GetMoves(Vector2Int pos, Board boardRef, bool canTake)
        {
            Vector2Int[] directions =
            {
                new (1, 1),
                new (1, -1),
                new (-1, 1),
                new (-1, -1),
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            return GenericMoveLogic.GetMovesInStraightDirection(pos, directions, boardRef, canTake);
        }
    }
}
