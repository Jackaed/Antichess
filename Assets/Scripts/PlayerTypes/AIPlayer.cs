using Antichess.AI;
using Antichess.Core;

namespace Antichess.PlayerTypes
{
    public class AIPlayer : Player
    {
        private const ulong TtSize = 16777216;
        private readonly float _playingStrength;
        private readonly TranspositionTable _transpositionTable;

        private Evaluator _evaluator;
        private bool _hasReturnedMove;
        private int _numPositionsSearched;

        public AIPlayer(Board board, bool isWhite, float playingStrength = 1.0f) : base(board, isWhite)
        {
            _playingStrength = playingStrength;

            _transpositionTable = new TranspositionTable(TtSize);
            _hasReturnedMove = true;
        }

        public override Move SuggestMove()
        {
            // If we've returned a valid move, and we're being asked for another one, it's a new request, and we should 
            // start a new evaluator.
            if (_hasReturnedMove)
            {
                _evaluator = new Evaluator(BoardRef, _transpositionTable, _playingStrength);
                _hasReturnedMove = false;
            }

            var move = _evaluator.BestMove;
            if (move != null)
                _hasReturnedMove = true;
            return move;
        }
    }
}