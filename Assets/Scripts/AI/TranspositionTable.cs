using System.Linq;
using Antichess.Core;

namespace Antichess.AI
{
    public class TranspositionTable
    {
        private readonly ulong _size;
        private readonly Entry[] _table;
        private Board _board;

        public TranspositionTable(ulong size)
        {
            _size = size;
            _table = Enumerable.Repeat(Entry.NotEvaluated, (int) _size).ToArray();
        }

        public Entry Lookup(ulong key)
        {
            Entry val = _table[key % _size];
            return val.Key == key ? val : Entry.NotEvaluated;
        }

        public void Store(ulong key, int score, bool wasMate, ushort depth, NodeType nodeType, Move refutationMove)
        {
            _table[key % _size] = new Entry(key, score, wasMate, depth, nodeType, refutationMove);
        }

        public struct Entry
        {
            public static readonly Entry NotEvaluated = new(ulong.MinValue, int.MinValue, false, 0,
                NodeType.NotEvaluated, null);

            public readonly ushort Depth;
            public readonly ulong Key;
            public readonly int Score;
            public readonly bool WasMate;
            public readonly NodeType TtNodeType;
            public readonly Move RefutationMove;

            public Entry(ulong key, int score, bool wasMate, ushort depth, NodeType nodeType, 
                Move refutationMove)
            {
                WasMate = wasMate;
                Key = key;
                Score = score;
                Depth = depth;
                TtNodeType = nodeType;
                RefutationMove = refutationMove;
            }
        }
    }
}