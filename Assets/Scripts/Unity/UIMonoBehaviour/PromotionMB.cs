using Antichess.Core.Pieces;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.Unity.UIMonoBehaviour
{
    /// <summary>
    /// This class is attached to the Promotion prefabs in the Unity Editor. This allows other
    /// classes to easily access the various buttons that are on the Promotion UI object itself. For
    /// code related to the Promotion itself, look at the PromotionUI class.
    /// </summary>
    public class PromotionMB : MonoBehaviour
    {
        /// <summary>
        /// These buttons are assigned to in the editor, meaning that they act as a reference to the
        /// actual UI buttons.
        /// </summary>
        public Button Knight,
            Bishop,
            Rook,
            Queen,
            King,
            Close;
        public Piece.Types Selection { get; private set; }

        /// <summary>
        /// This function gets ran when the object this script is attached to is created.
        /// </summary>
        private void Awake()
        {
            Selection = Piece.Types.None;
            Close.onClick.AddListener(OnCloseButtonPress);
            Knight.onClick.AddListener(
                delegate
                {
                    OnPieceClick(Piece.Types.Knight);
                }
            );
            Bishop.onClick.AddListener(
                delegate
                {
                    OnPieceClick(Piece.Types.Bishop);
                }
            );
            Queen.onClick.AddListener(
                delegate
                {
                    OnPieceClick(Piece.Types.Queen);
                }
            );
            King.onClick.AddListener(
                delegate
                {
                    OnPieceClick(Piece.Types.King);
                }
            );
            Rook.onClick.AddListener(
                delegate
                {
                    OnPieceClick(Piece.Types.Rook);
                }
            );
        }

        public void OnPieceClick(Piece.Types type)
        {
            Selection = type;
        }

        private void OnCloseButtonPress()
        {
            Destroy(gameObject);
        }
    }
}
