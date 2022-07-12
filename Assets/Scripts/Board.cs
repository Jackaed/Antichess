using System.Collections.Generic;
using System.Linq;
using Antichess.Pieces;
using Antichess.PositionTypes;
using JetBrains.Annotations;
using UnityEngine;
using Position = Antichess.PositionTypes.Position;

namespace Antichess
{
    public class Board
    {

        private const int MaxSearchDepth = 5;

        private readonly List<Position> _bPieceLocations;
        private readonly Piece[,] _data;
        private readonly Stack<BoardChange> _moveHistory;
        private bool _legalMovesOutdated;
        private int _numPositionsAnalysed;
        

        private readonly Dictionary<Position, List<Position>> _legalMoves;

        private readonly List<Position> _wPieceLocations;

        public Board()
        {
            _data = new Piece[Constants.BoardSize, Constants.BoardSize];
            _wPieceLocations = new List<Position>();
            _bPieceLocations = new List<Position>();
            _legalMoves = new Dictionary<Position, List<Position>>();
            _moveHistory = new Stack<BoardChange>();
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            // ReSharper restore StringLiteralTypo
            UpdateLegalMoves();
        }

        
        // Copy constructor, returns a copy of the board toClone
        private Board(Board toClone)
        {
            _data = (Piece[,]) toClone._data.Clone();
            _wPieceLocations = new List<Position>(toClone._wPieceLocations);
            _bPieceLocations = new List<Position>(toClone._bPieceLocations);
            _moveHistory = new Stack<BoardChange>(toClone._moveHistory);
            _legalMoves = new Dictionary<Position, List<Position>>(toClone._legalMoves);
            _legalMovesOutdated = toClone._legalMovesOutdated;
            WhitesMove = toClone.WhitesMove;
        }
        
        public bool Move(Move move)
        {
            if (!IsLegal(move)) return false;
            
            UnsafeMove(move);
            return true;
        }

        private Dictionary<Position, List<Position>> LegalMoves
        {
            get
            {
                if (_legalMovesOutdated) UpdateLegalMoves();
                return new Dictionary<Position, List<Position>>(_legalMoves);
            }
        }

        public void UndoLastMove()
        {
            UndoMove(_moveHistory.Pop());
        }

        public bool CanTake { get; private set; }
        public Position EnPassantTargetSquare { get; private set; }

        public bool WhitesMove { get; private set; }

        public virtual void OnNewFrame() { }

        public static void AddLegalMove(Move move, Board board, Dictionary<Position, List<Position>> legalMoves,
            bool canTake)
        {
            var pieceFrom = board.PieceAt(move.From);
            if (pieceFrom == null) return;
            if (canTake != board.CanTake)
            {
                legalMoves.Clear();
                board.CanTake = true;
            }

            if (legalMoves.ContainsKey(move.From))
                legalMoves[move.From].Add(move.To);
            else
                legalMoves[move.From] = new List<Position> {move.To};
        }

        private void UpdateLegalMoves()
        {
            CanTake = false;
            _legalMoves.Clear();

            var pieceLocations = WhitesMove ? _wPieceLocations : _bPieceLocations;
            foreach (var pos in pieceLocations) PieceAt(pos).AddMoves(pos, this, _legalMoves);
            _legalMovesOutdated = false;
        }

        private bool IsLegal(Move move)
        {
            if (_legalMovesOutdated)
                UpdateLegalMoves();
            return _legalMoves.ContainsKey(move.From) && _legalMoves[move.From].Any(p => move.To == p);
        }

        private string LegalMovesToString()
        {
            return string.Join("; ", _legalMoves.Select(pair => $"{pair.Key} => {string.Join(", ", pair.Value)}"));
        }

        public Piece PieceAt(Position pos)
        {
            return _data[pos.X, pos.Y];
        }

        
        // Adds/removes a piece, but does is not overridable, so is not used to create a new GameObject. Both are used
        // when making moves.
        private void Add([NotNull] Piece piece, Position pos)
        {
            pos = new Position(pos.X, pos.Y);
            _data[pos.X, pos.Y] = piece;

            var pieceLocation = piece.IsWhite ? _wPieceLocations : _bPieceLocations;
            var takesLocation = piece.IsWhite ? _bPieceLocations : _wPieceLocations;
            
            takesLocation.Remove(pos);
            pieceLocation.Add(pos);
        }
        
        private void Remove(Position pos)
        {
            var piece = PieceAt(pos);
            
            if (piece == null) return;

            var pieceLocation = piece.IsWhite ? _wPieceLocations : _bPieceLocations;
            pieceLocation.Remove(new Position(pos.X, pos.Y));
            _data[pos.X, pos.Y] = null;
        }
        
        // Equivalents of Add and Remove, but fully create pieces from scratch, including their GameObjects in the 
        // RenderedBoard class.
        protected virtual void Create([NotNull] Piece piece, Position pos)
        {
            Add(piece, pos);
        }
        
        protected virtual void Destroy(Position pos)
        {
            Remove(pos);
        }

