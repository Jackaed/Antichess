using System.Linq;
using Antichess.Core;

namespace Antichess.Evaluation
{
    /// <summary>
    /// Represents a transposition table, which is a hash table of previously evaluated positions in
    /// a chess board, storing the evaluation that position was assigned. Improves the speed of the
    /// AI evaluation, by preventing it from re-evaluating positions that it has already reached
    /// through a different move order.
    ///
    /// Note that this does not store all previous evaluations, and many get overwritten throughout
    /// the progress of a game. However, the transposition table still gets thousands of successful
    /// reads throughout an evaluation.
    /// </summary>
    public class TranspositionTable
    {
        private readonly ulong _size;
        private readonly Entry[] _table;
        private readonly Board _board;

        public TranspositionTable(ulong size)
        {
            _size = size;
            _table = Enumerable.Repeat(Entry.NotEvaluated, (int)_size).ToArray();
        }

        /// <summary>
        /// Gets the given entry with a specific zobrist key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Entry Lookup(ulong key)
        {
            var val = _table[key % _size];
            return val.Key == key ? val : Entry.NotEvaluated;
        }

        /// <summary>
        /// Creates and stores an entry at a given zobrist key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="score"></param>
        /// <param name="wasMate"></param>
        /// <param name="depth"></param>
        /// <param name="nodeType"></param>
        /// <param name="refutationMove"></param>
        public void Store(
            ulong key,
            int score,
            bool wasMate,
            ushort depth,
            NodeType nodeType,
            Move refutationMove
        )
        {
            _table[key % _size] = new Entry(key, score, wasMate, depth, nodeType, refutationMove);
        }

        /// <summary>
        /// Represents the data stored in a transposition table entry. This does not just store the
        /// score that was assigned in the evaluation, but also the depth that was reached when this
        /// score was assigned (allowing us to improve previous searches), whether the position
        /// resulted in a checkmate, what the best move turned out to be in this position, and
        /// whether this position was pruned in Alpha-Beta pruning or not.
        /// </summary>
        public readonly struct Entry
        {
            public static readonly Entry NotEvaluated =
                new(ulong.MinValue, int.MinValue, false, 0, NodeType.NotEvaluated, null);

            public readonly ushort Depth;
            public readonly ulong Key;
            public readonly int Score;
            public readonly bool WasMate;
            public readonly NodeType TtNodeType;
            public readonly Move RefutationMove;

            public Entry(
                ulong key,
                int score,
                bool wasMate,
                ushort depth,
                NodeType nodeType,
                Move refutationMove
            )
            {
                WasMate = wasMate;
                Key = key;
                Score = score;
                Depth = depth;
                TtNodeType = nodeType;
                RefutationMove = refutationMove;
            }
        }

        /// <summary>
        /// Allows the computer to know if the evaluation score assigned to this position was pruned
        /// using alpha beta pruning. If it was pruned, this may be because the score was either
        /// going to be too good to be worth considering, in which case this node would be a lower
        /// bound (as it is either the score or better), or if this position was too bad to be worth
        /// considering, in which case this node would be an upper bound (as the score is either
        /// this score or worse).
        /// </summary>
        public enum NodeType : byte
        {
            NotEvaluated,
            Exact,
            UpperBound,
            LowerBound
        }
    }
}
