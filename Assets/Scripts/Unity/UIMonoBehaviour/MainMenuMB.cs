using Antichess.Core;
using Antichess.PlayerTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.Unity.UIMonoBehaviour
{
    public class MainMenuMB : MonoBehaviour
    {
        public TMP_Dropdown bDropdown;
        public GameObject bSlider;
        public Button startButton;
        public TMP_Dropdown wDropdown;
        public GameObject wSlider;

        public Player GetWhitePlayer(RenderedBoard board)
        {
            return wDropdown.value == 0
                ? new User(board, true)
                : new AIPlayer(board, true, wSlider.GetComponent<Slider>().value);
        }

        public Player GetBlackPlayer(RenderedBoard board)
        {
            return bDropdown.value == 1
                ? new User(board, false)
                : new AIPlayer(board, false, bSlider.GetComponent<Slider>().value);
        }

        public void OnWDropdownChange(int val)
        {
            wSlider.SetActive(val == 1);
        }

        public void OnBDropdownChange(int val)
        {
            bSlider.SetActive(val == 0);
        }
    }
}
