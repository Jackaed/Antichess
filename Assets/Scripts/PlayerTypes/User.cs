using Antichess.Pieces;
using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess.PlayerTypes
{
    public class User : Player
    {
        private Camera _cam;
        
        public User(Board board, bool isWhite) : base(board, isWhite)
        {
            _cam = Camera.main;
        }

        private bool _hasFrom;
        private Position _from;

        private Move GetPossibleMove(Move move)
        {
            if (BoardRef.PieceAt(move.From).GetType() != typeof(Pawn)) return move;
            
            //Test if user is attempting to move the pawn forward by two
            if (move.To.y == move.From.y + (IsWhite ? 2 : -2))
            {
                return new Move(_from, new PawnDoubleMovePosition(move.To,
                    move.To - (IsWhite ? Vector2Int.up : Vector2Int.down)));
            }
            
            // Test if user is attempting to make an en passant
            if (BoardRef.PieceAt(move.To) == null && move.From.x != move.To.x)
            {
                return (new Move(_from, new EnPassantPosition(move.To,
                    new Position(move.To.x, (byte) (move.To.y - (IsWhite ? 1 : -1))))));
            }
            
            // Test if user is attempting to promote

            return move;
        }
        
        public override Move SuggestMove()
        {
            if (!Input.GetMouseButtonDown(0)) return null;
            
            var mouseRay = _cam!.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(mouseRay, out var hit)) return null;
            
            var pos = ObjectLoader.GetBoardCoords(hit.point);
            if (_hasFrom)
            {
                _hasFrom = false;
                return GetPossibleMove(new Move(_from, pos));
            }
            else
            {
                if (BoardRef.PieceAt(pos) == null) return null;
                _from = pos;
                _hasFrom = true;
            }
            return null;
        }
    }
}