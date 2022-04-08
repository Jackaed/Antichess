using UnityEngine;

// ReSharper disable VirtualMemberCallInConstructor

public class Board
{
    private static readonly Vector2Int Size = new(8, 8);
    protected readonly IPiece[,] Data;
    public bool WhitesMove;

    protected Board()
    {
        Data = new IPiece[Size.x, Size.y];
        // ReSharper disable once VirtualMemberCallInConstructor
        AddPiece(new King(true), new Vector2Int(4, 0));
        AddPiece(new King(false), new Vector2Int(4, 7));
        AddPiece(new Queen(true), new Vector2Int(3, 0));
        AddPiece(new Queen(false), new Vector2Int(3, 7));
        AddPiece(new Bishop(true), new Vector2Int(2, 0));
        AddPiece(new Bishop(true), new Vector2Int(5, 0));
        AddPiece(new Bishop(false), new Vector2Int(2, 7));
        AddPiece(new Bishop(false), new Vector2Int(5, 7));
        AddPiece(new Knight(true), new Vector2Int(1, 0));
        AddPiece(new Knight(true), new Vector2Int(6, 0));
        AddPiece(new Knight(false), new Vector2Int(1, 7));
        AddPiece(new Knight(false), new Vector2Int(6, 7));
        AddPiece(new Rook(true), new Vector2Int(0, 0));
        AddPiece(new Rook(true), new Vector2Int(7, 0));
        AddPiece(new Rook(false), new Vector2Int(0, 7));
        AddPiece(new Rook(false), new Vector2Int(7, 7));

        for (var i = 0; i < 8; i++)
        {
            AddPiece(new Pawn(true), new Vector2Int(i, 1));
            AddPiece(new Pawn(false), new Vector2Int(i, 6));
        }
    }

    public IPiece PieceAt(Vector2Int pos)
    {
        return Data[pos.x, pos.y];
    }

    protected virtual void AddPiece(IPiece piece, Vector2Int pos)
    {
        Data[pos.x, pos.y] = piece;
    }

    public virtual void MovePiece(Move move)
    {
        Data[move.to.x, move.to.y] = Data[move.from.x, move.from.y];
        Data[move.from.x, move.from.y] = null;
    }
}
