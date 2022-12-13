using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public sealed class Bishop : IPieceData
    {
        private static Bishop _instance;

        private Bishop() { }

        public static Bishop Instance => _instance ??= new Bishop();

        public GameObject BlackModel => ObjectLoader.Instance.bBishop;
        public GameObject WhiteModel => ObjectLoader.Instance.wBishop;

        public uint Value => 300;

        public void AddLegalMoves(
            Position pos,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            Position[] directions = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
            GenericMoveLogic.AddLegalMovesInDirections(
                pos,
                directions,
                boardRef,
                legalMoves,
                onlyCaptures
            );
        }

        public override string ToString()
        {
            return "Bishop";
        }
    }
}
