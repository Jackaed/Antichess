using System.Collections.Generic;
using Antichess.Pieces;
using Antichess.TargetSquares;

namespace Antichess
{
    // This provides an interface to the logic of the board, whilst hiding many of the public methods that shouldn't be
    // available to the player.
    public class Board
    {
        private readonly BoardLogic _boardLogic;

        public Board(bool renderBoard)
        {
            _boardLogic = renderBoard ? new RenderedBoardLogic() : new BoardLogic();
        }

        public Dictionary<Position, List<Position>> LegalMoves => _boardLogic.LegalMoves;
        public bool WhitesMove => _boardLogic.WhitesMove;

        public void OnNewFrame()
        {
            _boardLogic.OnNewFrame();
        }

        public bool IsLegal(Move move)
        {
            return _boardLogic.IsLegal(move);
        }

        public Piece PieceAt(Position pos)
        {
            return _boardLogic.PieceAt(pos);
        }

        public bool TryMove(Move move)
        {
            return _boardLogic.MovePiece(move);
        }
    }
}
