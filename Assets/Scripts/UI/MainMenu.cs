using Antichess.Core;
using Antichess.PlayerTypes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Antichess.UI
{
    public class MainMenu : MonoBehaviour
    {
        public TMP_Dropdown wDropdown;
        public TMP_Dropdown bDropdown;
        public GameObject wSlider;
        public GameObject bSlider;
        public Button startButton;

        public Player GetWhitePlayer (RenderedBoard board) {
            
            return wDropdown.value == 0
                ? new User(board, true)
                : new AIPlayer(board, true,
                    wSlider.GetComponent<Slider>().value);
        }

        public Player GetBlackPlayer(RenderedBoard board)
        {
            return bDropdown.value == 1
                ? new User(board, false)
                : new AIPlayer(board, false,
                    bSlider.GetComponent<Slider>().value);
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