using UnityEngine;
using UnityEngine.SceneManagement;

namespace Boogiepop
{
    public class egg : MonoBehaviour
    {

        public void BackMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

}