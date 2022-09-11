using Antichess.Unity;
using UnityEngine;

namespace Antichess.Core.Pieces
{
    public class Knight : IPieceData
    {
        private static Knight _instance;

        private Knight()
        {
        }

        public static Knight Instance => _instance ??= new Knight();

        public GameObject WhiteModel => ObjectLoader.Instance.wKnight;
        public GameObject BlackModel => ObjectLoader.Instance.bKnight;
        public uint Value => 300;

        public void AddLegalMoves(Position pos, Board boardRef, LegalMoves legalMoves, bool onlyCaptures)
        {
            Position[] offsets =
            {
                new(2, 1),
                new(2, -1),
                new(1, 2),
                new(1, -2),
                new(-1, 2),
                new(-1, -2),
                new(-2, 1),
                new(-2, -1)
            };
            GenericMoveLogic.AddLegalMovesAtOffsets(pos, offsets, boardRef, legalMoves, onlyCaptures);
        }

        public override string ToString()
        {
            return "Knight";
        }
    }
}