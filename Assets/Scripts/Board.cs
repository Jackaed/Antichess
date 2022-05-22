using System.Collections.Generic;
using System.Linq;
using Antichess.Pieces;
using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

namespace Antichess
{
    public class Board
    {
        public static readonly Vector2Int Size = new(8, 8);
        private readonly Piece[,] _data;
        private Dictionary<Vector2Int, List<Vector2Int>> _legalMoves;

        protected Board()
        {
            _data = new Piece[Size.x, Size.y];
            // ReSharper disable StringLiteralTypo
            ProcessFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            UpdateLegalMoves();
            // ReSharper restore StringLiteralTypo
        }

        private bool WhitesMove { get; set; }

        private void UpdateLegalMoves()
        {
            var canTake = false;
            var lastCouldTake = false;
            var moves = new Dictionary<Vector2Int, List<Vector2Int>>();
            for (var x = 0; x < Size.x; x++)
            for (var y = 0; y < Size.y; y++)
            {
                var pos = new Vector2Int(x, y);
                var pieceFrom = PieceAt(pos);


                if (pieceFrom == null || pieceFrom.IsWhite != WhitesMove) continue;
                var pieceMoves = pieceFrom.GetMoves(pos, this, canTake);
                canTake = pieceMoves.CanTake || canTake;

                if (lastCouldTake != canTake)
                {
                    moves = new Dictionary<Vector2Int, List<Vector2Int>>();
                    lastCouldTake = true;
                }

                if (pieceMoves.MoveList.Count != 0) moves[pos] = pieceMoves.MoveList;
            }

            _legalMoves = moves;
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

        public Piece PieceAt(Vector2Int pos)
        {
            return _data[pos.x, pos.y];
        }

        protected virtual void AddPiece(Piece piece, Vector2Int pos)
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