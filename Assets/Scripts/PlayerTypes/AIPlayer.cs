namespace Antichess.PlayerTypes
{
    public class AIPlayer : Player
    {
        private int _numPositionsSearched;

        public AIPlayer(Board board, bool isWhite) : base(board, isWhite) { }

//        private Move SuggestRandomMove()
//        {
//            var randFromIndex = _random.Next(0, BoardRef.NumPositionsFrom);
//            var fromEnumerator = BoardRef.GetLegalPositionsFrom();
//            uint x = 0;
//
//            while (fromEnumerator.MoveNext())
//            {
//                x++;
//                if(x - 1 != randFromIndex) continue;
//                var from = fromEnumerator.Current;
//                var toEnumerator = BoardRef.GetLegalPositionsTo(from);
//                var y = 0;
//                var randToIndex = _random.Next(0, BoardRef.NumPositionsTo(from));
//                
//                while (toEnumerator.MoveNext())
//                {
//                    y++;
//                    Debug.Log(randFromIndex + " " + randToIndex);
//                    if (y - 1 != randToIndex) continue;
//                    return new Move(from, toEnumerator.Current);
//                } 
//                
//            }
//
//            return null;
//        }

        public override Move SuggestMove()
        {
            if (!BoardRef.BestMoveOutdated) return BoardRef.IsEvaluating ? null : BoardRef.BestMove;

            BoardRef.StartEval();
            return null;
        }
    }
}
