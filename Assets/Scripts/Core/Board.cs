using System;
using System.Collections.Generic;
using Antichess.Core.Pieces;
using Antichess.Unity;
using JetBrains.Annotations;

namespace Antichess.Core
{
    public class Board
    {
        public enum Winners : byte
        {
            White,
            Black,
            Stalemate,
            None
        }

        protected const int Size = 8;
        private readonly Piece[,] _data;

        private readonly Stack<BoardChange> _moveHistory;

        private readonly List<ulong> _repetitionHistory;

        protected readonly LegalMoves LegalMoves;

        protected readonly PieceLocations PieceLocations;
        private bool _gameHasStarted;
        private ushort _halfMoveClock;
        private Winners _winner;
        private bool _winnerOutdated;

        private ulong _zobristBlacksMove;
        private ulong _zobristHash;
        private bool _zobristOutdated;
        private ulong[,,] _zobristTable;

        protected Board()
        {
            _gameHasStarted = false;
            _data = new Piece[ObjectLoader.BoardSize, ObjectLoader.BoardSize];
            PieceLocations = new PieceLocations();
            LegalMoves = new LegalMoves(this, PieceLocations);
            _moveHistory = new Stack<BoardChange>();
            InitZobrist();
            _repetitionHistory = new List<ulong> { ZobristHash };
            _winner = Winners.None;
            _winnerOutdated = true;
            _zobristOutdated = true;
            _halfMoveClock = 0;
        }

        /// <summary>
        ///     Copy constructor, returns a copy of the board toClone
        /// </summary>
        /// <param name="toClone"></param>
        protected Board(Board toClone)
        {
            _data = (Piece[,])toClone._data.Clone();
            PieceLocations = new PieceLocations(toClone.PieceLocations);
            _moveHistory = new Stack<BoardChange>(toClone._moveHistory);
            LegalMoves = new LegalMoves(toClone.LegalMoves, PieceLocations, this);
            _zobristTable = (ulong[,,])toClone._zobristTable.Clone();
            _zobristOutdated = toClone._zobristOutdated;
            _zobristHash = toClone._zobristHash;
            WhitesMove = toClone.WhitesMove;
            _winner = toClone._winner;
            _halfMoveClock = toClone._halfMoveClock;
            _repetitionHistory = new List<ulong>(toClone._repetitionHistory);
        }

        public bool WhitesMove { get; private set; }

        public Position EnPassantTargetSquare { get; private set; }

        public Winners Winner
        {
            get
            {
                if (_winnerOutdated)
                    UpdateWinner();

                return _winner;
            }
        }

        protected ulong ZobristHash
        {
            get
            {
                if (!_zobristOutdated)
                    return _zobristHash;

                _zobristHash = 0;
                if (!WhitesMove)
                    _zobristHash ^= _zobristBlacksMove;

                foreach (var pos in PieceLocations.All)
                {
                    var j = PieceAt(pos).Index;
                    _zobristHash ^= _zobristTable[pos.X, pos.Y, j];
                }

                if (LegalMoves.CanEnPassant && EnPassantTargetSquare != null)
                {
                    _zobristHash ^= _zobristTable[
                        EnPassantTargetSquare.X,
                        EnPassantTargetSquare.Y,
                        12
                    ];
                }

                _zobristOutdated = false;
                return _zobristHash;
            }
        }

        public void Destroy()
        {
            if (_gameHasStarted && Winner == Winners.None)
                return;

            foreach (var pos in PieceLocations.All)
                Destroy(pos);
        }

        public virtual void StartNewGame()
        {
            if (_gameHasStarted && Winner == Winners.None)
                return;
            // cspell: disable-next-line
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ProcessFenString("8/6P1/8/2k5/4K3/8/8/8 w - - 0 1");

            _gameHasStarted = true;
        }

        public Piece PieceAt(Position pos)
        {
            return _data[pos.X, pos.Y];
        }

        /// <summary>
        ///     Adds a piece at the given position
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pos"></param>
        private void Add(Piece piece, Position pos)
        {
            PieceLocations.Remove(pos, PieceAt(pos));
            PieceLocations.Add(pos, piece);

            _data[pos.X, pos.Y] = piece;

            OnChange();
        }

        /// <summary>
        ///     Removes a piece at the given position
        /// </summary>
        /// <param name="pos"></param>
        private void Remove(Position pos)
        {
            var piece = PieceAt(pos);

            if (piece == null)
                return;

            PieceLocations.Remove(pos, piece);
            _data[pos.X, pos.Y] = null;

            OnChange();
        }

        /// <summary>
        ///     Whenever the board state changes, this makes various aspects of the program
        ///     outdated, so they will be updated upon next access.
        /// </summary>
        private void OnChange()
        {
            LegalMoves.OnBoardChange();
            _zobristOutdated = true;
            _winnerOutdated = true;
        }

        /// <summary>
        ///     Equivalents of Add() but creates a new GameObject if this is a RenderedBoard.
        /// </summary>
        protected virtual void Create(Piece piece, Position pos)
        {
            Add(piece, pos);
        }

