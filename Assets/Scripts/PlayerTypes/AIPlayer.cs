using Antichess.PositionTypes;
using UnityEngine;
using Random = System.Random;

namespace Antichess.PlayerTypes
{
    public class AIPlayer : Player
    {
        private readonly Random _random;
        private const int MaxSearchDepth = 5;
        private int _numPositionsSearched;

        public AIPlayer(Board board, bool isWhite) : base(board, isWhite)
        {
            _random = new Random();
        }

        private Move SuggestMoveUsingDepthSearch()
        {
            _numPositionsSearched = 0;
            var testBoard = BoardRef.Copy();

            Move bestMove = null;
            var bestEval = -int.MaxValue;
            
            var fromEnum = testBoard.GetLegalPositionsFrom();
            while (fromEnum.MoveNext())
            {
                var toEnum = testBoard.GetLegalPositionsTo(fromEnum.Current);
                while (toEnum.MoveNext())
                {
                    var move = new Move(fromEnum.Current, toEnum.Current);

                    testBoard.MovePiece(move);
                    var eval = Negamax(testBoard, 1, false);

                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                    testBoard.UndoLastMove();
                }
            }
            Debug.Log(_numPositionsSearched);

            return bestMove;
        }

        private int Negamax(Board board, uint depth, bool isWhite)
        {
            if (depth == MaxSearchDepth)
            {
                _numPositionsSearched++;
                return board.Evaluate() * (IsWhite ? 1 : -1);
            }

            var value = -int.MaxValue;

            foreach(var move in board.GetLegalMoves())
            {
                foreach(var posTo in move.Value)
                {
                    board.MovePiece(new Move(move.Key, posTo));
                    value = Mathf.Max( value, -Negamax(board, depth + 1, !isWhite));
                    board.UndoLastMove();
                }
            }

            return value;
        }

        private Move SuggestRandomMove()
        {
            var randFromIndex = _random.Next(0, BoardRef.NumPositionsFrom);
            var fromEnumerator = BoardRef.GetLegalPositionsFrom();
            uint x = 0;

            while (fromEnumerator.MoveNext())
            {
                x++;
                if(x - 1 != randFromIndex) continue;
                var from = fromEnumerator.Current;
                var toEnumerator = BoardRef.GetLegalPositionsTo(from);
                var y = 0;
                var randToIndex = _random.Next(0, BoardRef.NumPositionsTo(from));
                
                while (toEnumerator.MoveNext())
                {
                    y++;
                    Debug.Log(randFromIndex + " " + randToIndex);
                    if (y - 1 != randToIndex) continue;
                    return new Move(from, toEnumerator.Current);
                } 
                
            }

            return null;
        }

        public override Move SuggestMove()
        {
            return SuggestMoveUsingDepthSearch(); // Temporary, will be replaced with actual AI code.
        }
    }
}
