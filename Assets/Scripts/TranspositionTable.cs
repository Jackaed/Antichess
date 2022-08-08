using System.Linq;

namespace Antichess
{
    public class TranspositionTable
    {
        public class Entry
        {
            public static readonly Entry NotEvaluated = new(ulong.MinValue, int.MinValue, 0, 
                NodeType.NotEvaluated, 0);

            public readonly ulong Key;
            public readonly int Score;
            public readonly ushort Depth;
            public readonly NodeType TtNodeType;
            public readonly ushort Age;

            public Entry(ulong key, int score, ushort depth, NodeType nodeType, ushort age)
            {
                Key = key;
                Score = score;
                Depth = depth;
                TtNodeType = nodeType;
                this.Age = age;
            }
        }
        
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
            var val = _table[key % _size];
            return val.Key == key ? val : Entry.NotEvaluated;
        }

        public void Store(ulong key, int score, ushort depth, NodeType nodeType, ushort age)
        {
            if (_table[key % _size].TtNodeType == NodeType.NotEvaluated || _table[key % _size].Age > age)
                _table[key % _size] = new Entry(key, score, depth, nodeType, age);
        }
    }
}
