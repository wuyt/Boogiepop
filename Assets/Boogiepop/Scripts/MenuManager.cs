using UnityEngine;
using UnityEngine.SceneManagement;

namespace Boogiepop
{
    /// <summary>
    /// 菜单控制
    /// </summary>
    public class MenuManager : MonoBehaviour
    {
        /// <summary>
        /// 系统变量
        /// </summary>
        private GameVariable gameVariable;
        /// <summary>
        /// 开始的文本提示
        /// </summary>
        public GameObject textStart;
        /// <summary>
        /// 完成的文本提示
        /// </summary>
        public GameObject textComplete;

        void Start()
        {
            //获取系统变量
            gameVariable = FindObjectOfType<GameVariable>();

            //设置显示文本
            if (gameVariable.IsCompleteGame)
            {
                textStart.SetActive(false);
                textComplete.SetActive(true);
            }
            else
            {
                textStart.SetActive(true);
                textComplete.SetActive(false);
            }
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene("Main");
        }
        /// <summary>
        /// 应用退出
        /// </summary>
        public void Quit()
        {
            //Application.Quit();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        /// <summary>
        /// 重置游戏
        /// </summary>
        public void Reset()
        {
            gameVariable.IsCompleteGame = false;

            //重置完成后重启游戏
            SceneManager.LoadScene("Menu");
        }
    }
}

