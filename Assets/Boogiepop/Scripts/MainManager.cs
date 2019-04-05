using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Boogiepop
{
    /// <summary>
    /// 主场景逻辑
    /// </summary>
    public class MainManager : MonoBehaviour
    {
        /// <summary>
        /// 音源
        /// </summary>
        public AudioSource audioSource;
        /// <summary>
        /// 文本显示
        /// </summary>
        public Text uiText;
        /// <summary>
        /// 播放序号
        /// </summary>
        public int number;
        /// <summary>
        /// 识别对象发过来的信息
        /// </summary>
        public SendInformation sendInformation;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            uiText = GameObject.FindObjectOfType<Text>();
            uiText.transform.parent.gameObject.SetActive(false);
        }
        /// <summary>
        /// 音频播放
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayAudio()
        {
            //播放音频
            audioSource.clip = sendInformation.audioClips[number];
            audioSource.Play();
            //显示文本
            uiText.transform.parent.gameObject.SetActive(true);
            uiText.text = sendInformation.texts[number];
            //等待音频播放完毕
            yield return new WaitForSeconds(
                sendInformation.audioClips[number].length + 0.5f);
            if (number < sendInformation.audioClips.Length - 1)
            {
                number++;
                StartCoroutine(PlayAudio());
            }
            else
            {
                uiText.text = "";
                uiText.transform.parent.gameObject.SetActive(false);
                if (sendInformation.changeStatus)
                {
                    //TODO
                }
            }
        }

        /// <summary>
        /// 发现识别对象
        /// </summary>
        /// <param name="information">要播放的信息</param>
        void OnTrackingFound(SendInformation information)
        {
            sendInformation = information;
            number = 0;
            StartCoroutine(PlayAudio());
        }
        /// <summary>
        /// 识别对象丢失
        /// </summary>
        void OnTrackingLost()
        {
            //停止播放音频
            audioSource.Stop();
            audioSource.clip = null;
            //停止文字显示
            uiText.text = "";
            uiText.transform.parent.gameObject.SetActive(false);
            sendInformation = null;
            number = 0;
            //停止协程
            StopAllCoroutines();
        }
    }
}

