using System.Collections.Generic;
using Antichess.Pieces;
using Antichess.PlayerTypes;
using Antichess.TargetSquares;

namespace Antichess
{
    public class Board
    {
        private readonly BoardLogic _boardLogic;

        public Board(bool renderBoard)
        {
            _boardLogic = renderBoard ? new RenderedBoardLogic() : new BoardLogic();
        }

        public void OnNewFrame()
        {
            _boardLogic.OnNewFrame();
        }
        public Dictionary<Position, List<Position>> LegalMoves => _boardLogic.LegalMoves;
        public bool WhitesMove => _boardLogic.WhitesMove;
        
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