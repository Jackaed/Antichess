using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    /// <summary>
    /// The PieceData implementation for a bishop. Defines how a bishop can move and generally acts
    /// in the game.
    /// </summary>
    public sealed class Queen : IPieceData
    {
        private static Queen _instance;

        private Queen() { }

        public static Queen Instance => _instance ??= new Queen();

        public GameObject BlackModel => ObjectLoader.Instance.bQueen;
        public GameObject WhiteModel => ObjectLoader.Instance.wQueen;
        public uint Value => 900;

        public void AddLegalMoves(
            Position pos,
            Board boardRef,
            LegalMoves legalMoves,
            bool onlyCaptures
        )
        {
            Position[] directions =
            {
                new(1, 1),
                new(1, -1),
                new(-1, 1),
                new(-1, -1),
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

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
            return "Queen";
        }
    }
}
