using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Antichess.Unity.UIMonoBehaviour
{
    public class GameOverMB : MonoBehaviour
    {
        public Button PlayAgain,
            Exit;
        public TMP_Text GameOverText;

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
