using UnityEngine;

namespace Antichess.Core.Pieces
{
    public interface IPieceData
    {
        public GameObject BlackModel { get; }

        public GameObject WhiteModel { get; }

        public uint Value { get; }

        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures);
    }
}