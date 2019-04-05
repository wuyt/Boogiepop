using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace Boogiepop
{
    /// <summary>
    /// 识别追踪处理
    /// </summary>
    public class TrackableEventHandler : DefaultTrackableEventHandler
    {
        /// <summary>
        /// 场景逻辑控制
        /// </summary>
        private GameObject gameManager;
        /// <summary>
        /// 识别后发送的信息
        /// </summary>
        public SendInformation information;
        /// <summary>
        /// 重写Start事件
        /// </summary>
        protected override void Start()
        {
            gameManager = GameObject.Find("/GameManager");
            base.Start();
        }
        /// <summary>
        /// 发现识别象
        /// </summary>
        protected override void OnTrackingFound()
        {
            base.OnTrackingFound();
            gameManager.SendMessage("OnTrackingFound", information);
        }
        /// <summary>
        /// 识别对象丢失
        /// </summary>
        protected override void OnTrackingLost()
        {
            base.OnTrackingLost();
            gameManager.SendMessage("OnTrackingLost");
        }
    }
}

