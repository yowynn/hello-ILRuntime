//******************************************************
//FileName        :Gun.cs
//Description     :大炮类
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;
using System.Collections.Generic;

public class Gun : UnitySingleton<Gun>
{
    /// <summary> 刚体 </summary>
    Rigidbody2D rigidbodyGun;
    /// <summary> 移动方向 </summary>
    [SerializeField] public Vector2 movement;
    /// <summary> 大炮状态 </summary>
    GunState state;
    /// <summary> 开火计时器 </summary>
    float fireTimer;
    /// <summary> 动画 </summary>
    [HideInInspector] public Animator animatorGun;
    /// <summary> 子弹池 </summary>
    [HideInInspector] public List<Bullet> listBullet = new List<Bullet>();

    void Start()
    {
        rigidbodyGun = GetComponent<Rigidbody2D>();
        animatorGun = GetComponent<Animator>();
        fireTimer = Random.Range(1f, 2f);
    }

    private void FixedUpdate()
    {
        if (!Player.Instance.IsState(PlayerState.SAD)) //判断主角状态
        {
            if (Game.Instance.IsState(GameState.READYGO) || Game.Instance.IsState(GameState.GAME)) //判断游戏状态
            {
                rigidbodyGun.MovePosition(rigidbodyGun.position + movement * Time.deltaTime); //刚体移动
            }
            if (Game.Instance.IsState(GameState.GAME))
            {
                switch (state)
                {
                    case GunState.IDLE: //待机
                        if (fireTimer > 0)
                        {
                            fireTimer -= Time.deltaTime; //开火计时
                            if (fireTimer <= 0) //开火
                            {
                                SetState(GunState.FIRE);
                            }
                        }
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 发射子弹
    /// </summary>
    /// <param name="type">预设子弹类型 (Type>0 只发射对应糖果 -1 随机)</param>
    public void Fire(int type = -1)
    {
        for (int i = 0; i < listBullet.Count; i++) //找到死亡的子弹
        {
            if (listBullet[i].isDead)
            {
                listBullet[i].Fire(transform.position, type);  //发射 (Type>0 只发射对应糖果 -1 随机)
                break;
            }
        }
        SetState(GunState.IDLE); //切换idle状态
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.stringGunCollision)) //碰到后转向
        {
            movement = new Vector2(-movement.x, movement.y);
        }
    }

    /// <summary>
    /// 设置大炮状态
    /// </summary>
    /// <param name="state">大炮状态</param>
    public void SetState(GunState state)
    {
        switch (state)
        {
            case GunState.IDLE: //待机
                animatorGun.SetBool(Constants.stringFire, false); //设置动画
                fireTimer = Random.Range(1f, 2f); //随机开火计时
                break;
            case GunState.FIRE: //开火
                animatorGun.SetBool(Constants.stringFire, true); //设置开火动画
                break;
        }
        this.state = state;
    }

    /// <summary> 判断大炮状态 </summary>
    public bool IsState(GunState state)
    {
        return this.state == state;
    }
}
