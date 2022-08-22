using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class RenderedPiece : Piece
    {
        public GameObject GameObject;
        
        public RenderedPiece(bool isWhite, Piece.Types type, Position pos) : base(isWhite, type)
        {
            GameObject = Object.Instantiate(Model);
            GameObject.transform.position = ObjectLoader.GetRealCoords(pos);
        }

        public static RenderedPiece ToRenderedPiece(Piece piece, Position pos)
        {
            return new RenderedPiece(piece.IsWhite, piece.Type, pos);
        }

        private GameObject Model => IsWhite ? PieceData.WhiteModel : PieceData.BlackModel;
    }
}