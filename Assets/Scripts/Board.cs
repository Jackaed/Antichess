using System.Collections.Generic;
using Antichess.Pieces;
using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

namespace Antichess
{
    public class Board
    {
        private static readonly Vector2Int Size = new(8, 8);
        private readonly IPiece[,] _data;

        private List<Move> LegalMoves
        {
            get
            {
                bool canTake = false;
                var moves = new List<Move>();
                for(var x = 0 ; x < Size.x; x++)
                {
                    for (var y = 0; y < Size.y; y++)
                    {
                        var pos = new Vector2Int(x, y);
                        moves.AddRange(PieceAt(pos).GetLegalMoves(pos, this));
                    }
                }

                return moves;
            }
        }

        private bool IsLegal(Move move)
        {
            var pieceFrom = PieceAt(move.From);
            var pieceTo = PieceAt(move.To);
            return (pieceFrom != null && pieceFrom.IsWhite == WhitesMove &&
                    (pieceTo == null || pieceTo.IsWhite != WhitesMove));
        }
        
        protected Board()
        {
            _data = new IPiece[Size.x, Size.y];
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            // ReSharper restore StringLiteralTypo
        }

        public bool WhitesMove { get; private set; }

        public IPiece PieceAt(Vector2Int pos)
        {
            return _data[pos.x, pos.y];
        }
        


        protected virtual void AddPiece(IPiece piece, Vector2Int pos)
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
            return true;

        }

        private static IPiece GetPieceFromChar(char character)
        {
            var isWhite = char.IsUpper(character);

            IPiece piece = char.ToLower(character) switch
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
            //  Returns a board from a FEN string.
            // Note that there is no castling in antichess, so these sections of the fen string are ignored.

            var y = Size.y - 1;
            var x = 0;
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
                    x += fenString[i] - '0';
                }
                else
                {
                    AddPiece(GetPieceFromChar(fenString[i]), new Vector2Int(x, y));
                    x++;
                }

                i++;
            }

            i++; // Skips the space

            WhitesMove = fenString[i] == 'w';
        }
    }
}
