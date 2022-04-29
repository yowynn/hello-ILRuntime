//******************************************************
//FileName        :Player.cs
//Description     :游戏主角
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class Player : UnitySingleton<Player>
{
    /// <summary> 主角状态 </summary>
    PlayerState state;
    /// <summary> 主角动画 </summary>
    [SerializeField] public Animator animatorPlayer;
    /// <summary> 鼠标位置 </summary>
    Vector3 mouseWorldPos;
    /// <summary> dotween动作 </summary>
    Tween tween;
    /// <summary> 爱心动画 </summary>
    public SkeletonAnimation skeletonAnimationHeart;
    /// <summary> 主角spine </summary>
    [HideInInspector] public SkeletonMecanim skeletonMecanimPlayer;
    public float walkSpeed = 5;//走路速度
    /// <summary> 音效 </summary>
    private AudioSource audioSourceEffect;

    void Start()
    {
        animatorPlayer = GetComponent<Animator>();
        skeletonMecanimPlayer = GetComponent<SkeletonMecanim>();
        audioSourceEffect = GetComponent<AudioSource>();
        SetState(PlayerState.IDLE);
    }

    void Update()
    {
        if (Game.Instance.IsState(GameState.GAME)) //游戏中
        {
            switch (state)
            {
                case PlayerState.IDLE:
                case PlayerState.WALK:
                case PlayerState.HAPPY:
                    if (Input.GetKeyDown(KeyCode.Mouse0)) //鼠标按下
                    {
                        SetState(PlayerState.WALK); //切换走路状态
                        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //鼠标坐标转化
                        if (mouseWorldPos.x > transform.position.x)
                        {
                            transform.localScale = Vector3.left + Vector3.up; //翻转
                        }
                        else
                        {
                            transform.localScale = Vector3.one; //切换正常
                        }
                        var pos = new Vector3(mouseWorldPos.x, transform.position.y, 0); //找到要移动的坐标
                        if (tween != null) //删除上一个移动逻辑
                        {
                            tween.Kill();
                            tween = null;
                        }
                        //移动到坐标点
                        tween = DOTween.To(() => transform.position, x => transform.position = x, pos, GetTime(pos)).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            SetState(PlayerState.IDLE); //切换idle状态
                        });
                    }
                    break;
            }
        }
        else if (Game.Instance.IsState(GameState.EXPLAIN2)) //旁边2
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) //按下
            {
                SetState(PlayerState.WALK); //切换状态
                var pos = new Vector3(0, transform.position.y, 0); //寻找坐标
                //移动
                DOTween.To(() => transform.position, x => transform.position = x, pos, GetTime(pos)).SetEase(Ease.Linear).OnComplete(() =>
                {
                    SetState(PlayerState.IDLE);
                });
            }
        }
    }

    /// <summary>
    /// 计算移动时间
    /// </summary>
    /// <param name="pos">坐标点</param>
    public float GetTime(Vector3 pos)
    {
        //确认当前移动速度
        if (!animatorPlayer.GetBool(Constants.stringSadWalk))
        {
            if (Game.Instance.counterNum > 9)
            {
                if (animatorPlayer.GetFloat(Constants.stringCounter) != 10)
                {
                    animatorPlayer.SetFloat(Constants.stringCounter, 10);
                }
                walkSpeed = 6f;
            }
            else
            {
                if (animatorPlayer.GetFloat(Constants.stringCounter) == 10)
                {
                    animatorPlayer.SetFloat(Constants.stringCounter, 0);
                }
                walkSpeed = 5f;
            }
        }
        //时间 = 距离/速度
        return (pos - transform.position).magnitude / walkSpeed;
    }

    /// <summary>
    /// 设置主角状态
    /// </summary>
    /// <param name="state">主角状态</param>
    public void SetState(PlayerState state)
    {
        this.state = state;
        switch (state)
        {
            case PlayerState.IDLE: //待机
                animatorPlayer.SetBool(Constants.stringEnd, false);
                animatorPlayer.SetBool(Constants.stringWalk, false);
                break;
            case PlayerState.WALK: //移动
                animatorPlayer.SetBool(Constants.stringWalk, true);
                break;
            case PlayerState.HAPPY: //开心
                animatorPlayer.SetBool(Constants.stringSadWalk, false);
                if (Game.Instance.IsState(GameState.GAME))
                {
                    skeletonMecanimPlayer.Skeleton.SetSkin(Constants.stringTang + (1 + (Game.Instance.counterNum + 1) / 5)); //根据数量切换皮肤
                }
                skeletonAnimationHeart.state.SetAnimation(0, Constants.stringIdle, false); //播放小心心
                animatorPlayer.SetBool(Constants.stringHappy, true); //播放开心动画
                if (animatorPlayer.GetFloat(Constants.stringCounter) > 10) //根据数量切换速度
                {
                    walkSpeed = 6f;
                }
                else
                {
                    walkSpeed = 5f;
                }
                CancelInvoke(Constants.stringHappyEnd); //清除Invoke
                Invoke(Constants.stringHappyEnd, 1f); //延迟1秒切换
                break;
            case PlayerState.HAPPYEND:
                Stop();  //清除主角状态
                animatorPlayer.SetBool(Constants.stringEnd, true); //播放动画
                break;
            case PlayerState.SAD:
                Stop(); //清除主角状态
                animatorPlayer.SetBool(Constants.stringSad, true); //播放动画
                break;
        }
    }

    /// <summary> 伤心状态结束 </summary>
    public void SadEnd()
    {
        animatorPlayer.SetBool(Constants.stringSad, false);
        animatorPlayer.SetBool(Constants.stringSadWalk, true);
        walkSpeed = 4f;
    }

    /// <summary> 开心状态结束 </summary>
    public void HappyEnd()
    {
        animatorPlayer.SetBool(Constants.stringHappy, false);
    }

    /// <summary> 清除主角状态 </summary>
    private void Stop()
    {
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
        animatorPlayer.SetBool(Constants.stringWalk, false);
    }

    /// <summary> 判断主角状态 </summary>
    public bool IsState(PlayerState state)
    {
        return this.state == state;
    }

    /// <summary>
    /// 播放角色音效
    /// </summary>
    /// <param name="EffectName">音乐名称</param>
    /// <param name="loop">是否循环</param>
    public void PlayEffect(string EffectName, bool loop = false)
    {
        //加载音频资源
        AudioClip clip = Resources.Load(Constants.stringEffectAddress + EffectName) as AudioClip;
        //加载音频资源
        audioSourceEffect.clip = clip;
        //音频是否重复
        audioSourceEffect.loop = loop;
        //音频播放
        audioSourceEffect.Play();
    }
}
