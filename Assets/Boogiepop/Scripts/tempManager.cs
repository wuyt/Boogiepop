using UnityEngine;
using UnityEngine.UI;

namespace Boogiepop
{
    public class tempManager : MonoBehaviour
    {
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
        private void Start()
        {
            sliderStatus = SliderStatus.Wait;
            timing = 0;
            sliderIndex = 0;
            currentSlider = sliders[sliderIndex];
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

        private void Update()
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
    }
}

