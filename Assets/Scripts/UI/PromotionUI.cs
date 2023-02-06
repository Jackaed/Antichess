using Antichess.Core;
using Antichess.Core.Pieces;
using UnityEngine;
using Antichess.Unity;
using Antichess.Unity.UIMonoBehaviour;

namespace Antichess.UI
{
    public class PromotionUI
    {
        private readonly GameObject _promotionGO;
        private readonly PromotionMB _promotionMB;

        public PromotionUI(bool isWhite, Camera cam, Move move)
        {
            _promotionGO = Object.Instantiate(
                isWhite ? ObjectLoader.Instance.wPromotionUI : ObjectLoader.Instance.bPromotionUI
            );
            var canvas = _promotionGO.GetComponent<Canvas>();
            canvas.worldCamera = cam;
            var transform = _promotionGO.GetComponent<RectTransform>();
            transform.position = RenderedBoard.GetRealCoords(move.To) + (0.5f * Vector3.up);
            _promotionMB = _promotionGO.GetComponent<PromotionMB>();
        }

        /// <summary>
        /// Gives the piece type that has been selected to promote to
        /// </summary>
        /// <value></value>
        public Piece.Types Selection {
            get {
                Piece.Types selection = _promotionMB.Selection;

                if(selection == Piece.Types.None) {
                    return Piece.Types.None;
                }

                Object.Destroy(_promotionGO);

                return selection;

            }
        }
        public bool IsCancelled => _promotionMB == null;
    }
}
