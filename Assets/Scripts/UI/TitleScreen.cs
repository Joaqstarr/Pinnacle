using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class TitleScreen : MonoBehaviour
    {

        public void PlayGame()
        {
            SceneManager.LoadScene(1);
        }

        public void ChangeKey(string key)
        {
            PlayerPrefs.SetString("gameKey", key);
        }
    }
}