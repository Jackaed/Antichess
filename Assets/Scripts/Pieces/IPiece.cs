using System.Collections.Generic;
using UnityEngine;

namespace Antichess.Pieces
{
    public interface IPiece
    {
        public bool IsWhite { get; }
        public GameObject Model { get; }

        public List<Move> GetLegalMoves(Vector2Int pos, Board boardRef)
        {
            return new List<Move>();
        }
    
        public bool IsLegal(Move move)
        {
            return true;
        }
    }
}