        /// <summary>
        ///     Destroys a piece at a given position. Similar to Remove(Piece, Position), but
        ///     permanently destroys a piece, rather than being used for moving pieces. Used for
        ///     RenderedBoard, as destroying pieces and moving them elsewhere are visually distinct
        ///     operations.
        /// </summary>
        /// <param name="pos"></param>
        protected virtual void Destroy([NotNull] Position pos)
        {
            Remove(pos);
        }

        /// <summary>
        ///     Checks if the provided move is legal, and if it is, makes the move.
        /// </summary>
        public virtual bool Move(Move move)
        {
            if (!LegalMoves.IsLegal(move) || Winner != Winners.None)
                return false;

            UnsafeMove(move);
            return true;
        }

        /// <summary>
        ///     Makes a move, but does not check if it is legal or not. Used in instances when we
        ///     already know a move is legal, such as when suggesting moves by iterating over the
        ///     LegalMoves dictionary.
        /// </summary>
        /// <param name="move"></param>
        protected virtual void UnsafeMove(Move move)
        {
            _repetitionHistory.Add(ZobristHash);

            _moveHistory.Push(
                new BoardChange(
                    move,
                    PieceAt(move.To),
                    EnPassantTargetSquare?.Clone(),
                    _halfMoveClock
                )
            );

            _halfMoveClock++;

            // Reset Halfmove clock if there is a capture or a pawn move.
            if (PieceAt(move.To) != null || PieceAt(move.From).Type == Piece.Types.Pawn)
                _halfMoveClock = 0;

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

            Destroy(move.To);
            Add(PieceAt(move.From), move.To);
            Remove(move.From);
            WhitesMove = !WhitesMove;
        }

        protected void UndoLastMove()
        {
            if (_moveHistory.Count == 0)
                return;
            UndoMove(_moveHistory.Pop());
            _repetitionHistory.RemoveAt(_repetitionHistory.Count - 1);
        }

        protected virtual void UndoMove(BoardChange change)
        {
            EnPassantTargetSquare = change.OldEnPassantTarget;

            Add(PieceAt(change.Move.To), change.Move.From);
            Remove(change.Move.To);

            if (change.Taken != null)
                Create(change.Taken, change.Move.To);

            WhitesMove = !WhitesMove;

            if (change.Move.Flag == Core.Move.Flags.EnPassant)
            {
                Create(
                    new Piece(!WhitesMove, Piece.Types.Pawn),
                    change.Move.To - Position.Ahead(WhitesMove)
                );
            }

            if (change.Move is Promotion)
                Create(new Piece(WhitesMove, Piece.Types.Pawn), change.Move.From);

            _halfMoveClock = change.HalfMoveClock;
        }

        protected virtual void UpdateWinner()
        {
            _winnerOutdated = false;

            _winner =
                LegalMoves.Count == 0
                    ? WhitesMove
                        ? Winners.White
                        : Winners.Black
                    : (_winner == Winners.None && _halfMoveClock >= 50)
                    || CalculateDrawByRepetition()
                        ? _winner = Winners.Stalemate
                        : Winners.None;
        }

        private bool CalculateDrawByRepetition()
        {
            ulong currentZobrist = ZobristHash;

            int numIrreversibleMoves = _halfMoveClock - 2;
            int count = 0;

            while (numIrreversibleMoves >= 0)
            {
                if (
                    _repetitionHistory[
                        _repetitionHistory.Count - _halfMoveClock + numIrreversibleMoves
                    ] == currentZobrist
                )
                {
                    count++;
                }

                if (count >= 2)
                    return true;

                numIrreversibleMoves -= 2;
            }

            return false;
        }

        private void InitZobrist()
        {
            var buffer = new byte[sizeof(ulong)];
            _zobristTable = new ulong[8, 8, 13];
            for (var x = 0; x < _zobristTable.GetLength(0); x++)
            {
                for (var y = 0; y < _zobristTable.GetLength(1); y++)
                {
                    for (var z = 0; z < _zobristTable.GetLength(2); z++)
                    {
                        ObjectLoader.Instance.Rand.NextBytes(buffer);
                        _zobristTable[x, y, z] = BitConverter.ToUInt64(buffer, 0);
                    }
                }
            }

            ObjectLoader.Instance.Rand.NextBytes(buffer);
            _zobristBlacksMove = BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Sets board's data to the data fed in from a FEN string. Useful for making boards from
        /// standard positions, e.g. the board's starting position.
        /// </summary>
        /// <param name="fenString"></param>
        private void ProcessFenString(string fenString)
        {
            sbyte y = ObjectLoader.BoardSize - 1;
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
                    x += (sbyte)(fenString[i] - '0');
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

        private static Piece GetPieceFromChar(char character)
        {
            var isWhite = char.IsUpper(character);

            var type = char.ToLower(character) switch
            {
                'p' => Piece.Types.Pawn,
                'b' => Piece.Types.Bishop,
                'k' => Piece.Types.King,
                'n' => Piece.Types.Knight,
                'q' => Piece.Types.Queen,
                'r' => Piece.Types.Rook,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new Piece(isWhite, type);
        }
    }
}
