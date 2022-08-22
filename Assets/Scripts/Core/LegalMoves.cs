using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Antichess.Core
{
    // A self-sustaining Legal Move collection, that updates itself if required.
    public class LegalMoves
    {
        private readonly Board _board;
        private readonly ConcurrentBag<Move> _legalMoves;
        private readonly PieceLocations _pieceLocations;
        private bool _canEnPassant;

        private bool _canTake;
        private bool _isOutdated;

        public LegalMoves(Board board, PieceLocations pieceLocations)
        {
            _legalMoves = new ConcurrentBag<Move>();
            _board = board;
            _pieceLocations = pieceLocations;
            _isOutdated = true;
        }

        public LegalMoves(LegalMoves other, PieceLocations pieceLocations, Board board)
        {
            _legalMoves = new ConcurrentBag<Move>(other._legalMoves);
            _board = board;
            _pieceLocations = pieceLocations;
            _isOutdated = other._isOutdated;
        }

        public int Count => Bag.Count;

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

                return _legalMoves.ToList();
            }
        }

        private ConcurrentBag<Move> Bag
        {
            get
            {
                if (_isOutdated)
                    Update();
                return _legalMoves;
            }
        }

        public override string ToString()
        {
            return string.Join(", ", Bag);
        }

        public void OnBoardChange()
        {
            _isOutdated = true;
        }

        public bool IsLegal(Move move)
        {
            return Bag.Contains(move);
        }

        public void Add(Move move)
        {
            if (_board.PieceAt(move.From) == null) return;

            if (move.Flag == Move.Flags.EnPassant) _canEnPassant = true;

            _legalMoves.Add(move);
        }

        // Updates the _legalMoves collection to represent the legal moves in the current position in the board.
        private void Update()
        {
            _canTake = false;
            _canEnPassant = false;
            _isOutdated = false;
            _legalMoves.Clear();

            // Find all of the possible captures, and if none are found, then search for non-capture moves.
            Parallel.ForEach(_pieceLocations.ColourPositions(_board.WhitesMove), pos =>
            {
                _board.PieceAt(pos).AddLegalMoves(pos, _board, this, true);
            });

            if (_legalMoves.Count == 0)
            {
                Parallel.ForEach(_pieceLocations.ColourPositions(_board.WhitesMove), pos =>
                {
                    _board.PieceAt(pos).AddLegalMoves(pos, _board, this, false);
                });
            }
        }
    }
}