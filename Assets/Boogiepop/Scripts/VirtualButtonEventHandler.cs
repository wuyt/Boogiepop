using UnityEngine;
using Vuforia;

namespace Boogiepop
{
    /// <summary>
    /// 虚拟按钮事件
    /// </summary>
    public class VirtualButtonEventHandler : MonoBehaviour, IVirtualButtonEventHandler
    {

        /// <summary>
        /// 虚拟按钮行为类
        /// </summary>
        VirtualButtonBehaviour virtualButtonBehaviour;
        /// <summary>
        /// 游戏逻辑控制
        /// </summary>
        private GameObject gameManager;
        /// <summary>
        /// 按钮对应颜色
        /// </summary>
        public Color btnColor;

        void Start()
        {
            virtualButtonBehaviour = GetComponent<VirtualButtonBehaviour>();
            virtualButtonBehaviour.RegisterEventHandler(this);  //注册事件

            gameManager = GameObject.Find("/GameManager");
        }

        /// <summary>
        /// 当虚拟按钮被挡住
        /// </summary>
        /// <param name="vb">虚拟按钮行为类</param>
        public void OnButtonPressed(VirtualButtonBehaviour vb)
        {
            gameManager.SendMessage("BtnOnPress", btnColor);
        }
        /// <summary>
        /// 当虚拟按钮遮挡离开
        /// </summary>
        /// <param name="vb">虚拟按钮行为类</param>
        public void OnButtonReleased(VirtualButtonBehaviour vb)
        {
            gameManager.SendMessage("BtnOnRelease");
        }
    }
}
