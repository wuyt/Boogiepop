using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Boogiepop
{
    /// <summary>
    /// 主场景逻辑
    /// </summary>
    public class MainManager : MonoBehaviour
    {
        /// <summary>
        /// 主场景状态
        /// </summary>
        public enum MainStatus
        {
            /// <summary>
            /// 提问阶段
            /// </summary>
            Question,
            /// <summary>
            /// 回答阶段
            /// </summary>
            Anwser
        }
        /// <summary>
        /// 主场景状态
        /// </summary>
        public MainStatus mainStatus;
        /// <summary>
        /// 系统变量
        /// </summary>
        private GameVariable gameVariable;

        #region Play Information
        /// <summary>
        /// 音源
        /// </summary>
        [Header("信息播放")]
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
        /// <summary>
        /// 当前滚动条
        /// </summary>
        [Header("解锁")]
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

        #region Question
        /// <summary>
        /// 识别对象数组，0-3：照片，4：锁，5-8：拼图
        /// </summary>
        [Header("初始提问")]
        public TrackableEventHandler[] targets;
        /// <summary>
        /// 解谜类型，0：拼图，1：解锁，2：找图片
        /// </summary>
        public int puzzleType;
        /// <summary>
        /// 拼图目标，5-8
        /// </summary>
        public int puzzleImage;
        /// <summary>
        /// 解锁密码，0-5
        /// </summary>
        public int[] LockKey;
        /// <summary>
        /// 寻找照片，0-3
        /// </summary>
        public int foundPic;
        /// <summary>
        /// 提问照片，0-3
        /// </summary>
        public int picQuestion;
        /// <summary>
        /// 提示照片，0-3
        /// </summary>
        public int picHelp;

        #endregion

        private void Awake()
        {
            PiStart();
        }

        private void Start()
        {
            MakeQuestion();
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
                        if (mainStatus == MainStatus.Question)
                        {
                            MakeAnswer();
                        }
                        else
                        {
                            BackMenu();
                        }
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
                BackMenu();
            }
        }
        #endregion

        #region Question
        /// <summary>
        /// 设置提问阶段信息
        /// </summary>
        private void MakeQuestion()
        {
            mainStatus = MainStatus.Question;
            RandomType();
            RandomPicQuestionAndHelp();
            for (int i = 0; i < targets.Length; i++)
            {
                if (i < 4)  //照片
                {
                    if (i == picQuestion)   //提问照片
                    {
                        switch (puzzleType)
                        {
                            case 0:     //拼图类型
                                QuestionPuzzle();
                                break;
                            case 1:     //解锁类型
                                QuestionUnlock();
                                break;
                            case 2:     //找照片类型
                                QuestionFindPic();
                                break;
                            default:
                                Debug.Log("puzzleType error");
                                break;
                        }
                    }
                    else  //其他照片
                    {
                        QuestionPicOther(i);
                    }
                }
                else if (i == 4)   //锁
                {
                    HiddenLock();
                }
                else if (i > 4) //  拼图图片
                {
                    QuestionPuzzleImage(i);
                }
            }
        }
        /// <summary>
        /// 提问阶段拼图类型
        /// </summary>
        private void QuestionPuzzle()
        {
            //随机生成目标
            puzzleImage = Random.Range(5, 9);
            //加载信息
            string[] paths = new string[3];
            paths[0] = targets[picQuestion].name.Replace("ImageTarget", "") + "/story";
            paths[1] = "puzzle/question";
            paths[2] = "puzzle/" + targets[puzzleImage].name.Replace("ImageTarget", "")
                + "/question";
            SetTargetInformation(picQuestion, paths, true);
        }
        /// <summary>
        /// 提问阶段解锁类型
        /// </summary>
        private void QuestionUnlock()
        {
            //生成随机密码
            LockKey = new int[3];

            for (int i = 0; i < LockKey.Length; i++)
            {
                LockKey[i] = Random.Range(0, 5);
                Color color = Color.cyan;
                switch (LockKey[i])
                {
                    case 0:
                        color = Color.blue;
                        break;
                    case 1:
                        color = Color.red;
                        break;
                    case 2:
                        color = Color.yellow;
                        break;
                    case 3:
                        color = Color.white;
                        break;
                    case 4:
                        color = Color.black;
                        break;
                    default:
                        Debug.Log("radom key error");
                        break;
                }
                sliders[i].GetComponent<SliderColor>().color = color;
            }
            //加载信息
            string[] paths = new string[2];
            paths[0] = targets[picQuestion].name.Replace("ImageTarget", "") + "/story";
            paths[1] = "lock/question";
            SetTargetInformation(picQuestion, paths, true);
        }
        /// <summary>
        /// 提问阶段找照片类型
        /// </summary>
        private void QuestionFindPic()
        {
            //生成随机目标
            do
            {
                foundPic = Random.Range(0, 4);
            } while (foundPic == picQuestion);

            //加载信息
            string[] paths = new string[2];
            paths[0] = targets[picQuestion].name.Replace("ImageTarget", "") + "/story";
            paths[1] = targets[foundPic].name.Replace("ImageTarget", "") + "/search";
            SetTargetInformation(picQuestion, paths, true);
        }
        /// <summary>
        /// 提问阶段其他照片设置
        /// </summary>
        /// <param name="index">序号</param>
        private void QuestionPicOther(int index)
        {
            string[] paths = new string[1];
            paths[0] = targets[index].name.Replace("ImageTarget", "")
                + "/introduction";

            SetTargetInformation(index, paths, false);
        }
        /// <summary>
        /// 生成提问照片和提示照片
        /// </summary>
        private void RandomPicQuestionAndHelp()
        {
            picQuestion = Random.Range(0, 4);
            do
            {
                picHelp = Random.Range(0, 4);
            } while (picQuestion == picHelp);
        }
        /// <summary>
        /// 隐藏解锁
        /// </summary>
        private void HiddenLock()
        {
            //注册Vuforia启动完成事件
            Vuforia.VuforiaARController.Instance
                .RegisterVuforiaStartedCallback(OnVuforiaStarted);
        }
        /// <summary>
        /// Vuforia启动完成事件
        /// </summary>
        private void OnVuforiaStarted()
        {
            targets[4].gameObject.SetActive(false);
        }
        /// <summary>
        /// 随机生成解谜类型
        /// </summary>
        private void RandomType()
        {
            do
            {
                puzzleType = Random.Range(0, 3);
            } while (FindObjectOfType<GameVariable>().CompleteTypes[puzzleType]);
        }
        /// <summary>
        /// 拼图初始设置
        /// </summary>
        /// <param name="index">序号</param>
        private void QuestionPuzzleImage(int index)
        {
            string[] paths = new string[1];
            paths[0] = "puzzle/help";

            SetTargetInformation(index, paths, false);
        }
        #endregion

        #region Answer
        /// <summary>
        /// 设置回答阶段信息
        /// </summary>
        private void MakeAnswer()
        {
            mainStatus = MainStatus.Anwser;
            switch (puzzleType)
            {
                case 0:     //拼图类型
                    AnwserPuzzle();
                    break;
                case 1:     //解锁类型
                    AnwserUnlock();
                    break;
                case 2:     //找照片类型
                    AnwserFoundPic();
                    break;
                default:
                    Debug.Log("puzzleType error");
                    break;
            }
        }
        /// <summary>
        /// 回答阶段找照片类型
        /// </summary>
        private void AnwserFoundPic()
        {
            string[] paths;

            for (int i = 0; i < 4; i++)
            {
                if (i == picQuestion)   //提问照片
                {
                    paths = new string[1];
                    paths[0] = targets[foundPic].name.Replace("ImageTarget", "")
                        + "/search";
                    SetTargetInformation(picQuestion, paths, false);
                }
                else if (i == foundPic)   //目标照片
                {
                    paths = new string[1];
                    paths[0] = targets[i].name.Replace("ImageTarget", "")
                        + "/introduction";
                    SetTargetInformation(i, paths, true);
                }
                else    //其他照片
                {
                    paths = new string[1];
                    paths[0] = targets[i].name.Replace("ImageTarget", "")
                        + "/say0" + Random.Range(1, 3).ToString();
                    SetTargetInformation(i, paths, false);
                }
            }
        }
        /// <summary>
        /// 回答阶段解锁类型
        /// </summary>
        private void AnwserUnlock()
        {
            string[] paths;

            for (int i = 0; i < 4; i++)
            {
                if (i == picQuestion)   //提问照片
                {
                    paths = new string[1];
                    paths[0] = "lock/question";
                    SetTargetInformation(picQuestion, paths, false);
                }
                else if (i == picHelp)   //提示照片
                {
                    paths = new string[3];
                    for (int j = 0; j < paths.Length; j++)
                    {
                        paths[j] = "lock/" + LockKey[j].ToString();
                    }
                    SetTargetInformation(i, paths, false);
                }
                else    //其他照片
                {
                    paths = new string[1];
                    paths[0] = targets[i].name.Replace("ImageTarget", "")
                        + "/say0" + Random.Range(1, 3).ToString();
                    SetTargetInformation(i, paths, false);
                }
            }

            targets[4].gameObject.SetActive(true);
        }
        /// <summary>
        /// 回答阶段拼图设置
        /// </summary>
        private void AnwserPuzzle()
        {
            string[] paths;

            for (int i = 0; i < 4; i++)
            {
                if (i == picQuestion)   //提问照片
                {
                    paths = new string[2];
                    paths[0] = "puzzle/question";
                    paths[1] = "puzzle/"
                        + targets[puzzleImage].name.Replace("ImageTarget", "")
                        + "/question";
                    SetTargetInformation(picQuestion, paths, true);
                }
                else if (i == picHelp)   //提示照片
                {
                    paths = new string[1];
                    paths[0] = "puzzle/"
                        + targets[puzzleImage].name.Replace("ImageTarget", "")
                        + "/help";
                    SetTargetInformation(i, paths, false);
                }
                else    //其他照片
                {
                    paths = new string[1];
                    paths[0] = targets[i].name.Replace("ImageTarget", "")
                        + "/say0" + Random.Range(1, 3).ToString();
                    SetTargetInformation(i, paths, false);
                }
            }

            //成功拼图
            paths = new string[1];
            paths[0] = "puzzle/"
                        + targets[puzzleImage].name.Replace("ImageTarget", "")
                        + "/help";
            SetTargetInformation(puzzleImage, paths, true);
        }

        #endregion

        #region commond
        /// <summary>
        /// 为识别目标添加信息
        /// </summary>
        /// <param name="index">序号</param>
        /// <param name="paths">路径数组</param>
        /// <param name="status">状态</param>
        private void SetTargetInformation(int index, string[] paths, bool status)
        {
            targets[index].information.audioClips
                = new AudioClip[paths.Length];
            targets[index].information.texts
                = new string[paths.Length];

            for (int i = 0; i < paths.Length; i++)
            {
                targets[index].information.audioClips[i]
                    = Resources.Load<AudioClip>(paths[i]);
                targets[index].information.texts[i]
                    = Resources.Load<TextAsset>(paths[i]).text;
            }

            targets[index].information.changeStatus = status;
        }
        /// <summary>
        /// 返回菜单场景
        /// </summary>
        private void BackMenu()
        {
            string scene = "Menu";

            //获取系统变量
            GameVariable gameVariable = FindObjectOfType<GameVariable>();
            gameVariable.IsCompleteGame = true;
            gameVariable.CompleteTypes[puzzleType] = true;

            //如果都完成了，重置完成类型
            if (gameVariable.CompleteTypes[0]
                && gameVariable.CompleteTypes[1]
                && gameVariable.CompleteTypes[2])
            {
                for (int i = 0; i < gameVariable.CompleteTypes.Length; i++)
                {
                    gameVariable.CompleteTypes[i] = false;
                }

                scene = "egg";
            }

            SceneManager.LoadScene(scene);
        }
        #endregion
    }
}

