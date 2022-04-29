//******************************************************
//FileName        :Bullet.cs
//Description     :子弹类
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    /// <summary> 子弹类型 </summary>
    public CandyType candyType;
    /// <summary> 子弹图片 </summary>
    public List<Sprite> listCandySprite;
    /// <summary> 是否死亡 </summary>
    [HideInInspector] public bool isDead = true;
    /// <summary> 是否是炸弹 </summary>
    [HideInInspector] public bool isBomb = false;
    /// <summary> 绘制器 </summary>
    SpriteRenderer spriteRenderer;
    /// <summary> 刚体 </summary>
    Rigidbody2D rigidbodyBullet;
    /// <summary> 炸弹动画 </summary>
    Animator animatorBomb;

    void Start()
    {
        rigidbodyBullet = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animatorBomb = GetComponentInChildren<Animator>(true);
        Dead(); //初始化 设置死亡状态
        Gun.Instance.listBullet.Add(this); //加入子弹池
    }

    /// <summary>
    /// 发射
    /// </summary>
    /// <param name="pos">坐标</param>
    /// <param name="Type">预设子弹类型 (Type>0 只发射对应糖果 -1 随机)</param>
    public void Fire(Vector3 pos, int Type)
    {
        if (Type > -1) //游戏说明1状态 只发射糖果
        {
            SetCandyType(Game.Instance.listChooseCandy[Type]); //设置发射的类型
        }
        else //游戏中
        {
            var num = Random.Range(0, Game.Instance.listChooseCandy.Count + 1); //随机发射类型
            if (num < Game.Instance.listChooseCandy.Count) //糖果
            {
                SetCandyType(Game.Instance.listChooseCandy[num]); //设置发射的类型
            }
            else //bomb
            {
                isBomb = true; //炸弹状态
                animatorBomb.gameObject.SetActive(true); //显示炸弹
                spriteRenderer.color = Color.clear; //绘制器关闭
            }
        }
        transform.position = pos; //设置子弹坐标
        isDead = false; //设置子弹状态
        rigidbodyBullet.velocity = new Vector3(0, -6f, 0); //设置子弹速度
    }

    private void Update()
    {
        if (Game.Instance.IsState(GameState.EXPLAIN1)) //游戏说明1状态
        {
            if (transform.position.y < -11 && rigidbodyBullet.velocity != Vector2.zero) //糖果停止下落
            {
                rigidbodyBullet.gravityScale = 0;
                rigidbodyBullet.velocity = Vector2.zero;
                Game.Instance.ShowGuide(transform.position);
                Game.Instance.SetState(GameState.EXPLAIN2);
            }
        }
        else if (Game.Instance.IsState(GameState.GAME)) //正常游戏
        {
            if (!isDead && transform.position.y < -20) //子弹坠落死亡
            {
                Dead();
            }
        }
    }

    /// <summary> 子弹死亡 状态清除 回归子弹池 </summary>
    public void Dead()
    {
        rigidbodyBullet.gravityScale = 0;
        rigidbodyBullet.velocity = Vector3.zero;
        transform.position = new Vector3(-10, 0, 0);
        isDead = true;
        isBomb = false;
        animatorBomb.gameObject.SetActive(false);
        spriteRenderer.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.stringPlayer) && !Player.Instance.IsState(PlayerState.SAD)) //碰到主角
        {
            if (isBomb) //炸弹
            {
                Game.Instance.StopMusic(); //停止音乐
                rigidbodyBullet.gravityScale = 0; //重力清零
                rigidbodyBullet.velocity = Vector3.zero; //速度清零
                animatorBomb.SetBool(Constants.stringBomb, true); //爆炸
                Game.Instance.PlayEffect(Constants.stringBomb); //播放音效
                Game.Instance.PlayEffect(Constants.stringCry);
                Player.Instance.SetState(PlayerState.SAD); //主角切换状态
                //“唔~我不要这个~“（对白文件9）
                Game.Instance.PlayVoice(Constants.voiceBomb, () =>
                {
                    Game.Instance.PlayVoice(Constants.voiceBombEnd); //播放对白
                });
            }
            else
            {
                Game.Instance.PlayEffect(Constants.stringGetCandy); //播放音效
                Player.Instance.SetState(PlayerState.HAPPY); //主角切换状态
                if (Game.Instance.IsState(GameState.EXPLAIN2)) //游戏说明2状态
                {
                    Game.Instance.gameObjectGuide.SetActive(false);
                    //小朋友，请你帮助角色收集糖果吧~要小心不要接到炸弹呦~“（对白文件8）
                    Game.Instance.PlayVoice(Constants.voiceReadyGo, () =>
                    {
                        Game.Instance.SetState(GameState.READYGO); //切换游戏状态
                    });
                }
                else
                {
                    Game.Instance.AddCounter(); //计数器++
                }
                Dead(); //碰撞死亡
            }
        }
    }

    /// <summary> 绘制当前糖果 </summary>
    public void SetCandyType(CandyType candyType)
    {
        spriteRenderer.sprite = listCandySprite[(int)candyType];
        this.candyType = candyType;
    }
}
