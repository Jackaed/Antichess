using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class King : IPieceData
    {
        private static King _instance;

        private King()
        {
        }

        public static King Instance => _instance ??= new King();

        public GameObject BlackModel => ObjectLoader.Instance.bKing;
        public GameObject WhiteModel => ObjectLoader.Instance.wKing;
        public uint Value => 200;

        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures)
        {
            // The offsets of a King's potential movement options, from his current position "pos"
            Position[] offsets =
            {
                new(0, 1),
                new(1, 1),
                new(1, 0),
                new(1, -1),
                new(0, -1),
                new(-1, -1),
                new(-1, 0),
                new(-1, 1)
            };
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, offsets, boardRef, legalMoves, onlyCaptures);
        }

        public override string ToString()
        {
            return "King";
        }
    }
}