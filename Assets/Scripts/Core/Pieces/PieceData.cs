using UnityEngine;

namespace Antichess.Core.Pieces
{
    /// <summary>
    /// Represents how a type of piece can act in a given situation, and how it generally acts.
    /// </summary>
    public interface IPieceData
    {
        public GameObject BlackModel { get; }

        public GameObject WhiteModel { get; }

        public uint Value { get; }

        public void AddLegalMoves(
            Position pos,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        );
    }
}
