using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public sealed class Rook : IPieceData
    {
        private static Rook _instance;

        private Rook() { }

        public static Rook Instance => _instance ??= new Rook();

        public GameObject BlackModel => ObjectLoader.Instance.bRook;
        public GameObject WhiteModel => ObjectLoader.Instance.wRook;
        public uint Value => 500;

        public void AddLegalMoves(
            Position pos,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            Position[] directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

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
            return "Rook";
        }
    }
}
