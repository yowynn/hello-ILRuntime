//******************************************************
//FileName        :Game.cs
//Description     :游戏主场景
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class Game : UnitySingleton<Game>
{
    /// <summary> 游戏状态 </summary>
    public GameState state = GameState.LOGO;

    /// <summary> LOGO界面 </summary>
    public GameObject gameObjectGameLogo;

    /// <summary> 相机 </summary>
    public Camera cameraMain;

    /// <summary> 主游戏场景 </summary>
    public Transform transformGameScene;

    /// <summary> 弹跳糖果集合 </summary>
    [HideInInspector] public List<Candy> listCandy;

    /// <summary> 选择的糖果类型集合 </summary>
    [HideInInspector] public List<CandyType> listChooseCandy = new List<CandyType>();

    /// <summary> 选择的糖果按钮集合 </summary>
    [HideInInspector] public List<Button> listCandyButton = new List<Button>();

    /// <summary> 跑马灯特效 </summary>
    public GameObject gameObjectChooseLight;

    /// <summary> 彩带特效 </summary>
    public Animator animatorCelebrate;

    /// <summary> 选中糖果光效 </summary>
    public List<GameObject> listGameObjectLight;

    /// <summary> 选中糖果 </summary>
    public List<Transform> listTransformTitleCandy;

    /// <summary> ReadyGo界面 </summary>
    public Animator animatorReadyGo;

    /// <summary> GameEnd界面 </summary>
    public GameObject gameObjectGameEnd;

    /// <summary> GameEnd光效 </summary>
    public GameObject gameObjectGameEndLight;

    /// <summary> 重玩按钮 </summary>
    public GameObject gameObjectGameEndButton;

    /// <summary> 计数器 </summary>
    public Counter counter;
    /// <summary> 计数板特效 </summary>
    public Animator animatorCounter;
    /// <summary> 计数器数字特效 </summary>
    public Animator animatorCounterNum;

    /// <summary> 计数器数字 </summary>
    [HideInInspector] public int counterNum = 0;

    /// <summary> 最大值 </summary>
    public int counterMax = 20;

    /// <summary> 背景音乐 </summary>
    public AudioSource audioSourceMusic;
    /// <summary> 特效1 </summary>
    public AudioSource audioSourceEffect;
    /// <summary> 特效2 </summary>
    public AudioSource audioSourceEffect2;
    /// <summary> 旁白 </summary>
    public AudioSource audioSourceVoice;

    /// <summary> 引导小手计时数 </summary>
    [HideInInspector] public float timer;
    /// <summary> 引导小手 </summary>
    public GameObject gameObjectGuide;

    //游戏初始化
    public void GameInit()
    {
        StopMusic();
        gameObjectGameEnd.SetActive(false);//关闭GameEnd界面
        listChooseCandy.Clear();//清除选中糖果
        cameraMain.transform.position = new Vector3(0, 0, -10); //重置主相机
        Player.Instance.transform.position = new Vector3(4, -13, 0); //重置主角位置
        Player.Instance.transform.localScale = Vector3.one; //重置主角朝向
        Player.Instance.SetState(PlayerState.IDLE); //重置主角状态
        Gun.Instance.transform.position = new Vector3(0, -6.35f, 0);// 重置炮位置
        Gun.Instance.SetState(GunState.IDLE); //重置炮状态
        counterNum = 0; //重置计数器
        counter.SetCounterNum(0); //重置计数器
        Player.Instance.skeletonMecanimPlayer.Skeleton.SetSkin(Constants.stringTang0); //重置主角皮肤
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.CHOOSECANDY:
                //5S未操作，播放【引导小手引导点击选择按钮】，播放语音：
                timer += Time.deltaTime;
                if (timer > 5 && !gameObjectGuide.activeSelf) //显示引导小手
                {
                    //查看未选择的糖果
                    var choose = false;
                    for (int i = 0; i < listCandyButton.Count; i++)
                    {
                        for (int j = 0; j < listChooseCandy.Count; j++)
                        {
                            if (listChooseCandy[j] == listCandyButton[i].candyType)
                            {
                                choose = true;
                                break;
                            }
                        }
                        if (!choose)
                        {
                            ShowGuide(listCandyButton[i].transform.position); //显示引导小手
                            break;
                        }
                    }
                    var VoiceName = Constants.voiceChooseCandy0; //“点击按钮，选择你喜欢的三种糖果吧~“（对白文件3）
                    if (listChooseCandy.Count == 1)
                    {
                        VoiceName = Constants.voiceChooseCandy1; //再选两种喜欢的糖果吧~“（对白文件4）
                    }
                    else if (listChooseCandy.Count == 2)
                    {
                        VoiceName = Constants.voiceChooseCandy2; //还要选一种糖果呦~“（对白文件5）
                    }
                    PlayVoice(VoiceName, () => //播放对白
                    {
                        timer = 0;
                        gameObjectGuide.SetActive(false);
                    });
                }
                break;
        }
    }

    /// <summary>
    /// 显示帮助小手
    /// </summary>
    /// <param name="pos">小手坐标</param>
    public void ShowGuide(Vector3 pos)
    {
        //显示
        gameObjectGuide.SetActive(true);
        //定位
        gameObjectGuide.transform.position = pos;
    }


    /// <summary>
    /// 选择糖果
    /// </summary>
    /// <param name="candyType">糖果类型</param>
    public void TouchCandyButton(CandyType candyType)
    {
        if (IsState(GameState.CHOOSECANDY)) //选择糖果环节
        {
            timer = 0; //引导计时清零
            listChooseCandy.Add(candyType); //标记选中糖果
            listGameObjectLight[(int)candyType].SetActive(true); //播放选中光效

            //克隆选中糖果
            GameObject candy_prefab = (GameObject)Resources.Load("Prefabs/candy/" + candyType.ToString());
            candy_prefab.GetComponent<Candy>().isTitle = true;
            GameObject newCandy = GameObject.Instantiate(candy_prefab);
            newCandy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //设置选中糖果刚体状态
            newCandy.transform.localScale = Vector3.one;
            newCandy.transform.SetParent(listTransformTitleCandy[listChooseCandy.Count - 1], false); //设置节点
            listTransformTitleCandy[listChooseCandy.Count - 1].Find("Star").gameObject.SetActive(true); //播放选中特效

            //选中类型的糖果弹跳一次
            for (int i = 0; i < listCandy.Count; i++)
            {
                if (listCandy[i].candyType == candyType)
                {
                    listCandy[i].rigidbody2d.velocity = new Vector2(0, 3f);
                }
            }

            //计算选中多少糖果类型
            if (listChooseCandy.Count >= 3)
            {
                SetButtonScale(newCandy.transform, () => //选完按钮特效
                {
                    StopVoice(); //停止旁白
                    gameObjectChooseLight.SetActive(true); //播放特效
                    for (int i = 0; i < listTransformTitleCandy.Count; i++)
                    {
                        if (i == listTransformTitleCandy.Count - 1) //最后一个特效后转场
                        {
                            SetButtonScale(listTransformTitleCandy[i].transform, () => //选中糖果特效
                            {
                                //播放庆祝特效
                                animatorCelebrate.gameObject.SetActive(true);
                                animatorCelebrate.Play("caidai", 0, 0f);
                            });
                        }
                        else
                        {
                            SetButtonScale(listTransformTitleCandy[i].transform); //选中糖果特效
                        }
                    }
                });
            }
            else
            {
                SetButtonScale(newCandy.transform); //播放特效
            }
        }
    }

    /// <summary>
    /// 再次选中糖果
    /// </summary>
    /// <param name="candyType">糖果类型</param>
    public void ChooseCandyAgain(CandyType candyType)
    {
        if (IsState(GameState.CHOOSECANDY)) //确认状态
        {
            timer = 0; //引导计时清零
            //按钮震动
            for (int i = 0; i < listTransformTitleCandy.Count; i++)
            {
                if (listTransformTitleCandy[i].GetComponentInChildren<Candy>()?.candyType == candyType)
                {
                    SetButtonShake(listTransformTitleCandy[i].transform);
                    break;
                }
            }
            //糖果弹跳
            for (int i = 0; i < listCandy.Count; i++)
            {
                if (listCandy[i].candyType == candyType && listCandy[i].rigidbody2d.velocity == Vector2.zero)
                {
                    listCandy[i].rigidbody2d.velocity = new Vector2(0, 3f);
                }
            }
        }
    }

    /// <summary> 计数器增加计数 </summary>
    public void AddCounter()
    {
        counter.SetCounterNum(++counterNum); //计数增加
        if (counterNum == 10) //播放10特效
        {
            counter.HideNum();
            animatorCounterNum.gameObject.SetActive(true);
            animatorCounterNum.Play(Constants.stringIdle, 0, 0f);
        }
        else if (counterNum == 20) //播放20特效
        {
            animatorCounter.SetBool("Run", true);
        }
        if (counterNum >= counterMax) //计数满 游戏结束 切换状态
        {
            Player.Instance.SetState(PlayerState.HAPPYEND);
            SetState(GameState.NARRATOR2);
        }
    }

    /// <summary>
    /// 设置游戏状态
    /// </summary>
    /// <param name="state">状态类型</param>
    public void SetState(GameState state)
    {
        StopVoice(); //停止旁白
        switch (state)
        {
            case GameState.NARRATOR: //旁白
                PlayMusic(Constants.musicBg1);
                gameObjectGameLogo.SetActive(false);
                PlayVoice(Constants.voiceNarrator, () => //播放旁白2
                {
                    for (int i = 0; i < listCandyButton.Count; i++)
                    {
                        if (i == listCandyButton.Count - 1)
                            SetButtonScale(listCandyButton[i].transform, () =>  //播放特效
                            {
                                SetState(GameState.CHOOSECANDY);
                            });
                        else
                            SetButtonScale(listCandyButton[i].transform); //播放特效
                    }
                });
                break;
            case GameState.TRANSITION: //转场
                animatorCelebrate.gameObject.SetActive(false);
                for (int i = 0; i < listCandy.Count; i++) //设置所有糖果刚体
                {
                    listCandy[i].rigidbody2d.bodyType = RigidbodyType2D.Static;
                }
                //转场
                DOTween.To(() => cameraMain.transform.position, x => cameraMain.transform.position = x, new Vector3(0, -10, -10), 3).SetEase(Ease.Linear).OnComplete(() =>
                {
                    SetState(GameState.EXPLAIN1);
                });
                break;
            case GameState.EXPLAIN1: //游戏说明1
                StopMusic();
                gameObjectChooseLight.SetActive(false);
                for (int i = 0; i < listTransformTitleCandy.Count; i++) //清除选中糖果
                {
                    Destroy(listTransformTitleCandy[i].GetChild(1).gameObject);
                }
                for (int i = 0; i < listGameObjectLight.Count; i++) //关闭选中光效
                {
                    listGameObjectLight[i].SetActive(false);
                }
                for (int i = 0; i < listCandy.Count; i++) //糖果刚体状态重置
                {
                    listCandy[i].rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
                }
                for (int i = 0; i < listCandyButton.Count; i++) //糖果按钮状态重置
                {
                    listCandyButton[i].ButtonInit();
                }
                Gun.Instance.animatorGun.SetBool(Constants.stringFire, true); //炮发射子弹
                break;
            case GameState.EXPLAIN2: //游戏说明2
                PlayVoice(Constants.voiceExplain2); //旁白7
                break;
            case GameState.READYGO: //开始
                PlayMusic(Constants.musicBg2);
                animatorReadyGo.gameObject.SetActive(true); //准备界面
                break;
            case GameState.NARRATOR2: //旁白2
                PlayEffect(Constants.stringAmazing); //播放音效
                //“哇哦，太厉害了，我们收集了好多的糖果~“（对白文件11）
                PlayVoice(Constants.voiceNarrator2, () =>
                {
                    SetState(GameState.END);
                });
                break;
            case GameState.END: //结束
                PlayEffect(Constants.stringGameOver);
                gameObjectGameEnd.SetActive(true); //开启结束界面
                gameObjectGameEndLight.transform.DOLocalRotate(new Vector3(0, 0, 360), 10, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1); //光效旋转
                break;
        }
        this.state = state;
    }

    /// <summary>
    /// 呼吸特效
    /// </summary>
    /// <param name="transform">播放对象</param>
    /// <param name="callBack">完成回调</param>
    public void SetButtonScale(Transform transform, Action callBack = null)
    {
        transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                if (callBack != null) callBack();
            });
        });
    }

    /// <summary>
    /// 震动
    /// </summary>
    /// <param name="transform">震动对象</param>
    public void SetButtonShake(Transform transform)
    {
        transform.DOShakePosition(0.15f, 0.1f);
    }

    /// <summary>
    /// 判断游戏状态
    /// </summary>
    /// <param name="state">状态</param>
    public bool IsState(GameState state)
    {
        return this.state == state;
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="MusicName">音乐名称</param>
    /// <param name="loop">是否循环</param>
    public void PlayMusic(string MusicName, bool loop = true)
    {
        //加载音频资源
        AudioClip clip = Resources.Load(Constants.stringMusicAddress + MusicName) as AudioClip;
        //加载音频资源
        audioSourceMusic.clip = clip;
        //音频是否重复
        audioSourceMusic.loop = loop;
        //音频播放
        audioSourceMusic.Play();
    }

    /// <summary> 继续播放音乐 </summary>
    public void ReplayMusic()
    {
        if (!audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Play();
        }
    }

    /// <summary> 暂停播放音乐 </summary>
    public void StopMusic()
    {
        if (audioSourceMusic.isPlaying)
        {
            audioSourceMusic.Stop();
        }
    }

    /// <summary>
    /// 播放音乐特效
    /// </summary>
    /// <param name="EffectName">音乐名称</param>
    /// <param name="loop">是否循环</param>
    public void PlayEffect(string EffectName, bool loop = false)
    {
        //加载音频资源
        AudioClip clip = Resources.Load(Constants.stringEffectAddress + EffectName) as AudioClip;
        if (audioSourceEffect.isPlaying)
        {
            //加载音频资源
            audioSourceEffect2.clip = clip;
            //音频是否重复
            audioSourceEffect2.loop = loop;
            //音频播放
            audioSourceEffect2.Play();
        }
        else
        {
            //加载音频资源
            audioSourceEffect.clip = clip;
            //音频是否重复
            audioSourceEffect.loop = loop;
            //音频播放
            audioSourceEffect.Play();
        }
    }

    /// <summary>
    /// 播放旁白
    /// </summary>
    /// <param name="EffectName">音乐名称</param>
    /// <param name="callback">完成回调</param>
    public void PlayVoice(string EffectName, Action callback = null)
    {
        //加载音频资源
        AudioClip clip = Resources.Load(Constants.stringVoiceAddress + EffectName) as AudioClip;
        //加载音频资源
        audioSourceVoice.clip = clip;
        //音频是否重复
        audioSourceVoice.loop = false;
        //音频播放
        audioSourceVoice.Play();
        //音频回调
        if (callback != null) StartCoroutine(AudioPlayFinished(clip.length + 0.5f, callback));
    }

    /// <summary> 暂停旁白 </summary>
    public void StopVoice()
    {
        if (audioSourceVoice.isPlaying)
        {
            audioSourceVoice.Stop();
        }
    }

    /// <summary>
    /// 协成回调
    /// </summary>
    /// <param name="time">时间</param>
    /// <param name="callback">完成回调</param>
    private IEnumerator AudioPlayFinished(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        if (callback != null) callback();
    }
}