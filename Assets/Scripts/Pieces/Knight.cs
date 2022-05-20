using UnityEngine;

namespace Antichess.Pieces
{
    public class Knight : Piece
    {
        public Knight(bool isWhite) : base(isWhite) {}
        protected override GameObject WhiteModel => ObjectLoader.Instance.wKnight;
        protected override GameObject BlackModel => ObjectLoader.Instance.bKnight;
    }
}
