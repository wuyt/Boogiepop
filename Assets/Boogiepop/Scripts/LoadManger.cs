using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Boogiepop
{
    /// <summary>
    /// 启动场景管理
    /// </summary>
    public class LoadManger : MonoBehaviour
    {

        /// <summary>
        /// 滚动条
        /// </summary>
        private Slider slider;
        /// <summary>
        /// 异步操作对象
        /// </summary>
        private AsyncOperation async;

        void Start()
        {
            slider = FindObjectOfType<Slider>();    //找到场景中的滚动条并赋值
            StartCoroutine("LoadMenuScene");        //启动协程
        }

        /// <summary>
        /// 加载主菜单
        /// </summary>
        /// <returns>异步操作对象</returns>
        IEnumerator LoadMenuScene()
        {
            async = SceneManager.LoadSceneAsync("Menu");    //异步加载主菜单
            async.allowSceneActivation = false;     //停止自动跳转
            while (async.progress < 0.9f)       //没有加载完成则继续加载
            {
                slider.value = async.progress;      //将异步加载进度赋值给滚动条   
                yield return null;
            }
            yield return new WaitForSeconds(1f);    //等待1秒
            async.allowSceneActivation = true;      //场景跳转
        }
    }
}