using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Bishop : IPieceData
    {
        private Bishop() {}
        
        public GameObject BlackModel => ObjectLoader.Instance.bBishop;
        public GameObject WhiteModel => ObjectLoader.Instance.wBishop;
        
        public uint Value => 3;

        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures)
        {
            Position[] directions =
            {
                new(1, 1),
                new(1, -1),
                new(-1, 1),
                new(-1, -1)
            };
            GenericMoveLogic.AddLegalMovesInDirections(pos, directions, boardRef, legalMoves, onlyCaptures);
        }
        
        public override string ToString() => "Bishop";
        
        private static Bishop _instance = null;
        
        public static Bishop Instance => _instance ??= new Bishop();
    }
}