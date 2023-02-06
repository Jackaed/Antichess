using Antichess.Core;
using Antichess.PlayerTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Antichess.Unity.UIMonoBehaviour
{
    /// <summary>
    /// This class is attached to the Main Menu object in the Unity Editor. This allows other
    /// classes to easily access the various sliders and buttons that are part of the Main Menu
    /// class. To find code relating to the Main Menu UI code itself, go to the MainMenuUI class.
    /// </summary>
    public class MainMenuMB : MonoBehaviour
    {
        // These fields are assigned to within the Unity editor. They represent the various elements
        // within the main menu UI itself (the dropdowns, start button, etc).
        public TMP_Dropdown bDropdown;
        public GameObject bSlider;
        public Button startButton;
        public TMP_Dropdown wDropdown;
        public GameObject wSlider;

        /// <summary>
        /// Creates and returns the white player that is currently selected within the main menu
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public Player GetWhitePlayer(RenderedBoard board)
        {
            return wDropdown.value == 0
                ? new User(board, true)
                : new AIPlayer(board, true, wSlider.GetComponent<Slider>().value);
        }

        /// <summary>
        /// Creates and returns the black player that is currently selected within the main menu
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public Player GetBlackPlayer(RenderedBoard board)
        {
            return bDropdown.value == 1
                ? new User(board, false)
                : new AIPlayer(board, false, bSlider.GetComponent<Slider>().value);
        }

        /// <summary>
        /// Called whenever the value of the white player's dropdown changes. If the dropdown
        /// changes to an AI player, the slider for the AI player difficulty is set active. If it
        /// doesn't, it is set as inactive, and hidden.
        /// </summary>
        /// <param name="val"></param>
        public void OnWDropdownChange(int val)
        {
            wSlider.SetActive(val == 1);
        }

        /// <summary>
        /// Called whenever the value of the white player's dropdown changes. If the dropdown
        /// changes to an AI player, the slider for the AI player difficulty is set active. If it
        /// doesn't, it is set as inactive, and hidden.
        /// </summary>
        /// <param name="val"></param>
        public void OnBDropdownChange(int val)
        {
            bSlider.SetActive(val == 0);
        }
    }
}
