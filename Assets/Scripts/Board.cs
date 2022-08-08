using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Antichess.Pieces;
using Antichess.PositionTypes;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Antichess
{
    public class Board
    {
        private const int MaxSearchTimeMillis = 4000;

        private readonly Piece[,] _data;
        
        private readonly Stack<BoardChange> _moveHistory;

        private readonly List<Position> _wPieceLocations;
        private readonly List<Position> _bPieceLocations;
        
        private Move _bestMove;
        
        private bool _legalMovesOutdated;
        private readonly List<Move> _legalMoves;
        private bool _canEnPassant;
        
//        protected int ThreadsEvaluating;
//        private int _threadsLocked;
        private readonly NegamaxWorker _evaluator;
        public bool IsEvaluating;
        
        private WinnerEnum _winner;
        private bool _winnerOutdated;
        
        private ulong _zobristBlacksMove;
        private ulong _zobristHash;
        private bool _zobristOutdated;
        private ulong[,,] _zobristTable;
        
        public bool BestMoveOutdated;
        private ushort _moveCounter;
        private ushort _lastIrreversibleMove;

        private readonly List<ulong> _repetitionHistory;

        protected Board()
        {
            _evaluator = new NegamaxWorker(this);
            _data = new Piece[Constants.BoardSize, Constants.BoardSize];
            _wPieceLocations = new List<Position>();
            _bPieceLocations = new List<Position>();
            _legalMoves = new List<Move>();
            _moveHistory = new Stack<BoardChange>();
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            InitZobrist();
            _repetitionHistory = new List<ulong>() {ZobristHash};
            _bestMove = new Move(null, null);
            BestMoveOutdated = true;
            _winner = WinnerEnum.None;
            _winnerOutdated = true;
            _legalMovesOutdated = true;
            _zobristOutdated = true;
            // ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            // ReSharper restore StringLiteralTypo
        }

        public WinnerEnum Winner
        {
            get
            {
                if (_winnerOutdated)
                    UpdateWinner();

                return _winner;
            }
        }

        private class NegamaxWorker
        {
            private readonly Board _board;

            public NegamaxWorker(Board board)
            {
                _board = board;
            } // ReSharper disable Unity.PerformanceAnalysis

            public void Start()
            {
                // Not done using main thread to allow Unity to continue running while evaluation is completed
                if (_board.IsEvaluating) return;
                _board.IsEvaluating = true;
                new Thread(IDEvalTimer).Start();
            }

            private void IDEvalTimer(object stateInfo)
            {
                // If there's only one legal move, we should just make it, our evaluation is meaningless
                if (_board.LegalMoves.Count == 1)
                {
                    _board._bestMove = _board.LegalMoves[0];
                    _board.IsEvaluating = false;
                    _board.BestMoveOutdated = false;
                    return;
                }
                
                // Run a search with iterative deepening for MaxSearchTime
                var worker = new Thread(ParameterlessIDEval);
                worker.Start();
                Thread.Sleep(MaxSearchTimeMillis); // Sleeps current thread, not worker
                worker.Abort();
                
                _board.IsEvaluating = false;
                _board.BestMoveOutdated = false;
            }

            private void ParameterlessIDEval(object stateInfo)
            {
                IDEval(new Board(_board), out _board._bestMove);
            }
        }

        public Move BestMove => _bestMove;

        public void StartEval()
        {
            _evaluator.Start();
            
//           _threadsLocked = 0;
//           _bestMove = new EvaluatedMove(null, int.MinValue);
//           var legalMoves = new List<Move> (LegalMoves);


//           foreach (var negamaxWorker in legalMoves.Select(move => new NegamaxWorker(this, move)))
//           {
//               ThreadPool.QueueUserWorkItem(negamaxWorker.ThreadedNegamaxCall);
//           }

//           BestMoveOutdated = false;
        }

        private void InitZobrist()
        {
            var buffer = new byte[sizeof(ulong)];
            _zobristTable = new ulong[8, 8, 13];
            for (var x = 0; x < _zobristTable.GetLength(0); x++)
            for (var y = 0; y < _zobristTable.GetLength(1); y++)
            for (var z = 0; z < _zobristTable.GetLength(2); z++)
            {
                Constants.Instance.Rand.NextBytes(buffer);
                _zobristTable[x, y, z] = BitConverter.ToUInt64(buffer, 0);
            }

            Constants.Instance.Rand.NextBytes(buffer);
            _zobristBlacksMove = BitConverter.ToUInt64(buffer, 0);
        }

        private ulong ZobristHash
        {
            get
            {
                if (!_zobristOutdated)
                    return _zobristHash;

                _zobristHash = 0;
                if (!WhitesMove) _zobristHash = _zobristHash ^ _zobristBlacksMove;

                foreach (var pos in _wPieceLocations.Concat(_bPieceLocations))
                {
                    var j = PieceAt(pos).Value;
                    _zobristHash ^= _zobristTable[pos.X, pos.Y, j];
                }

                if (_canEnPassant && EnPassantTargetSquare != null)
                    _zobristHash = _zobristHash ^ _zobristTable[EnPassantTargetSquare.X, EnPassantTargetSquare.Y, 12];

                _zobristOutdated = false;
                return _zobristHash;
            }
        }


        // Copy constructor, returns a copy of the board toClone
        private Board(Board toClone)
        {
            _data = (Piece[,]) toClone._data.Clone();
            _wPieceLocations = new List<Position>(toClone._wPieceLocations);
            _bPieceLocations = new List<Position>(toClone._bPieceLocations);
            _moveHistory = new Stack<BoardChange>(toClone._moveHistory);
            _legalMoves = new List<Move>(toClone._legalMoves);
            _legalMovesOutdated = toClone._legalMovesOutdated;
            _zobristTable = (ulong[,,]) toClone._zobristTable.Clone();
            _zobristOutdated = toClone._zobristOutdated;
            _zobristHash = toClone._zobristHash;
            WhitesMove = toClone.WhitesMove;
            _evaluator = toClone._evaluator;
//           _threadsLocked = toClone._threadsLocked;
//           ThreadsEvaluating = toClone.ThreadsEvaluating;
            _winner = toClone._winner;
            _repetitionHistory = new List<ulong>(toClone._repetitionHistory);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Move(Move move)
        {
            if (IsLegal(move) && Winner == WinnerEnum.None) UnsafeMove(move);
        }

        public enum WinnerEnum
        {
            White,
            Black,
            Stalemate,
            None
        }

        protected virtual void UpdateWinner()
        {
            var count = 0;
            for (var i = _moveCounter - 4; i >= _lastIrreversibleMove; i -= 2)
            {
                if (_repetitionHistory[_moveCounter] == _repetitionHistory[i])
                    count++;
                if (count < 2) continue;
                _winner = WinnerEnum.Stalemate;
                _winnerOutdated = false;
                return;
            } 
            if (_lastIrreversibleMove == _moveCounter - 50)
                _winner = WinnerEnum.Stalemate;
            else
            {
                if (LegalMoves.Count == 0)
                    _winner = WhitesMove ? WinnerEnum.White : WinnerEnum.Black;
                else
                    _winner = WinnerEnum.None;
            }
            
            _winnerOutdated = false;
        }

        private List<Move> LegalMoves
        {
            get
            {
                if (_legalMovesOutdated) UpdateLegalMoves();

                return _legalMoves;
            }
        }

        private void UndoLastMove()
        {
            UndoMove(_moveHistory.Pop());
            _repetitionHistory.RemoveAt(_repetitionHistory.Count - 1);
        }

        public bool CanTake { get; private set; }

        public Position EnPassantTargetSquare { get; private set; }

        public bool WhitesMove { get; private set; }

        public static void AddLegalMove(Move move, Board board, List<Move> legalMoves, bool canTake)
        {
            var pieceFrom = board.PieceAt(move.From);
            if (pieceFrom == null) return;
            if (canTake != board.CanTake)
            {
                legalMoves.Clear();
                board.CanTake = true;
            }

            if (move.To.GetType() == typeof(EnPassantPosition))
            {
                board._canEnPassant = true;
            }

            legalMoves.Add(move);
        }

        private void UpdateLegalMoves()
        {
            CanTake = false;
            _canEnPassant = false;
            _legalMoves.Clear();

            var pieceLocations = WhitesMove ? _wPieceLocations : _bPieceLocations;
            foreach (var pos in pieceLocations) PieceAt(pos).AddMoves(pos, this, _legalMoves);
            _legalMovesOutdated = false;
        }

        private bool IsLegal(Move move)
        {
            return LegalMoves.Contains(move);
        }

        private string LegalMovesToString()
        {
            return string.Join("; ", LegalMoves);
        }

        public Piece PieceAt([NotNull] Position pos)
        {
            return _data[pos.X, pos.Y];
        }


        // Adds/removes a piece, but is not overridable, so is not used to create a new GameObject. Both are used
        // when making moves.
        private void Add([NotNull] Piece piece, [NotNull] Position pos)
        {
            pos = new Position(pos.X, pos.Y);
            _data[pos.X, pos.Y] = piece;

            var pieceLocation = piece.IsWhite ? _wPieceLocations : _bPieceLocations;
            var takesLocation = piece.IsWhite ? _bPieceLocations : _wPieceLocations;

            
            takesLocation.RemoveAll(r => r == new Position(pos.X, pos.Y));
            pieceLocation.Add(pos);

            _legalMovesOutdated = true;
            _zobristOutdated = true;
            BestMoveOutdated = true;
            _winnerOutdated = true;
        }

        private void Remove([NotNull] Position pos)
        {
            var piece = PieceAt(pos);

            if (piece == null) return;

            var pieceLocation = piece.IsWhite ? _wPieceLocations : _bPieceLocations;
            pieceLocation.RemoveAll(r => r == new Position(pos.X, pos.Y));
            _data[pos.X, pos.Y] = null;

            _legalMovesOutdated = true;
            _zobristOutdated = true;
            BestMoveOutdated = true;
            _winnerOutdated = true;
        }

        // Equivalents of Add and Remove, but fully create pieces from scratch, including their GameObjects in the 
        // RenderedBoard class.
        protected virtual void Create([NotNull] Piece piece, [NotNull] Position pos)
        {
            Add(piece, pos);
        }

        protected virtual void Destroy([NotNull] Position pos)
        {
            Remove(pos);
        }

        // Tree search algorithm for determining the value of a position, by analysing the heuristic value of the
        // potential positions that stem from the current one.
        private static int Negamax(uint depth, Board board, int alpha, int beta)
        {
            if (depth <= 0 || board.Winner != WinnerEnum.None)
                return board.HeuristicValue * (board.WhitesMove ? 1 : -1);

            var legalMoves = new List<Move>(board.LegalMoves);
            var score = int.MinValue;

            foreach (var move in legalMoves)
            {
                board.UnsafeMove(move);
                score = Mathf.Max(score, -Negamax(depth - 1, board, -beta, -alpha));
                board.UndoLastMove();

                alpha = Mathf.Max(score, alpha);
                if (alpha >= beta) return score; // Cutoff, prevents further analysis
            }

            return score;
        }
        
        private static void RootNode(uint depth, Board board, out Move bestMove)
        {
            Move tempBestMove = null;
            
            var alpha = int.MinValue + 100;
            const int beta = int.MaxValue - 100;

            var legalMoves = new List<Move>(board.LegalMoves);
            var score = int.MinValue;
            
            foreach (var move in legalMoves)
            {
                board.UnsafeMove(move);
                var eval = -Negamax(depth - 1, board, -beta, -alpha);
                
                if (eval > score)
                {
                    score = eval;
                    tempBestMove = move;
                }
                
                board.UndoLastMove();

                alpha = Mathf.Max(score, alpha);
            }

            bestMove = tempBestMove;
        }

        private static void IDEval (Board board, out Move bestMove)
        {
            for(uint i = 1; true; i++)
            {
                RootNode(i, board, out bestMove);
                Debug.Log("Finished eval to depth: " + i);
            }
            // ReSharper disable once FunctionNeverReturns
        }
        
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
                        return -_wPieceLocations.Sum(pos => (int) PieceAt(pos).Value)
                               + _bPieceLocations.Sum(pos => (int) PieceAt(pos).Value);
                    default:
                        Debug.Log("Null Winner");
                        return 0;
                }
            }
        }

        // Makes a move, but does not check if it is legal or not. Used in instances when we already know a move is 
        // legal, such as when suggesting moves by iterating over the LegalMoves dictionary.
        protected virtual void UnsafeMove(Move move)
        {
            
            _moveCounter++;
            
            _moveHistory.Push(new BoardChange(move, PieceAt(move.To),
                EnPassantTargetSquare == null ? null : EnPassantTargetSquare.Clone(), _lastIrreversibleMove));

            EnPassantTargetSquare = null;

            var pawnDoubleMove = move.To as PawnDoubleMovePosition;
            if (pawnDoubleMove != null) EnPassantTargetSquare = pawnDoubleMove.MovedThrough;

            var enPassant = move.To as EnPassantPosition;
            if (enPassant != null) Destroy(enPassant.TargetPieceSquare);

            var promotion = move.To as PromotionPosition;
            if (promotion != null)
            {
                Destroy(move.From);
                Create(promotion.PromotionPiece, move.From);
            }

            if (PieceAt(move.To) != null || PieceAt(move.From).GetType() == typeof(Pawn))
            {
                _lastIrreversibleMove = _moveCounter;
            }


            Add(PieceAt(move.From), move.To);
            Remove(move.From);
            WhitesMove = !WhitesMove;
            
            _repetitionHistory.Add(ZobristHash);
        }

        protected virtual void UndoMove(BoardChange change)
        {
            EnPassantTargetSquare = change.OldEnPassantTarget;

            Add(PieceAt(change.Move.To), change.Move.From);
            Remove(change.Move.To);

            if (change.Taken != null) Create(change.Taken, change.Move.To);

            WhitesMove = !WhitesMove;

            var enPassant = change.Move.To as EnPassantPosition;
            if (enPassant != null) Create(new Pawn(!WhitesMove), enPassant.TargetPieceSquare);

            var promotion = change.Move.To as PromotionPosition;
            if (promotion != null) Create(new Pawn(WhitesMove), change.Move.From);

            _moveCounter--;
            _lastIrreversibleMove = change.LastIrreversibleMove;
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
