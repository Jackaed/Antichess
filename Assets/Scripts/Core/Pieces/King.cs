using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class King : IPieceData
    {
        private King() { }

        public GameObject BlackModel => ObjectLoader.Instance.bKing;
        public GameObject WhiteModel => ObjectLoader.Instance.wKing;
        public uint Value => 2;

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
        public override string ToString() => "King";

        private static King _instance = null;
        public static King Instance => _instance ??= new King();
    }
}