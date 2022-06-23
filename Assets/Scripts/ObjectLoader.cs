using System;
using System.Collections.Generic;
using Antichess.Pieces;
using Antichess.TargetSquares;
using UnityEngine;

namespace Antichess
{
    internal class ObjectLoader : MonoBehaviour
    {
        public static Dictionary<Type, bool> UIButtonsDown = new()
        {
            {typeof(Rook), false},
            {typeof(Queen), false},
            {typeof(Knight), false},
            {typeof(King), false},
            {typeof(Bishop), false}
        };

        public GameObject bPawn,
            bBishop,
            bKnight,
            bRook,
            bQueen,
            bKing,
            bPromotionUI,
            wPawn,
            wBishop,
            wKnight,
            wRook,
            wQueen,
            wKing,
            wPromotionUI;


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

        public static Vector3 GetRealCoords(Position boardCoords)
        {
            return new Vector3(GetRealCoord(boardCoords.x), 0, (boardCoords.y - 3.5f) * 0.6f);
        }

        private static byte GetBoardCoord(float num)
        {
            var coord = Math.Round(num / 0.6f + 3.5f);
            return (byte) Math.Clamp(coord, 0, 7);
        }

        public static Position GetBoardCoords(Vector3 realCoords)
        {
            return new Position(GetBoardCoord(realCoords.x), GetBoardCoord(realCoords.z));
        }
    }
}
