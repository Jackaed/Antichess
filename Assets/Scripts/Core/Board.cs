using System;
using System.Collections.Generic;
using Antichess.Core.Pieces;
using Antichess.Unity;
using JetBrains.Annotations;

namespace Antichess.Core
{
    public class Board
    {
        private readonly Piece[,] _data;

        private readonly Stack<BoardChange> _moveHistory;

        private readonly List<ulong> _repetitionHistory;


        protected readonly LegalMoves LegalMoves;

        protected readonly PieceLocations PieceLocations;
        private ushort _lastIrreversibleMove;

        private ushort _moveCounter;

//        protected int ThreadsEvaluating;
//        private int _threadsLocked;

        private WinnerEnum _winner;
        private bool _winnerOutdated;

        private ulong _zobristBlacksMove;
        private ulong _zobristHash;
        private bool _zobristOutdated;
        private ulong[,,] _zobristTable;

        protected Board()
        {
            _data = new Piece[ObjectLoader.BoardSize, ObjectLoader.BoardSize];
            PieceLocations = new PieceLocations(this);
            LegalMoves = new LegalMoves(this, PieceLocations);
            _moveHistory = new Stack<BoardChange>();
            // ReSharper disable StringLiteralTypo
            //ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            InitZobrist();
            _repetitionHistory = new List<ulong> {ZobristHash};
            _winner = WinnerEnum.None;
            _winnerOutdated = true;
            _zobristOutdated = true;
            // ReSharper restore StringLiteralTypo
        }


        // Copy constructor, returns a copy of the board toClone
        protected Board(Board toClone)
        {
            _data = (Piece[,]) toClone._data.Clone();
            PieceLocations = new PieceLocations(toClone.PieceLocations, this);
            _moveHistory = new Stack<BoardChange>(toClone._moveHistory);
            LegalMoves = new LegalMoves(toClone.LegalMoves, PieceLocations, this);
            _zobristTable = (ulong[,,]) toClone._zobristTable.Clone();
            _zobristOutdated = toClone._zobristOutdated;
            _zobristHash = toClone._zobristHash;
            WhitesMove = toClone.WhitesMove;
//           _threadsLocked = toClone._threadsLocked;
//           ThreadsEvaluating = toClone.ThreadsEvaluating;
            _winner = toClone._winner;
            _repetitionHistory = new List<ulong>(toClone._repetitionHistory);
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

        private ulong ZobristHash
        {
            get
            {
                if (!_zobristOutdated)
                    return _zobristHash;

                _zobristHash = 0;
                if (!WhitesMove) _zobristHash = _zobristHash ^ _zobristBlacksMove;

                foreach (var pos in PieceLocations.All)
                {
                    var j = PieceAt(pos).Value;
                    _zobristHash ^= _zobristTable[pos.X, pos.Y, j];
                }

                if (LegalMoves.CanEnPassant && EnPassantTargetSquare != null)
                    _zobristHash = _zobristHash ^ _zobristTable[EnPassantTargetSquare.X, EnPassantTargetSquare.Y, 12];

                _zobristOutdated = false;
                return _zobristHash;
            }
        }

        private void InitZobrist()
        {
            var buffer = new byte[sizeof(ulong)];
            _zobristTable = new ulong[8, 8, 13];
            for (var x = 0; x < _zobristTable.GetLength(0); x++)
            for (var y = 0; y < _zobristTable.GetLength(1); y++)
            for (var z = 0; z < _zobristTable.GetLength(2); z++)
            {
                ObjectLoader.Instance.Rand.NextBytes(buffer);
                _zobristTable[x, y, z] = BitConverter.ToUInt64(buffer, 0);
            }

            ObjectLoader.Instance.Rand.NextBytes(buffer);
            _zobristBlacksMove = BitConverter.ToUInt64(buffer, 0);
        } // ReSharper disable Unity.PerformanceAnalysis
        public virtual bool Move(Move move)
        {
            if (!LegalMoves.IsLegal(move) || Winner != WinnerEnum.None) return false;

            UnsafeMove(move);
            return true;
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
            {
                _winner = WinnerEnum.Stalemate;
            }
            else
            {
                if (LegalMoves.Count == 0)
                    _winner = WhitesMove ? WinnerEnum.White : WinnerEnum.Black;
                else
                    _winner = WinnerEnum.None;
            }

            _winnerOutdated = false;
        }

        protected void UndoLastMove()
        {
            UndoMove(_moveHistory.Pop());
            _repetitionHistory.RemoveAt(_repetitionHistory.Count - 1);
        }

        public Position EnPassantTargetSquare { get; private set; }

        public bool WhitesMove { get; private set; }

        public Piece PieceAt([NotNull] Position pos)
        {
            return _data[pos.X, pos.Y];
        }

        private void OnChange()
        {
            LegalMoves.OnBoardChange();
            _zobristOutdated = true;
            _winnerOutdated = true;
        }

        // Adds/removes a piece, but is not overridable, so is not used to create a new GameObject. Both are used
        // when making moves.
        private void Add([NotNull] Piece piece, [NotNull] Position pos)
        {
            PieceLocations.Remove(pos, PieceAt(pos));
            PieceLocations.Add(pos, piece);

            _data[pos.X, pos.Y] = piece;

            OnChange();
        }

        private void Remove([NotNull] Position pos)
        {
            var piece = PieceAt(pos);

            if (piece == null) return;

            PieceLocations.Remove(pos, piece);
            _data[pos.X, pos.Y] = null;

            OnChange();
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


        // Makes a move, but does not check if it is legal or not. Used in instances when we already know a move is 
        // legal, such as when suggesting moves by iterating over the LegalMoves dictionary.
        protected virtual void UnsafeMove(Move move)
        {
            _moveCounter++;

            _moveHistory.Push(new BoardChange(move, PieceAt(move.To),
                EnPassantTargetSquare == null ? null : EnPassantTargetSquare.Clone(), _lastIrreversibleMove));

            EnPassantTargetSquare = null;

            if (move.Flag == Core.Move.Flags.PawnDoubleMove)
                EnPassantTargetSquare = move.To - Position.Ahead(WhitesMove);

            if (move.Flag == Core.Move.Flags.EnPassant)
                Destroy(move.To - Position.Ahead(WhitesMove));

            if (move is Promotion promotion)
            {
                Destroy(move.From);
                Create(promotion.PromotionPiece, move.From);
            }

            if (PieceAt(move.To) != null || PieceAt(move.From).GetType() == typeof(Pawn))
                _lastIrreversibleMove = _moveCounter;


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

            if (change.Move.Flag == Core.Move.Flags.EnPassant)
                Create(new Pawn(!WhitesMove), change.Move.To - Position.Ahead(WhitesMove));

            if (change.Move is Promotion promotion)
                Create(new Pawn(WhitesMove), change.Move.From);

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
            var y = (sbyte) (ObjectLoader.BoardSize - 1);
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