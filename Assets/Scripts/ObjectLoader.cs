using System;
using UnityEngine;

namespace Antichess
{
    internal class ObjectLoader : MonoBehaviour
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
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private static float GetRealCoord(int boardCoord)
        {
            return 0.6f * (boardCoord - 3.5f);
        }

        public static Vector3 GetRealCoords(Vector2Int boardCoords)
        {
            return new Vector3(GetRealCoord(boardCoords.x), 0, (boardCoords.y - 3.5f) * 0.6f);
        }

        private static int GetBoardCoord(float num)
        {
            var coord = Math.Round(num / 0.6f + 3.5f);
            return (int) Math.Clamp(coord, 0, 7);
        }
        
        public static Vector2Int GetBoardCoords(Vector3 realCoords)
        {
            return new Vector2Int(GetBoardCoord(realCoords.x), GetBoardCoord(realCoords.z));
        }
    }
}
