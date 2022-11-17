using Antichess.Core.Pieces;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.AI
{
    public class Promotion : MonoBehaviour
    {
        public Button Pawn, Knight, Bishop, Queen, King, Close;
        public Piece.Types Selection { get; private set; }

        private void Awake()
        {
            Selection = Piece.Types.None;
            Close.onClick.AddListener(OnCloseButtonPress);
            Pawn.onClick.AddListener(delegate { OnPieceClick(Piece.Types.Pawn); });
            Knight.onClick.AddListener(delegate { OnPieceClick(Piece.Types.Knight); });
            Bishop.onClick.AddListener(delegate { OnPieceClick(Piece.Types.Bishop); });
            Queen.onClick.AddListener(delegate { OnPieceClick(Piece.Types.Queen); });
            King.onClick.AddListener(delegate { OnPieceClick(Piece.Types.King); });
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