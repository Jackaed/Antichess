using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Antichess.Unity.UIMonoBehaviour
{
    /// <summary>
    /// This class is attached to the GameOver object within the Unity editor.
    /// </summary>
    public class GameOverMB : MonoBehaviour
    {
        // These fields refer to the buttons on the GameOver UI object itself within the Unity
        // Editor.
        public Button PlayAgain,
            Exit;
        public TMP_Text GameOverText;

        /// <summary>
        /// This function gets called whenever the GameOver object is created within Unity.
        /// </summary>
        public void Awake()
        {
            Exit.onClick.AddListener(Application.Quit);
            PlayAgain.onClick.AddListener(
                delegate
                {
                    Object.Destroy(this.gameObject);
                }
            );
        }
    }
}
