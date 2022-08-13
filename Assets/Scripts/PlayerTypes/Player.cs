using Antichess.Core;

namespace Antichess.PlayerTypes
{
    public abstract class Player
    {
        protected readonly Board BoardRef;
        protected bool IsWhite;

        protected Player(Board board, bool isWhite)
        {
            BoardRef = board;
            IsWhite = isWhite;
        }

        public abstract Move SuggestMove();
    }
}