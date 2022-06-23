using System;

namespace Antichess.PlayerTypes
{
    public class AIPlayer : Player
    {
        private readonly Random _random;

        public AIPlayer(Board board, bool isWhite) : base(board, isWhite)
        {
            _random = new Random();
        }

        private Move SuggestRandomMove()
        {
            var randMoveIndex = _random.Next(BoardRef.NumPositionsFrom);
            var moveEnumerator = BoardRef.GetLegalMoveEnumerator();
            uint i = 0;

            do
            {
                var move = moveEnumerator.Current;
                if (i == randMoveIndex)
                {
                    var randMoveToIndex = _random.Next(move.Value.Count);
                    return new Move(move.Key, move.Value[randMoveToIndex]);
                }

                i++;
            } while (moveEnumerator.MoveNext());

            return null;
        }

        public override Move SuggestMove()
        {
            return SuggestRandomMove(); // Temporary, will be replaced with actual AI code.
        }
    }
}