        // Calls a Negamax search on all possible positions, and returns the one that is best for whoever it is to
        // move at the moment.
        public Move BestMove
        {
            get
            {
                Move bestMove = null;
                var bestEval = int.MinValue;
                var boardCopy = new Board(this);
                var legalMoves = boardCopy.LegalMoves;
                boardCopy._numPositionsAnalysed = 0;
                
                foreach(var (from, positionsTo) in legalMoves)
                {
                    foreach (var to in positionsTo)
                    {
                        var move = new Move(from, to);
                        
                        boardCopy.UnsafeMove(move);
                        var eval = -Negamax(0, boardCopy, int.MinValue + 100, int.MaxValue - 100);

                        if (eval > bestEval || bestMove == null)
                        {
                            bestEval = eval;
                            bestMove = move;
                        }
                        
                        boardCopy.UndoLastMove();
                    }
                }
                Debug.Log(boardCopy._numPositionsAnalysed);
                return bestMove;
            }
        }

        private int Evaluate ()
        { 
            var boardCopy = new Board(this);
            return -Negamax(0, boardCopy, int.MinValue + 100, int.MaxValue - 100);
        }

        // Tree search algorithm for determining the value of a position, by analysing the heuristic value of the
        // potential positions that stem from the current one.
        private static int Negamax(uint depth, Board board, int alpha, int beta)
        {
            if (depth >= MaxSearchDepth)
            {
                return board.HeuristicValue * (board.WhitesMove ? 1 : -1);
            }

            var value = int.MinValue;
            var legalMoves = board.LegalMoves;
            
            foreach(var (from, positionsTo) in legalMoves)
            {
                foreach(var to in positionsTo)
                {
                    board.UnsafeMove(new Move(from, to));
                    value = Mathf.Max( value, -Negamax(depth + 1, board, -beta, -alpha));
                    board.UndoLastMove();
                    
                    alpha = Mathf.Max(value, alpha);
                    if (alpha >= beta)
                    {
                        return value;
                    }
                }
            }

            return value;
        }
        
        
        // Determines the heuristic value of the current board position, with no analysis of future positions.
        private int HeuristicValue
        {
            get
            {
                var total = -_wPieceLocations.Sum(pos => (int) PieceAt(pos).Value) 
                            + _bPieceLocations.Sum(pos => (int) PieceAt(pos).Value);
                _numPositionsAnalysed++;
                return total;
            }
        }
        
        // Makes a move, but does not check if it is legal or not. Used in instances when we already know a move is 
        // legal, such as when suggesting moves by iterating over the LegalMoves dictionary.
        protected virtual void UnsafeMove(Move move)
        {
            
            _moveHistory.Push(new BoardChange(move, PieceAt(move.To), 
                EnPassantTargetSquare == null ? null : EnPassantTargetSquare.Clone()));

            EnPassantTargetSquare = null;
            
            var pawnDoubleMove = move.To as PawnDoubleMovePosition;
            if (pawnDoubleMove != null) EnPassantTargetSquare = pawnDoubleMove.MovedThrough;

            var enPassant = move.To as EnPassantPosition;
            if (enPassant != null) Destroy(enPassant.TargetPieceSquare);

            var promotion = move.To as PromotionPosition;
            if (promotion != null) Create(promotion.PromotionPiece, move.From);


            Add(PieceAt(move.From), move.To);
            Remove(move.From);
            WhitesMove = !WhitesMove;
            _legalMovesOutdated = true;
        }
        
        protected virtual void UndoMove(BoardChange change)
        {
            EnPassantTargetSquare = change.OldEnPassantTarget;

            Add(PieceAt(change.Move.To), change.Move.From);
            Remove(change.Move.To);
            
            if (change.Taken != null)
            {
                Create(change.Taken, change.Move.To);
            }
            
            WhitesMove = !WhitesMove;
            
            var enPassant = change.Move.To as EnPassantPosition;
            if (enPassant != null) Create(new Pawn(!WhitesMove), enPassant.TargetPieceSquare);
            
            var promotion = change.Move.To as PromotionPosition;
            if (promotion != null) Create(new Pawn(WhitesMove), change.Move.From);
            
            _legalMovesOutdated = true;
        }
        
        private static Piece GetPieceFromChar(char character)
        {
            var isWhite = char.IsUpper(character);

            Piece piece = char.ToLower(character) switch
            {
                'p' => new Pawn(isWhite),
                'b' => new Bishop(isWhite),
                'k' => new King(isWhite),
                'n' => new Knight(isWhite),
                'q' => new Queen(isWhite),
                'r' => new Rook(isWhite),
                _ => null
            };

            return piece;
        }

        private void ProcessFenString(string fenString)
        {
            var y = (sbyte) (Constants.BoardSize - 1);
            sbyte x = 0;
            var i = 0;
            while (fenString[i] != ' ')
            {
                if (fenString[i] == '/')
                {
                    y--;
                    x = 0;
                }
                else if (char.IsNumber(fenString[i]))
                {
                    x += (sbyte) (fenString[i] - '0');
                }
                else
                {
                    Create(GetPieceFromChar(fenString[i]), new Position(x, y));
                    x++;
                }

                i++;
            }

            i++; // Skips the space

            WhitesMove = fenString[i] == 'w';
        }
    }
}
