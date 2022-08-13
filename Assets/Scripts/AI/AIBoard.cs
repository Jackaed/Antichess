using System.Linq;
using Antichess.Core;
using UnityEngine;

namespace Antichess.AI
{
    public class AIBoard : Board
    {
        public bool FinishedPrematurely;

        public AIBoard(Board board) : base(board)
        {
        }

        public Move BestMove { get; private set; }

        // Determines the heuristic value of the current board position, with no analysis of future positions.
        private int HeuristicValue
        {
            get
            {
                switch (Winner)
                {
                    case WinnerEnum.White:
                        return int.MaxValue - 200;
                    case WinnerEnum.Black:
                        return int.MinValue + 200;
                    case WinnerEnum.Stalemate:
                        return 0;
                    case WinnerEnum.None:
                        return -PieceLocations.White.Sum(pos => (int) PieceAt(pos).Value)
                               + PieceLocations.Black.Sum(pos => (int) PieceAt(pos).Value);
                    default:
                        Debug.Log("Null Winner");
                        return 0;
                }
            }
        }

        private int Negamax(uint depth, int alpha, int beta)
        {
            if (depth <= 0 || Winner != WinnerEnum.None)
                return HeuristicValue * (WhitesMove ? 1 : -1);

            var score = int.MinValue;

            foreach (var move in LegalMoves.List)
            {
                UnsafeMove(move);
                score = Mathf.Max(score, -Negamax(depth - 1, -beta, -alpha));
                UndoLastMove();

                alpha = Mathf.Max(score, alpha);
                if (alpha >= beta) return score; // Cutoff, prevents further analysis
            }

            return score;
        }

        private void RootNode(uint depth)
        {
            Move tempBestMove = null;

            var alpha = int.MinValue + 100;
            const int beta = int.MaxValue - 100;

            var score = int.MinValue;

            foreach (var move in LegalMoves.List)
            {
                UnsafeMove(move);
                var eval = -Negamax(depth - 1, -beta, -alpha);

                if (eval > score)
                {
                    score = eval;
                    tempBestMove = move;
                }

                UndoLastMove();

                alpha = Mathf.Max(score, alpha);
            }

            BestMove = tempBestMove;
        }

        public void IDEval()
        {
            FinishedPrematurely = false;
            if (LegalMoves.List.Count == 1)
            {
                BestMove = LegalMoves.List[0];
                FinishedPrematurely = true;
            }
            else
            {
                for (uint i = 1;; i++)
                {
                    RootNode(i);
                    Debug.Log("Finished eval to depth: " + i);
                }
            }
        }
    }
}