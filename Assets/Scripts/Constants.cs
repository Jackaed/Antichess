using System;
using System.Collections.Generic;
using Antichess.Pieces;
using Antichess.PositionTypes;
using UnityEngine;

namespace Antichess
{
    internal class Constants : MonoBehaviour
    {
        public static readonly double MoveSpeed = 25.0;
        public static readonly byte BoardSize = 8;
        
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


        public static Constants Instance { get; private set; }

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
            return new Vector3(GetRealCoord(boardCoords.X), 0, (boardCoords.Y - 3.5f) * 0.6f);
        }

        private static sbyte GetBoardCoord(float num)
        {
            var coord = Math.Round(num / 0.6f + 3.5f);
            return (sbyte) Math.Clamp(coord, 0, Constants.BoardSize);
        }

        public static Position GetBoardCoords(Vector3 realCoords)
        {
            return new Position(GetBoardCoord(realCoords.x), GetBoardCoord(realCoords.z));
        }
    }
}
