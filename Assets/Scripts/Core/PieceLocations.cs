using System.Collections.Generic;
using System.Linq;
using Antichess.Core.Pieces;
using JetBrains.Annotations;

namespace Antichess.Core
{
    public class PieceLocations
    {
        private readonly Board _board;
        public readonly List<Position> White, Black;

        public PieceLocations(Board board)
        {
            _board = board;
            White = new List<Position>();
            Black = new List<Position>();
        }

        public PieceLocations(PieceLocations other, Board board)
        {
            White = new List<Position>(other.White);
            Black = new List<Position>(other.Black);
            _board = board;
        }

        public List<Position> All => White.Concat(Black).ToList();

        public void Add(Position pos, [NotNull] Piece piece)
        {
            pos = pos.Regular;
            if (piece.IsWhite)
                White.Add(pos);
            else
                Black.Add(pos);
        }

        public void Remove(Position pos, Piece piece)
        {
            if (piece == null) return;

            pos = pos.Regular;
            if (piece.IsWhite)
                White.RemoveAll(r => r == pos);
            else
                Black.RemoveAll(r => r == pos);
        }

        public List<Position> ColourPositions(bool isWhite)
        {
            return isWhite ? White : Black;
        }
    }
}