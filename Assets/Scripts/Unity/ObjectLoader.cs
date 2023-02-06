using UnityEngine;
using Random = System.Random;

namespace Antichess.Unity
{
    /// <summary>
    /// Loads in references to various unity assets, including those in the current scene, such as
    /// the 3d camera, the 3d models for all of the pieces, etc.
    /// </summary>
    internal class ObjectLoader : MonoBehaviour
    {
        public const byte BoardSize = 8;
        public Camera cam;
        public AudioSource audioSource;
        public Transform WhiteCameraTransform;
        public Transform BlackCameraTransform;

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

        public AudioClip move,
            capture;

        public Random Rand;

        public Material wBaseMaterial,
            wEmissiveMaterial,
            bBaseMaterial,
            bEmissiveMaterial;

        public static ObjectLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            Rand = new Random();
        }
    }
}
