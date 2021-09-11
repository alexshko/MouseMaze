using UnityEngine;
using UnityEngine.SceneManagement;

namespace alexshko.colamazle.core
{
    public class Menu : MonoBehaviour
    {
        public void LoadLevel(string lvl)
        {
            int level = int.Parse(lvl);
            SceneManager.LoadScene(level);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
