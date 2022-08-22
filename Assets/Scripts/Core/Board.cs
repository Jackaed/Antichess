using System;
using System.Collections.Generic;
using Antichess.Core.Pieces;
using Antichess.Unity;
using JetBrains.Annotations;
using UnityEngine;

namespace Antichess.Core
{
    public class Board
    {
        private readonly Piece[,] _data;

        private readonly Stack<BoardChange> _moveHistory;

        private readonly List<ulong> _repetitionHistory;

        protected readonly LegalMoves LegalMoves;

        protected readonly PieceLocations PieceLocations;
        protected ushort LastIrreversibleMove;

        private ushort _moveCounter;

        private Winners _winner;
        private bool _winnerOutdated;

        private ulong _zobristBlacksMove;
        private ulong _zobristHash;
        private bool _zobristOutdated;
        private ulong[,,] _zobristTable;
        
        public bool WhitesMove { get; private set; }
        
        public Position EnPassantTargetSquare { get; private set; }
        
        protected Board()
        {
            _data = new Piece[ObjectLoader.BoardSize, ObjectLoader.BoardSize];
            PieceLocations = new PieceLocations(this);
            LegalMoves = new LegalMoves(this, PieceLocations);
            _moveHistory = new Stack<BoardChange>();
            // ReSharper disable StringLiteralTypo
            //ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ReSharper restore StringLiteralTypo
            InitZobrist();
            _repetitionHistory = new List<ulong> {ZobristHash};
            _winner = Winners.None;
            _winnerOutdated = true;
            _zobristOutdated = true;
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
            _winner = toClone._winner;
            _repetitionHistory = new List<ulong>(toClone._repetitionHistory);
        }
        
        public Piece PieceAt(Position pos)
        {
            return _data[pos.X, pos.Y];
        }
        
        private void Add(Piece piece, Position pos)
        {
            PieceLocations.Remove(pos, PieceAt(pos));
            PieceLocations.Add(pos, piece);

            _data[pos.X, pos.Y] = piece;

            OnChange();
        }

        private void Remove(Position pos)
        {
            var piece = PieceAt(pos);

            if (piece == null) return;

            PieceLocations.Remove(pos, piece);
            _data[pos.X, pos.Y] = null;

            OnChange();
        }
        
        // Whenever the board state changes, this makes various aspects of the program outdated, so they will be updated
        // upon next access.
        private void OnChange()
        {
            LegalMoves.OnBoardChange();
            _zobristOutdated = true;
            _winnerOutdated = true;
        }

        // Equivalents of Add() but creates a new GameObject if this is a RenderedBoard.
        protected virtual void Create([NotNull] Piece piece, [NotNull] Position pos)
        {
            Add(piece, pos);
        }

        // Equivalent of Remove() but destroys the piece's GameObject in the RenderedBoard override.
        protected virtual void Destroy([NotNull] Position pos)
        {
            Remove(pos);
        }

        // Makes a move with legality checks
        public virtual bool Move(Move move)
        {
            if (!LegalMoves.IsLegal(move) || Winner != Winners.None) return false;

            UnsafeMove(move);
            return true;
        }
        
        // Makes a move, but does not check if it is legal or not. Used in instances when we already know a move is 
        // legal, such as when suggesting moves by iterating over the LegalMoves dictionary.
        protected virtual void UnsafeMove(Move move)
        {
            _moveCounter++;

            _moveHistory.Push(new BoardChange(move, PieceAt(move.To),
                EnPassantTargetSquare == null ? null : EnPassantTargetSquare.Clone(), LastIrreversibleMove));

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

            if (PieceAt(move.To) != null || PieceAt(move.From).Type == Piece.Types.Pawn)
                LastIrreversibleMove = _moveCounter;


            Destroy(move.To);
            Add(PieceAt(move.From), move.To);
            Remove(move.From);
            WhitesMove = !WhitesMove;

            _repetitionHistory.Add(ZobristHash);
        }
        
        protected void UndoLastMove()
        {
            UndoMove(_moveHistory.Pop());
            _repetitionHistory.RemoveAt(_repetitionHistory.Count - 1);
        }
        
        protected virtual void UndoMove(BoardChange change)
        {
            EnPassantTargetSquare = change.OldEnPassantTarget;

            Add(PieceAt(change.Move.To), change.Move.From);
            Remove(change.Move.To);

            if (change.Taken != null) Create(change.Taken, change.Move.To);

            WhitesMove = !WhitesMove;

            if (change.Move.Flag == Core.Move.Flags.EnPassant)
                Create(new Piece(!WhitesMove, Piece.Types.Pawn), change.Move.To - Position.Ahead(WhitesMove));

            if (change.Move is Promotion promotion)
                Create(new Piece(WhitesMove, Piece.Types.Pawn), change.Move.From);

            _moveCounter--;
            LastIrreversibleMove = change.LastIrreversibleMove;
        }
        
        public enum Winners : byte
        {
            White,
            Black,
            Stalemate,
            None
        }
        
        public Winners Winner
        {
            get
            {
                if (_winnerOutdated)
                    UpdateWinner();

                return _winner;
            }
        }
        
        protected virtual void UpdateWinner()
        {
            var count = 0;
            for (var i = _moveCounter - 4; i >= LastIrreversibleMove; i -= 2)
            {
                if (_repetitionHistory[_moveCounter] == _repetitionHistory[i])
                    count++;
                if (count < 2) continue;
                _winner = Winners.Stalemate;
                _winnerOutdated = false;
                return;
            }

            if (LastIrreversibleMove == _moveCounter - 50)
            {
                _winner = Winners.Stalemate;
            }
            else
            {
                if (LegalMoves.Count == 0)
                    _winner = WhitesMove ? Winners.White : Winners.Black;
                else
                    _winner = Winners.None;
            }

            _winnerOutdated = false;
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
        }

        protected ulong ZobristHash
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
        
        // Sets board's data to the data fed in from a FEN string. Useful for making boards from standard positions, 
        // e.g. the board's starting position.
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