using System.Collections.Generic;
using System.Linq;
using Antichess.Pieces;
using Antichess.TargetSquares;
using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

namespace Antichess
{
    // This performs the actual logical operations of the board
    public class Board
    {
        public static readonly Position Size = new(8, 8);

        private readonly List<Position> _blackPieceLocations;
        private readonly Piece[,] _data;

        private readonly Dictionary<Position, List<Position>> _legalMoves;

        private readonly List<Position> _whitePieceLocations;
        private bool _couldTake;
        public bool CanTake;
        public Position EnPassantTargettableSquare;

        public Board()
        {
            _data = new Piece[Size.x, Size.y];
            _whitePieceLocations = new List<Position>();
            _blackPieceLocations = new List<Position>();
            _legalMoves = new Dictionary<Position, List<Position>>();
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ProcessFenString("8/6P1/8/8/1k6/8/7K/8 w - - 0 1");
            // ReSharper restore StringLiteralTypo
            UpdateLegalMoves();
        }

        public int NumPositionsFrom => _legalMoves.Count;

        public bool WhitesMove { get; private set; }

        public Dictionary<Position, List<Position>>.Enumerator GetLegalMoveEnumerator()
        {
            return _legalMoves.GetEnumerator();
        }

        public Position GetLegalMoveTo(Position from, int moveToIndex)
        {
            return _legalMoves[from][moveToIndex];
        }

        public int NumPositionsTo(Position from)
        {
            return _legalMoves[from].Count;
        }

        public virtual void OnNewFrame() { }

        public static void AddLegalMove(Move move, Board board, Dictionary<Position, List<Position>> legalMoves)
        {
            var pieceFrom = board.PieceAt(move.From);
            if (pieceFrom == null) return;
            if (board._couldTake != board.CanTake)
            {
                legalMoves.Clear();
                board._couldTake = true;
            }

            if (legalMoves.ContainsKey(move.From))
                legalMoves[move.From].Add(move.To);
            else
                legalMoves[move.From] = new List<Position> {move.To};
        }

        private void UpdateLegalMoves()
        {
            _couldTake = false;
            CanTake = false;
            _legalMoves.Clear();

            var pieceLocations = WhitesMove ? _whitePieceLocations : _blackPieceLocations;
            foreach (var pos in pieceLocations) PieceAt(pos).AddMoves(pos, this, _legalMoves);

            EnPassantTargettableSquare = null;
        }

        public bool IsLegal(Move move)
        {
            return _legalMoves.ContainsKey(move.From) && _legalMoves[move.From].Any(p => move.To == p);
        }

        private string LegalMovesToString()
        {
            return string.Join("; ", _legalMoves.Select(pair => $"{pair.Key} => {string.Join(", ", pair.Value)}"));
        }

        public Piece PieceAt(Position pos)
        {
            return _data[pos.x, pos.y];
        }

        private void AddPieceGenerally(Piece piece, Position pos)
        {
            pos = new Position(pos.x, pos.y);
            var pieceLocation = piece.IsWhite ? _whitePieceLocations : _blackPieceLocations;
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
