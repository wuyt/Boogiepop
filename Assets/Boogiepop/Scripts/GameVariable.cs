﻿using UnityEngine;

namespace Boogiepop
{
    /// <summary>
    /// 系统变量
    /// </summary>
    public class GameVariable : MonoBehaviour
    {
        /// <summary>
        /// 是否完成游戏
        /// </summary>
        public bool IsCompleteGame;
        /// <summary>
        /// 完成的类型
        /// </summary>
        public bool[] CompleteTypes;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}