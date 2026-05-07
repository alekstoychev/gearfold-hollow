using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class CustomSceneManager : MonoBehaviour
    {
        public static void LoadScene(string  sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
