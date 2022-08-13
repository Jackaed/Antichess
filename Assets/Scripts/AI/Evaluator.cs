using System.Threading;
using Antichess.Core;

namespace Antichess.AI
{
    public class Evaluator
    {
        private const int MaxSearchTimeMillis = 4000;
        private readonly AIBoard _board;
        private readonly Thread _timerThread;

        private bool _isEvaluating;

        public Evaluator(Board board)
        {
            _board = new AIBoard(board);
            _isEvaluating = true;
            _timerThread = new Thread(IDEvalTimer);
            _timerThread.Start();
        }

        public Move BestMove
        {
            get
            {
                if (!_board.FinishedPrematurely) return _isEvaluating ? null : _board.BestMove;

                _timerThread.Abort();
                _isEvaluating = false;
                return _isEvaluating ? null : _board.BestMove;
            }
        }

        private void IDEvalTimer(object stateInfo)
        {
            // If there's only one legal move, we should just make it, our evaluation is meaningless
            // Run a search with iterative deepening for MaxSearchTime
            var worker = new Thread(_board.IDEval);
            worker.Start();
            Thread.Sleep(MaxSearchTimeMillis); // Sleeps current thread, not worker
            worker.Abort();

            _isEvaluating = false;
        }
    }
}