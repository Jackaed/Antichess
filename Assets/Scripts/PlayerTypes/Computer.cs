using Antichess.Evaluation;
using Antichess.Core;

namespace Antichess.PlayerTypes
{
    /// <summary>
    /// The player class for a computer controlled player. The AI Evaluates positions and suggests the position with
    /// the highest evaluation score to make a move within a set period of analysis time.
    /// </summary>
    public class Computer : Player
    {
        /// <summary>
        /// Transposition table size
        /// </summary>
        private const ulong TtSize = 16777216;
        private readonly float _playingStrength;
        private readonly TranspositionTable _transpositionTable;

        private Evaluator _evaluator;
        private bool _hasReturnedMove;

        public Computer(Board board, bool isWhite, float playingStrength = 1.0f)
            : base(board, isWhite)
        {
            _playingStrength = playingStrength;

            _transpositionTable = new TranspositionTable(TtSize);
            _hasReturnedMove = true;
        }

        public override Move SuggestMove()
        {
            // If we've returned a valid move, and we're being asked for another one, it's a new
            // request, and we should start a new evaluator.
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
