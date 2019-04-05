using System;
using UnityEngine;

namespace Boogiepop
{
    /// <summary>
    /// 识别后发送的信息
    /// </summary>
    [Serializable]
    public class SendInformation
    {
        /// <summary>
        /// 文本数组
        /// </summary>
        public string[] texts;
        /// <summary>
        /// 音频数组
        /// </summary>
        public AudioClip[] audioClips;
        /// <summary>
        /// 状态转换
        /// </summary>
        public bool changeStatus;
    }
}
