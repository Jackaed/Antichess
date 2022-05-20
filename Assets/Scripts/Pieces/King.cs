using UnityEngine;

namespace Antichess.Pieces
{
    public class King : Piece
    {
        public King(bool isWhite) : base(isWhite) {}
        protected override GameObject BlackModel => ObjectLoader.Instance.bKing;
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKing;
    }
}
