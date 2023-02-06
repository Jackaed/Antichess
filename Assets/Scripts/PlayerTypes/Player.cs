using Antichess.Core;

namespace Antichess.PlayerTypes
{
    /// <summary>
    /// The abstract type used to represent a player in a chess game.
    /// </summary>
    public abstract class Player
    {
        protected readonly Board BoardRef;
        protected readonly bool IsWhite;

        protected Player(Board board, bool isWhite)
        {
            BoardRef = board;
            IsWhite = isWhite;
        }

        public abstract Move SuggestMove();
    }
}
