using Antichess.Core.Pieces;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.Unity.UIMonoBehaviour
{
    public class PromotionMB : MonoBehaviour
    {
        public Button Knight,
            Bishop,
            Rook,
            Queen,
            King,
            Close;
        public Piece.Types Selection { get; private set; }

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