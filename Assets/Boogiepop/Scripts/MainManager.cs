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
        #region Play Information
        [Header("信息播放")]

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
        #endregion

        #region Lock
        [Header("解锁")]
        /// <summary>
        /// 当前滚动条
        /// </summary>
        public Slider currentSlider;
        /// <summary>
        /// 当前滚动条状态
        /// </summary>
        public SliderStatus sliderStatus;
        /// <summary>
        /// 计时器
        /// </summary>
        private float timing;
        /// <summary>
        /// 滚动条计数器
        /// </summary>
        private int sliderIndex;
        /// <summary>
        /// 滚动条数组
        /// </summary>
        public Slider[] sliders;
        #endregion

        private void Start()
        {
            PiStart();
        }

        private void Update()
        {
            LockUpdate();
        }

        /// <summary>
        /// 发现识别对象
        /// </summary>
        /// <param name="information">要播放的信息</param>
        void OnTrackingFound(SendInformation information)
        {
            PiOnTrackingFound(information);
            LockStart();
        }
        /// <summary>
        /// 识别对象丢失
        /// </summary>
        void OnTrackingLost()
        {
            PiOnTrackingLost();
        }

        #region Play Information
        /// <summary>
        /// 播放信息启动内容
        /// </summary>
        private void PiStart()
        {
            audioSource = GetComponent<AudioSource>();
            uiText = GameObject.FindObjectOfType<Text>();
            uiText.transform.parent.gameObject.SetActive(false);
        }
        /// <summary>
        /// 播放信息识别处理
        /// </summary>
        /// <param name="information">要播放的信息</param>
        private void PiOnTrackingFound(SendInformation information)
        {
            sendInformation = information;
            number = 0;
            StartCoroutine(PlayAudio());
        }
        /// <summary>
        /// 播放信息识别丢失处理
        /// </summary>
        private void PiOnTrackingLost()
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
        /// <summary>
        /// 音频播放
        /// </summary>
        /// <returns></returns>
        IEnumerator PlayAudio()
        {
            if (sendInformation.audioClips.Length > 0)
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
        }
        #endregion

        #region Lock
        /// <summary>
        /// 解锁开始处理
        /// </summary>
        private void LockStart()
        {
            sliderStatus = SliderStatus.Wait;
            timing = 0;
            sliderIndex = 0;
            currentSlider = sliders[sliderIndex];
        }
        /// <summary>
        /// 解锁Update
        /// </summary>
        private void LockUpdate()
        {
            //处于计时状态
            if (sliderStatus == SliderStatus.Timing)
            {
                //设置滚动条的值
                currentSlider.value = Time.time - timing;

                //如果滚动条满了
                if (currentSlider.value == 1)
                {
                    //判断颜色是否正确
                    if (currentSlider.GetComponent<SliderColor>().color ==
                        currentSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color)
                    {
                        //颜色正确
                        sliderStatus = SliderStatus.Complete;
                        DetectSlider();
                    }
                    else
                    {
                        //颜色错误
                        currentSlider.value = 0;
                        timing = Time.time;
                    }
                }
            }
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="btnColor"></param>
        public void BtnOnPress(Color btnColor)
        {
            sliderStatus = SliderStatus.Timing;     //进入计时状态

            currentSlider.transform.Find("Fill Area/Fill")  //获取Fill子游戏对象
                .GetComponent<Image>()      //获取Image组件
                .color = btnColor;        //修改颜色

            timing = Time.time;
        }
        /// <summary>
        /// 按钮松开
        /// </summary>
        public void BtnOnRelease()
        {
            sliderStatus = SliderStatus.Wait;   //进入等待状态
            currentSlider.value = 0;
            timing = 0;
        }
        /// <summary>
        /// 滚动条检查
        /// </summary>
        private void DetectSlider()
        {
            if (sliderIndex < sliders.Length - 1)
            {
                sliderIndex++;
                currentSlider = sliders[sliderIndex];
                sliderStatus = SliderStatus.Wait;
                timing = 0;
            }
            else
            {
                Debug.Log("unlocked");
            }
        }
        #endregion
    }
}

