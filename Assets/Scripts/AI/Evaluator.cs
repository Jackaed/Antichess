using System;
using System.Threading;
using System.Threading.Tasks;
using Antichess.Core;

namespace Antichess.AI
{
    public class Evaluator
    {
        private const int MaxSearchTimeMillis = 4000;
        private const int CancellationCheckFrequency = 100;
        private const int NumCancellationChecks = MaxSearchTimeMillis / CancellationCheckFrequency;
        private const int MinMoveWaitTime = 500;
        private readonly AIBoard _board;

        private bool _isEvaluating;
        private bool _hasExceededMinWaitTime;
        private readonly CancellationTokenSource _timerTaskCancellationToken;

        // Evaluation begins as soon as the evaluator is created.
        public Evaluator(Board board, TranspositionTable transpositionTable)
        {
            _hasExceededMinWaitTime = false;
            _board = new AIBoard(board, transpositionTable);
            _isEvaluating = true;
            _timerTaskCancellationToken = new CancellationTokenSource();
            var token = _timerTaskCancellationToken.Token;
            Task.Run(() => IDEvalTimer(token), token);
        }

        // Returns the best move in the position, once it has been calculated for MaxSearchTimeMillis milliseconds. 
        // Until that point, null is returned.
        public Move BestMove
        {
            get
            {
                if (!_board.FinishedPrematurely) return _isEvaluating ? null : _board.BestMove;

                // If, for whatever reason, we finished early, for example if there was only one legal move in the
                // position, then we should just stop evaluating right away and return that move.
                _timerTaskCancellationToken.Cancel();

                if (!_hasExceededMinWaitTime) return null;
                
                _isEvaluating = false;
                return _board.BestMove;

            }
        }

        
        private void IDEvalTimer(CancellationToken finishedPrematurely)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Task.Run(() => _board.IDEval(token), token);
            
            // A minimum wait time means that moves arent made instantly, even if they can be, to allow an observer to
            // process what has happened
            for (var i = 0; i < NumCancellationChecks; i++)
            {
                if (CancellationCheckFrequency * i >= MinMoveWaitTime)
                {
                    _hasExceededMinWaitTime = true;
                }
                if (finishedPrematurely.IsCancellationRequested)
                {
                    tokenSource.Cancel();

                    if (CancellationCheckFrequency * i >= MinMoveWaitTime)
                    {
                        _isEvaluating = false;
                        return;
                    }
                }
                Thread.Sleep(CancellationCheckFrequency);
            }
            Thread.Sleep((MaxSearchTimeMillis) % CancellationCheckFrequency);
            
            tokenSource.Cancel();
            _isEvaluating = false;
        }
    }
}