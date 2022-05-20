using UnityEngine;

namespace Antichess.Pieces
{
    public class Queen : Piece
    {
        public Queen(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bQueen;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wQueen;
    }
}
