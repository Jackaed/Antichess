using System;
using System.Linq;
using System.Threading;
using Antichess.Core;
using UnityEngine;
using Random = System.Random;

namespace Antichess.AI
{
    // A version of the board used for calculating the best move in a given situation.
    public class AIBoard : Board
    {
        private readonly int _heuristicValueMaxRandomOffset;
        private readonly Random _random;
        private readonly TranspositionTable _transpositionTable;
        public bool FinishedPrematurely;


        public AIBoard(Board board, TranspositionTable transpositionTable, int heuristicValueMaxRandomOffset) :
            base(board)
        {
            _random = new Random();
            _transpositionTable = transpositionTable;
            _heuristicValueMaxRandomOffset = heuristicValueMaxRandomOffset;
        }

        public Move BestMove { get; private set; }

        // Determines the heuristic value of the current board position, with no analysis of future positions. A positive
        // value is better for white, and a negative is better for black. Subsequently, if white has won, infinity is
        // given, as black should avoid this position if at all possible, and vice versa.
        private int HeuristicValue => Winner switch
        {
            Winners.White => int.MaxValue - 200,
            Winners.Black => int.MinValue + 200,
            Winners.Stalemate => 0,
            Winners.None => -PieceLocations.White.Sum(pos => (int) PieceAt(pos).Value) +
                            PieceLocations.Black.Sum(pos => (int) PieceAt(pos).Value) +
                            _random.Next(-_heuristicValueMaxRandomOffset, _heuristicValueMaxRandomOffset + 1),
            _ => throw new ArgumentOutOfRangeException()
        };

        // Looks at the subsequent moves beyond this point, and calls itself on those moves. If we have reached our max
        // depth, we stop looking forward, and return the value of the board, without looking any deeper. Uses the Alpha
        // -Beta pruning optimisation, alongside the transposition table.
        private Result Negamax(uint depth, int alpha, int beta, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return new Result(false, 0, null);

            int origAlpha = alpha;

            // If this exact position has been evaluated before to the same or greater depth, simply return that value,
            // or use it for alpha-beta pruning.
            TranspositionTable.Entry ttEntry = _transpositionTable.Lookup(ZobristHash);
            Move refutationMove = null;
            if (ttEntry.TtNodeType != NodeType.NotEvaluated)
            {
                if (ttEntry.Depth >= depth)
                {
                    switch (ttEntry.TtNodeType)
                    {
                        case NodeType.Exact:
                            return new Result(ttEntry.WasMate, ttEntry.Score, ttEntry.RefutationMove);
                        case NodeType.LowerBound:
                            alpha = Math.Max(alpha, ttEntry.Score);
                            break;
                        case NodeType.UpperBound:
                            beta = Math.Min(beta, ttEntry.Score);
                            break;
                        case NodeType.NotEvaluated:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (alpha >= beta)
                        return new Result(ttEntry.WasMate, ttEntry.Score, ttEntry.RefutationMove);
                }
                refutationMove = ttEntry.RefutationMove;
            }

            // If we've reached a leaf node, stop and return.
            if (depth <= 0 && !LegalMoves.CanTake)
                return new Result(false, HeuristicValue * (WhitesMove ? 1 : -1), null);

            if (Winner != Winners.None)
                return new Result(true, HeuristicValue * (WhitesMove ? 1 : -1), null);

            Result score = new(false, int.MinValue, null);


            // Search all future positions, searching the best move calculated to a slightly lower depth first.
            bool refutationMoveCausedCutoff = false;

            if (refutationMove != null && LegalMoves.IsLegal(refutationMove))
            {
                UnsafeMove(refutationMove);
                Result eval = -Negamax(depth - 1, -beta, -alpha, token);

                if (eval.Eval > score.Eval)
                {
                    score = eval;
                    score.BestMove = refutationMove;
                }

                UndoLastMove();

                alpha = Mathf.Max(score.Eval, alpha);
                if (alpha >= beta) refutationMoveCausedCutoff = true;
            }

            if (!refutationMoveCausedCutoff)
                foreach (Move move in LegalMoves.List.Where(move => move != refutationMove))
                {
                    UnsafeMove(move);
                    Result eval = -Negamax(depth - 1, -beta, -alpha, token);

                    if (eval.Eval > score.Eval)
                    {
                        score = eval;
                        score.BestMove = move;
                    }

                    UndoLastMove();

                    alpha = Mathf.Max(score.Eval, alpha);
                    if (alpha >= beta) break; // Cutoff, prevents further analysis
                }

            // Store data in transposition table
            NodeType nodeType;
            if (score.Eval <= origAlpha)
                nodeType = NodeType.UpperBound;
            else if (score.Eval >= beta)
                nodeType = NodeType.LowerBound;
            else
                nodeType = NodeType.Exact;

            _transpositionTable.Store(ZobristHash, score.Eval, score.WasMate, (ushort) depth, nodeType, score.BestMove);

            return score;
        }

        // Negamax call on the root node. This is the same as usual negamax, however it also sets the best move at the
        // end, as well as setting alpha beta constraints for the root node.

        // Iteratively performs a depth search on the current position, until the CancellationToken requests that it 
        // stops. At the end of each eval, BestMove is updated with the best move that it has currently calculated.
        public void IDEval(CancellationToken token)
        {
            FinishedPrematurely = false;
            if (LegalMoves.Count == 0) return;
            BestMove = LegalMoves.List[0];
            if (LegalMoves.List.Count == 1)
                FinishedPrematurely = true;
            else
                for (uint i = 1; !token.IsCancellationRequested; i++)
                {
                    Debug.Log("Finished eval to depth: " + (i - 1));
                    Result result = Negamax(i, int.MinValue + 100, int.MaxValue - 100, token);
                    BestMove = result.BestMove;
                    FinishedPrematurely = FinishedPrematurely || result.WasMate;
                }
        }

        // Returns an evaluation, as well as telling us whether mate was found. Used to allow the evaluation to end
        // early if a path to mate has been found.
        private struct Result
        {
            public Result(bool wasMate, int eval, Move bestMove)
            {
                WasMate = wasMate;
                Eval = eval;
                BestMove = bestMove;
            }

            public readonly bool WasMate;
            public readonly int Eval;
            public Move BestMove;

            public static Result operator -(Result result)
            {
                return new Result(result.WasMate, -result.Eval, result.BestMove);
            }
        }
    }
}