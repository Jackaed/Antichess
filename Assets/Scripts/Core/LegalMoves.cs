using System.Collections.Generic;

namespace Antichess.Core
{
    public class LegalMoves
    {
        private readonly Board _board;
        private readonly List<Move> _legalMoves;
        private readonly PieceLocations _pieceLocations;
        private bool _canEnPassant;

        private bool _canTake;
        private bool _isOutdated;

        public LegalMoves(Board board, PieceLocations pieceLocations)
        {
            _legalMoves = new List<Move>();
            _board = board;
            _pieceLocations = pieceLocations;
            _isOutdated = true;
        }

        public LegalMoves(LegalMoves other, PieceLocations pieceLocations, Board board)
        {
            _legalMoves = new List<Move>(other._legalMoves);
            _board = board;
            _pieceLocations = pieceLocations;
            _isOutdated = other._isOutdated;
        }

        public int Count => List.Count;

        public bool CanTake
        {
            get
            {
                if (_isOutdated)
                    Update();

                return _canTake;
            }
        }

        public bool CanEnPassant
        {
            get
            {
                if (_isOutdated)
                    Update();

                return _canEnPassant;
            }
        }

        public List<Move> List
        {
            get
            {
                if (_isOutdated)
                    Update();

                return new List<Move>(_legalMoves);
            }
        }

        public override string ToString()
        {
            return string.Join(", ", List);
        }

        public void OnBoardChange()
        {
            _isOutdated = true;
        }

        public bool IsLegal(Move move)
        {
            return List.Contains(move);
        }

        public void Add(Move move, bool canTake)
        {
            if (_board.PieceAt(move.From) == null) return;

            if (canTake != _canTake)
            {
                _legalMoves.Clear();
                _canTake = true;
            }

            if (move.Flag == Move.Flags.EnPassant) _canEnPassant = true;

            _legalMoves.Add(move);
        }

        private void Update()
        {
            _canTake = false;
            _canEnPassant = false;
            _isOutdated = false;
            _legalMoves.Clear();

            foreach (var pos in _pieceLocations.ColourPositions(_board.WhitesMove))
                _board.PieceAt(pos).AddMoves(pos, _board, this);
        }
    }
}