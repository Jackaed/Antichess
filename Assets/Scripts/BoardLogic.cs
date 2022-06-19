using System;
using System.Collections.Generic;
using System.Linq;
using Antichess.Pieces;
using Antichess.TargetSquares;
using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

namespace Antichess
{
    // This performs the actual logical operations of the board
    public class BoardLogic
    {
        public static readonly Position Size = new(8, 8);
        private readonly Piece[,] _data;
        private bool _couldTake;
        public bool CanTake;
        public Position EnPassantTargettableSquare;

        public BoardLogic()
        {
            _data = new Piece[Size.x, Size.y];
            _whitePieceLocations = new List<Position>();
            _blackPieceLocations = new List<Position>();
            LegalMoves = new Dictionary<Position, List<Position>>();
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            // ReSharper restore StringLiteralTypo
            UpdateLegalMoves();
            
        }

        public Dictionary<Position, List<Position>> LegalMoves { get; private set; }

        public bool WhitesMove { get; private set; }
        
        private readonly List<Position> _whitePieceLocations;
        
        private readonly List<Position> _blackPieceLocations;

        public virtual void OnNewFrame() { }

        public void AddLegalMove(Move move)
        {
            var pieceFrom = PieceAt(move.From);
            if (pieceFrom == null) return;
            if (_couldTake != CanTake)
            {
                LegalMoves.Clear();
                _couldTake = true;
            }

            if (LegalMoves.ContainsKey(move.From))
                LegalMoves[move.From].Add(move.To);
            else
                LegalMoves[move.From] = new List<Position> {move.To};
        }

        private void UpdateLegalMoves()
        {
            _couldTake = false;
            CanTake = false;
            LegalMoves.Clear();

            var pieceLocations = WhitesMove ? _whitePieceLocations : _blackPieceLocations;
            foreach(var pos in pieceLocations)
            {
                if (PieceAt(pos) == null)
                {
                    Debug.Log(pos);
                }
                PieceAt(pos).AddMoves(pos, this);
            }

            EnPassantTargettableSquare = null;
        }

        public bool IsLegal(Move move)
        {
            Debug.Log(LegalMovesToString());
            return LegalMoves.ContainsKey(move.From) && LegalMoves[move.From].Any(p => move.To == p);
        }

        private string LegalMovesToString()
        {
            return string.Join("; ", LegalMoves.Select(pair => $"{pair.Key} => {string.Join(", ", pair.Value)}"));
        }

        public Piece PieceAt(Position pos)
        {
            return _data[pos.x, pos.y];
        }

        private void AddPieceGenerally(Piece piece, Position pos)
        {
            pos = new Position(pos.x, pos.y);
            if(pos == new Position(4, 4)) Debug.Log("I did a bad thing! ");
                var pieceLocation = (piece.IsWhite ? _whitePieceLocations : _blackPieceLocations);
            _data[pos.x, pos.y] = piece;
            (piece.IsWhite ? _blackPieceLocations : _whitePieceLocations).Remove(pos);
            pieceLocation.Add(pos);
        }
        
        protected virtual void AddPiece(Piece piece, Position pos)
        {
            AddPieceGenerally(piece, pos);
        }

        public virtual bool MovePiece(Move move)
        {
            if (!IsLegal(move))
            {
                Debug.Log("Illegal move");
                return false;
            }

            var pawnDoubleMove = move.To as PawnDoubleMovePosition;
            if (pawnDoubleMove != null) EnPassantTargettableSquare = pawnDoubleMove.MovedThrough;

            var enPassant = move.To as EnPassantPosition;
            if (enPassant != null) RemovePiece(enPassant.TargetPieceSquare);

            var promotion = move.To as PromotionPosition;
            if (promotion != null) AddPiece(promotion.PromotionPiece, move.From);

            
            AddPieceGenerally(PieceAt(move.From), move.To);
            RemovePieceGenerally(move.From);
            WhitesMove = !WhitesMove;
            UpdateLegalMoves();
            return true;
        }

        // Not overridden in the RenderedBoardLogic class
        private void RemovePieceGenerally(Position pos)
        {
            if(pos == new Position(4, 4)) Debug.Log("I removed a bad thing! ");
            (PieceAt(pos).IsWhite ? _whitePieceLocations : _blackPieceLocations).Remove(new Position(pos.x, pos.y));
            _data[pos.x, pos.y] = null;
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
            var y = (byte) (Size.y - 1);
            byte x = 0;
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
                    x += (byte) (fenString[i] - '0');
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
