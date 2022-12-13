using UnityEngine;

namespace Antichess.Core.Pieces
{
    public sealed class RenderedPiece : Piece
    {
        public GameObject GameObject;

        private RenderedPiece(bool isWhite, Types type, Vector3 pos) : base(isWhite, type)
        {
            GameObject = Object.Instantiate(Model);
            GameObject.transform.position = pos;
        }

        private GameObject Model => IsWhite ? PieceData.WhiteModel : PieceData.BlackModel;

        public static RenderedPiece ToRenderedPiece(Piece piece, Vector3 pos)
        {
            return new RenderedPiece(piece.IsWhite, piece.Type, pos);
        }
    }
}
