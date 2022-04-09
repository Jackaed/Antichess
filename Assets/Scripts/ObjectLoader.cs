using System;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    public GameObject bPawn,
        bBishop,
        bKnight,
        bRook,
        bQueen,
        bKing,
        wPawn,
        wBishop,
        wKnight,
        wRook,
        wQueen,
        wKing;

    public static ObjectLoader Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public static Vector3 GetRealCoords(Vector2Int boardCoords)
    {
        return new Vector3((boardCoords.x - 3.5f) * 0.6f, 0, (boardCoords.y - 3.5f) * 0.6f);
    }

    public static Vector2Int GetBoardCoords(Vector3 realCoords)
    {
        return new Vector2Int((int) Math.Clamp(Math.Round(realCoords.x / 0.6f + 3.5f), 0, 7),
            (int) Math.Clamp(Math.Round(realCoords.z / 0.6f + 3.5f), 0, 7));
    }
}
