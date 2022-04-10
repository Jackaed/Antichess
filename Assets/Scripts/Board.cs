using System;
using Antichess.Pieces;
using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

namespace Antichess
{
    public class Board
    {
        private static readonly Vector2Int Size = new(8, 8);
        protected readonly IPiece[,] Data;
        public bool WhitesMove { get; private set; }

        public Board()
        {
            Data = new IPiece[Size.x, Size.y];
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public IPiece PieceAt(Vector2Int pos)
        {
            return Data[pos.x, pos.y];
        }

        public virtual void AddPiece(IPiece piece, Vector2Int pos)
        {
            Data[pos.x, pos.y] = piece;
        }

        public virtual void MovePiece(Move move)
        {
            Data[move.To.x, move.To.y] = Data[move.From.x, move.From.y];
            Data[move.From.x, move.From.y] = null;
        }
        
        private static IPiece GetPieceFromChar (char character)
        {
            var isWhite = char.IsUpper(character);
            IPiece piece;
            
            switch (Char.ToLower(character))
            {
                case 'p':
                    piece = new Pawn(isWhite);
                    break;
                case 'b':
                    piece = new Bishop(isWhite);
                    break;
                case 'k':
                    piece = new King(isWhite);
                    break;
                case 'n':
                    piece = new Knight(isWhite);
                    break;
                case 'q':
                    piece = new Queen(isWhite);
                    break;
                case 'r':
                    piece = new Rook(isWhite);
                    break;
                default:
                    piece = null;
                    break;
            }

            return piece;
        }
        
        public void ProcessFenString(String fenString)
        {
            //  Returns a board from a fenstring.
            // Note that there is no castling in antichess, so these sections of the fen string are ignored.

            var y = 7;
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
