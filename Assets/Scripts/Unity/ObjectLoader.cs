using UnityEngine;
using Random = System.Random;

namespace Antichess.Unity
{
    internal class ObjectLoader : MonoBehaviour
    {
        public const byte BoardSize = 8;

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
            wPromotionUI,
            mainMenuUI,
            gameOverUI,
            legalMoveIndicator;

        public Material wBaseMaterial, wEmissiveMaterial, bBaseMaterial, bEmissiveMaterial;
        public AudioClip move, capture;
        public AudioSource audioSource;

        public Random Rand;


        public static ObjectLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            Rand = new Random();
        }
    }
}