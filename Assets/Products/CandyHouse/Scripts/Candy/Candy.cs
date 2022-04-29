//******************************************************
//FileName        :Candy.cs
//Description     :弹跳糖果
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;

public class Candy : MonoBehaviour
{
    /// <summary> 糖果类型 </summary>
    public CandyType candyType;
    /// <summary> 精灵 </summary>
    [HideInInspector] public SpriteRenderer spriteRenderer;
    /// <summary> 刚体 </summary>
    [HideInInspector] public Rigidbody2D rigidbody2d;
    /// <summary> 是否是选中大糖果 </summary>
    public bool isTitle = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        if (!isTitle)
        {
            Game.Instance.listCandy.Add(this); //加入弹跳糖果集合
        }
    }
}
