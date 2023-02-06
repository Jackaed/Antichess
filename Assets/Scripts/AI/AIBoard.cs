using System;
using System.Linq;
using System.Threading;
using Antichess.Core;
using UnityEngine;
using Random = System.Random;

namespace Antichess.AI
{
    /// <summary>
    /// Used for the AI to make moves and test their effectiveness, then unmake those moves and try
    /// alternatives. 
    /// </summary>
    public class AIBoard : Board
    {
        private readonly int _heuristicValueMaxRandomOffset;
        private readonly TranspositionTable _transpositionTable;
        public bool FinishedPrematurely;
        private readonly Random _random;

        public AIBoard(
            Board board,
            TranspositionTable transpositionTable,
            int heuristicValueMaxRandomOffset
        ) : base(board)
        {
            _random = new Random();
            _transpositionTable = transpositionTable;
            _heuristicValueMaxRandomOffset = heuristicValueMaxRandomOffset;
        }
        
        public Move BestMove { get; private set; }

        /// <summary>
        /// Determines the heuristic value of the current board position, with no analysis of future
        /// positions. A positive value is better for white, and a negative is better for black.
        /// Subsequently, if white has won, infinity is given, as black should avoid this position
        /// if possible, and vice versa. Determines the heuristic value of the current board
        /// position, with no analysis of future positions.
        /// </summary>
        private int HeuristicValue =>
            Winner switch
            {
                Winners.White => int.MaxValue - 200,
                Winners.Black => int.MinValue + 200,
                Winners.Stalemate => 0,
                Winners.None
                    => -PieceLocations.White.Sum(pos => (int)PieceAt(pos).Value)
                        + PieceLocations.Black.Sum(pos => (int)PieceAt(pos).Value),
                _ => throw new ArgumentOutOfRangeException()
            };
        
        private int GetRandomOffset()
        {
            int temp = _random.Next(
                -_heuristicValueMaxRandomOffset,
                _heuristicValueMaxRandomOffset
            );
            return temp;
        }

        /// <summary>
        /// Looks at the subsequent moves beyond this point, and calls itself on those moves. If we
        /// have reached our max depth, we stop looking forward, and return the value of the board,
        /// without looking any deeper. Uses the Alpha -Beta pruning optimization, alongside the
        /// transposition table. If the CancellationToken is cancelled, the function returns garbage
        /// in order to exit as soon as possible.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="token"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Result Negamax(uint depth, int alpha, int beta, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return new Result(false, 0, null);
            }

            var origAlpha = alpha;
            // If this exact position has been evaluated before to the same or greater depth, simply
            // return that value, or use it for alpha-beta pruning.
            var ttEntry = _transpositionTable.Lookup(ZobristHash);
            Move refutationMove = null;
            if (ttEntry.TtNodeType != TranspositionTable.NodeType.NotEvaluated)
            {
                if (ttEntry.Depth >= depth)
                {
                    switch (ttEntry.TtNodeType)
                    {
                        case TranspositionTable.NodeType.Exact:
                            return new Result(
                                ttEntry.WasMate,
                                ttEntry.Score,
                                ttEntry.RefutationMove
                            );
                        case TranspositionTable.NodeType.LowerBound:
                            alpha = Math.Max(alpha, ttEntry.Score);
                            break;
                        case TranspositionTable.NodeType.UpperBound:
                            beta = Math.Min(beta, ttEntry.Score);
                            break;
                        case TranspositionTable.NodeType.NotEvaluated:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (alpha >= beta)
                    {
                        return new Result(ttEntry.WasMate, ttEntry.Score, ttEntry.RefutationMove);
                    }
                }

                refutationMove = ttEntry.RefutationMove;
            }

            // If we've reached a leaf node, stop and return.
            if ((depth <= 0 && !LegalMoves.CanTake) || Winner != Winners.None)
            {
                return new Result(
                    Winner != Winners.None,
                    HeuristicValue * (WhitesMove ? 1 : -1),
                    null
                );
            }

            Result score = new(false, int.MinValue, null);
            // Search all future positions, searching the best move calculated to a slightly lower
            // depth first.

            foreach (var move in LegalMoves.OrderedMoves(refutationMove))
            {
                UnsafeMove(move);
                var eval = -Negamax(depth - 1, -beta, -alpha, token);
                eval.Eval += GetRandomOffset();

                if (eval.Eval > score.Eval || score.BestMove == null)
                {
                    score = eval;
                    score.BestMove = move;
                }

                UndoLastMove();

                alpha = Mathf.Max(score.Eval, alpha);
                if (alpha >= beta)
                    break; // Cutoff, prevents further analysis
            }

            // Store data in transposition table
            TranspositionTable.NodeType NodeType;
            if (score.Eval <= origAlpha)
                NodeType = TranspositionTable.NodeType.UpperBound;
            else if (score.Eval >= beta)
                NodeType = TranspositionTable.NodeType.LowerBound;
            else
                NodeType = TranspositionTable.NodeType.Exact;

            _transpositionTable.Store(
                ZobristHash,
                score.Eval,
                score.WasMate,
                (ushort)depth,
                NodeType,
                score.BestMove
            );

            return score;
        }

        /// <summary>
        /// Iteratively performs a depth search on the current position, until the CancellationToken
        /// requests that it stops. At the end of each eval, BestMove is updated with the best move
        /// that it has currently calculated.
        /// </summary>
        /// <param name="token"></param>
        public void IDEval(CancellationToken token)
        {
            FinishedPrematurely = false;
            if (LegalMoves.Count == 0)
                return;
            BestMove = LegalMoves.List[0];
            if (LegalMoves.List.Count == 1)
            {
                FinishedPrematurely = true;
            }
            else
            {
                for (uint i = 1; !token.IsCancellationRequested; i++)
                {
                    Debug.Log("Finished eval to depth: " + (i - 1));
                    var result = Negamax(i, int.MinValue + 100, int.MaxValue - 100, token);
                    if (!token.IsCancellationRequested && result.BestMove != null)
                    {
                        BestMove = result.BestMove;
                        FinishedPrematurely = FinishedPrematurely || result.WasMate;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an evaluation, as well as telling us whether mate was found. Used to allow the
        /// evaluation to end early if a path to mate has been found.
        /// </summary>
        private struct Result
        {
            public Result(bool wasMate, int eval, Move bestMove)
            {
                WasMate = wasMate;
                Eval = eval;
                BestMove = bestMove;
            }

            public readonly bool WasMate;
            public int Eval;
            public Move BestMove;

            public static Result operator -(Result result)
            {
                return new Result(result.WasMate, -result.Eval, result.BestMove);
            }
        }
    }
}
