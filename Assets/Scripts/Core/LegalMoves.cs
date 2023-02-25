using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Antichess.Core
{
    /// <summary>
    /// A self-sustaining Legal Move collection, that updates itself if and when required, such as
    /// when a move is made, or more generally the board changes state.
    /// </summary>
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
            get { return Bag.ToList(); }
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

        public List<Move> OrderedMoves(Move refutationMove)
        {
            var list = List;
            var tagged = list.Select((item, i) => new { Item = item, Index = (int?)i });
            var index = (
                from pair in tagged
                where pair.Item == refutationMove
                select pair.Index
            ).FirstOrDefault();
            if (index == null)
                return list;
            var temp = list[0];
            list[0] = list[(int)index];
            return list;
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
            if (_board.PieceAt(move.From) == null)
                return;

            if (move.Flag == Move.Flags.EnPassant)
                _canEnPassant = true;

            _legalMoves.Add(move);
        }

        /// <summary>
        /// Updates the legal moves to reflect the legal moves of those in the current position.
        /// Works by retrieving the legal moves from each of the pieces on the board (of the colour
        /// of the current player’s turn to move) in parallel, and adding them to the _legalMoves
        /// concurrent bag.
        /// </summary>
        private void Update()
        {
            _canTake = false;
            _canEnPassant = false;
            _isOutdated = false;
            _legalMoves.Clear();

            // Find all of the possible captures, and if none are found, then search for non-capture
            // moves.
            Parallel.ForEach(
                _pieceLocations.ColourPositions(_board.WhitesMove),
                pos => _board.PieceAt(pos).AddLegalMoves(pos, _board, this, true)
            );

            if (_legalMoves.Count == 0)
            {
                Parallel.ForEach(
                    _pieceLocations.ColourPositions(_board.WhitesMove),
                    pos => _board.PieceAt(pos).AddLegalMoves(pos, _board, this, false)
                );
            }
        }
    }
}
