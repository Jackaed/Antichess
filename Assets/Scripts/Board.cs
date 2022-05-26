using System.Collections.Generic;
using System.Linq;
using Antichess.Pieces;
using Antichess.TargetSquares;
using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

namespace Antichess
{
    public class Board
    {
        public static readonly Position Size = new(8, 8);
        private readonly Piece[,] _data;
        private Dictionary<Position, List<Position>> _legalMoves;
        public bool CanTake;
        private bool _couldTake;
        
        protected Board()
        {
            _data = new Piece[Size.x, Size.y];
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            UpdateLegalMoves();
            // ReSharper restore StringLiteralTypo
        }

        private bool WhitesMove { get; set; }

        public void AddLegalMove(Move move)
        {
            var pieceFrom = PieceAt(move.From);
            if (pieceFrom == null) return;
            if (_couldTake != CanTake)
            {
                _legalMoves.Clear();
                _couldTake = true;
            }

            if (_legalMoves.ContainsKey(move.From))
            {
                _legalMoves[move.From].Add(move.To);
            }
            else
            {
                _legalMoves[move.From] = new List<Position> {move.To};
            }
        }

        private void UpdateLegalMoves()
        {
            _couldTake = false;
            CanTake = false;
            _legalMoves = new Dictionary<Position, List<Position>>();
            for (byte x = 0; x < Size.x; x++)
            for (byte y = 0; y < Size.y; y++)
            {
                var pos = new Position(x, y);
                var pieceFrom = PieceAt(pos);


                if (pieceFrom == null || pieceFrom.IsWhite != WhitesMove) continue;
                pieceFrom.AddMoves(pos, this);
            }
        }

        private bool IsLegal(Move move)
        {
            Debug.Log(LegalMovesToString());
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

        protected virtual void AddPiece(Piece piece, Position pos)
        {
            _data[pos.x, pos.y] = piece;
        }

        public virtual bool MovePiece(Move move)
        {
            if (!IsLegal(move))
            {
                Debug.Log("Illegal move");
                return false;
            }

            _data[move.To.x, move.To.y] = PieceAt(move.From);
            _data[move.From.x, move.From.y] = null;
            WhitesMove = !WhitesMove;
            UpdateLegalMoves();
            return true;
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