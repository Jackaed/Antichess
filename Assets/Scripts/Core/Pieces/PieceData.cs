using UnityEngine;

namespace Antichess.Core.Pieces
{
    /// <summary>
    /// An abstract data type which represents and stores all of the information pertaining to how
    /// a specific piece should be regarded in a given situation. Distinct from Piece, as this only
    /// defines how types of pieces act, rather than the data associated with each instance of a piece.
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
