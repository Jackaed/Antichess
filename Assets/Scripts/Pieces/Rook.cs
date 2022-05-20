using System.Collections.Generic;
using UnityEngine;

namespace Antichess.Pieces
{
    public class Rook : Piece
    {
        public Rook(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bRook;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wRook;
        private List<Move> MovesInDir(Vector2Int pos, Vector2Int increments, Board boardRef)
        {
            List<Move> moveList = new();
            Vector2Int to = new Vector2Int(pos.x, pos.y) + increments;
            while (to.x < Board.Size.x && to.x >= 0 && to.y < Board.Size.y && to.y >= 0)
            {
                if (boardRef.PieceAt(to) == null)
                {
                    moveList.Add(new Move(pos, to));
                }
                
                else if (boardRef.PieceAt(to).IsWhite != IsWhite)
                {
                    moveList.Add(new Move(pos, to));
                    break;
                }

                else
                {
                    break;
                }
                
                to += increments;
            }
            
            return moveList;
        }

        public override List<Move> GetMoves(Vector2Int pos, Board boardRef)
        {
            List<Move> moveList = new();
            Vector2Int[] directions =
            {
                new (0, 1),
                new (1, 0),
                new (-1, 0),
                new (0, -1)
            };
            
            foreach(var direction in directions)
            {
                moveList.AddRange(MovesInDir(pos, direction, boardRef));
            }
            
            return moveList;
        }
    }
}
