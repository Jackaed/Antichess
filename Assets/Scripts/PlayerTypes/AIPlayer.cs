using Antichess.AI;
using Antichess.Core;

namespace Antichess.PlayerTypes
{
    public class AIPlayer : Player
    {
        private Evaluator _evaluator;
        private bool _hasReturnedMove;
        private int _numPositionsSearched;

        public AIPlayer(Board board, bool isWhite) : base(board, isWhite)
        {
            _hasReturnedMove = true;
        }


        public override Move SuggestMove()
        {
            // If we've returned a valid move, and we're being asked for another one, it's a new request, and we should 
            // start a new evaluator.
            if (_hasReturnedMove)
            {
                _evaluator = new Evaluator(BoardRef);
                _hasReturnedMove = false;
            }

            var move = _evaluator.BestMove;
            if (move != null)
                _hasReturnedMove = true;
            return move;
        }
    }
}