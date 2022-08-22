using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Rook : IPieceData
    {
        private Rook() { }

        public GameObject BlackModel => ObjectLoader.Instance.bRook;
        public GameObject WhiteModel => ObjectLoader.Instance.wRook;
        public uint Value => 5;

        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures)
        {
            Position[] directions =
            {
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef, legalMoves, onlyCaptures);
        }
        
        public override string ToString() => "Rook";
        
        private static Rook _instance = null;
        
        public static Rook Instance => _instance ??= new Rook();
    }
}