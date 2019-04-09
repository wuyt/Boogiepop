using UnityEngine;

namespace Boogiepop
{
    public class tempBtn : MonoBehaviour
    {
        /// <summary>
        /// 游戏逻辑控制
        /// </summary>
        private GameObject gameManager;
        /// <summary>
        /// 按钮对应颜色
        /// </summary>
        public Color btnColor;

        private void Start()
        {
            gameManager = GameObject.Find("/GameManager");
        }
        /// <summary>
        /// 按钮点击事件处理
        /// </summary>
        public void OnPress()
        {
            gameManager.SendMessage("BtnOnPress",btnColor);
        }
        /// <summary>
        /// 按钮松开事件处理
        /// </summary>
        public void OnRelease()
        {
            gameManager.SendMessage("BtnOnRelease");
        }
    }
}