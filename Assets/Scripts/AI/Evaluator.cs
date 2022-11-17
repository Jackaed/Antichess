using System.Threading;
using System.Threading.Tasks;
using Antichess.Core;
using UnityEngine;

namespace Antichess.AI
{
    public class Evaluator
    {
        private const int CancellationCheckFrequency = 100;
        private const int MinMoveWaitTime = 400;
        private readonly AIBoard _board;
        private readonly int _maxSearchTimeMillis;
        private readonly int _numCancellationChecks;
        private readonly CancellationTokenSource _timerTaskCancellationToken;
        private bool _hasExceededMinWaitTime;

        private bool _isEvaluating;

        // Evaluation begins as soon as the evaluator is created.
        public Evaluator(Board board, TranspositionTable transpositionTable, float playingStrength)
        {
            // the time spent searching scales from 0.5 second to 4 seconds as playing strength increases from 0 to 1.
            _maxSearchTimeMillis = Mathf.RoundToInt(500 + 3000 * playingStrength);

            // at a playing strength of 0, each position can be mis-evaluated by up to 2 Queen's worth in score. 
            var heuristicValueMaxRandomOffset = Mathf.RoundToInt(5000 * (1 - playingStrength));

            _numCancellationChecks = _maxSearchTimeMillis / CancellationCheckFrequency;

            _hasExceededMinWaitTime = false;
            _board = new AIBoard(board, transpositionTable, heuristicValueMaxRandomOffset);
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
            CancellationTokenSource tokenSource = new();
            var token = tokenSource.Token;

            Task.Run(() => _board.IDEval(token), token);

            // A minimum wait time means that moves aren't made instantly, even if they can be, to allow an observer to
            // process what has happened
            for (var i = 0; i < _numCancellationChecks; i++)
            {
                if (CancellationCheckFrequency * i >= MinMoveWaitTime) _hasExceededMinWaitTime = true;
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

            Thread.Sleep(_maxSearchTimeMillis % CancellationCheckFrequency);

            tokenSource.Cancel();
            _isEvaluating = false;
        }
    }
}