using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Antichess.Pieces;
using Antichess.PositionTypes;
using Unity.VisualScripting;
using UnityEngine;

namespace Antichess
{
    public class Board
    {

        private readonly List<Position> _blackPieceLocations;
        private readonly Piece[,] _data;
        private readonly Stack<BoardStateChange> _moveHistory;

        private readonly Dictionary<Position, List<Position>> _legalMoves;

        private readonly List<Position> _whitePieceLocations;

        public Board()
        {
            _data = new Piece[Constants.BoardSize, Constants.BoardSize];
            _whitePieceLocations = new List<Position>();
            _blackPieceLocations = new List<Position>();
            _legalMoves = new Dictionary<Position, List<Position>>();
            _moveHistory = new Stack<BoardStateChange>();
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            // ReSharper restore StringLiteralTypo
            UpdateLegalMoves();
        }

        public void UndoLastMove()
        {
            UnmakeMove(_moveHistory.Pop());
        }

        protected virtual void UnmakeMove(BoardStateChange move)
        {
            EnPassantTargetSquare = move.OldEnPassantTarget;
            
            AddPieceGenerally(PieceAt(move.To), move.From);
            RemovePieceGenerally(move.To);
            WhitesMove = !WhitesMove;
            
            AddPiece(move.Taken, move.To);
            
            var enPassant = move.To as EnPassantPosition;
            if (enPassant != null) AddPiece(new Pawn(!WhitesMove), enPassant.TargetPieceSquare);
            
            var promotion = move.To as PromotionPosition;
            if (promotion != null) AddPiece(new Pawn(WhitesMove), move.From);
            UpdateLegalMoves();
        }

        public bool CanTake { get; private set; }
        public Position EnPassantTargetSquare { get; private set; }

        public int NumPositionsFrom => _legalMoves.Count;

        public bool WhitesMove { get; private set; }

        // Allows for iteration over the whole dictionary, without public access to the dictionary itself. This prevents
        // players from being able to add legal moves.
        public Dictionary<Position, List<Position>>.KeyCollection.Enumerator GetLegalPositionsFrom()
        {
            return _legalMoves.Keys.GetEnumerator();
        }

        // These two methods allow users to view the legal moves of a position, without 
        public List<Position>.Enumerator GetLegalPositionsTo(Position from)
        {
            return _legalMoves[from].GetEnumerator();
        }

        public int NumPositionsTo(Position from)
        {
            return _legalMoves[from].Count;
        }

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

            var pieceLocations = WhitesMove ? _whitePieceLocations : _blackPieceLocations;
            foreach (var pos in pieceLocations) PieceAt(pos).AddMoves(pos, this, _legalMoves);
            
        }

        private bool IsLegal(Move move)
        {
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

        private void AddPieceGenerally(Piece piece, Position pos)
        {
            pos = new Position(pos.X, pos.Y);
            _data[pos.X, pos.Y] = piece;
            if (piece != null)
            {
                var pieceLocation = piece.IsWhite ? _whitePieceLocations : _blackPieceLocations;
            
                (piece.IsWhite ? _blackPieceLocations : _whitePieceLocations).Remove(pos);
                pieceLocation.Add(pos);
            }
        }

        public Board Copy()
        {
            return (Board) ObjectCopier.CloneObject(this);
        }

        protected virtual void AddPiece(Piece piece, Position pos)
        {
            AddPieceGenerally(piece, pos);
        }

        public Dictionary<Position, List<Position>> GetLegalMoves()
        {
            return (Dictionary<Position, List<Position>>) ObjectCopier.CloneObject(_legalMoves);
        }
        
        public int Evaluate ()
        {
            var total = -_whitePieceLocations.Sum(pos => (int) PieceAt(pos).Value);
            return total + _blackPieceLocations.Sum(pos => (int) PieceAt(pos).Value);
        }
        
        protected virtual void MakeMoveWithoutLegalityCheck(Move move)
        {
            _moveHistory.Push(new BoardStateChange(move.From, move.To, PieceAt(move.To), 
                EnPassantTargetSquare == null ? null : EnPassantTargetSquare.Clone()));

            EnPassantTargetSquare = null;
            
            var pawnDoubleMove = move.To as PawnDoubleMovePosition;
            if (pawnDoubleMove != null) EnPassantTargetSquare = pawnDoubleMove.MovedThrough;

            var enPassant = move.To as EnPassantPosition;
            if (enPassant != null) RemovePiece(enPassant.TargetPieceSquare);

            var promotion = move.To as PromotionPosition;
            if (promotion != null) AddPiece(promotion.PromotionPiece, move.From);


            AddPieceGenerally(PieceAt(move.From), move.To);
            RemovePieceGenerally(move.From);
            WhitesMove = !WhitesMove;
            UpdateLegalMoves();
        }

        public bool MovePiece(Move move)
        {
            if (!IsLegal(move))
            {
                Debug.Log("Illegal move");
                return false;
            }
            
            MakeMoveWithoutLegalityCheck(move);
            return true;
        }

        // Not overridden in the RenderedBoardLogic class
        private void RemovePieceGenerally(Position pos)
        {
            if (PieceAt(pos) == null) return;
            (PieceAt(pos).IsWhite ? _whitePieceLocations : _blackPieceLocations).Remove(new Position(pos.X, pos.Y));
            _data[pos.X, pos.Y] = null;
        }

        protected virtual void RemovePiece(Position pos)
        {
            RemovePieceGenerally(pos);
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
                    AddPiece(GetPieceFromChar(fenString[i]), new Position(x, y));
                    x++;
                }

                i++;
            }

            i++; // Skips the space

            WhitesMove = fenString[i] == 'w';
        }
    }
}
