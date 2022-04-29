//******************************************************
//FileName        :Counter.cs
//Description     :计数器类
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;
using System.Collections.Generic;

public class Counter : MonoBehaviour
{
    //保存计数
    public int number;
    //十位
    public SpriteRenderer spriteRendererTen;
    //个位
    public SpriteRenderer spriteRendererDigits;
    //0-9
    public List<Sprite> listSprite;

    /// <summary> 设置计数器数值 </summary>
    public void SetCounterNum(int num)
    {
        number = num;
        if (number >= 10) //大于10
        {
            spriteRendererTen.gameObject.SetActive(true);
            spriteRendererTen.sprite = listSprite[number / 10];
            spriteRendererDigits.sprite = listSprite[number % 10];
        }
        else //个位
        {
            spriteRendererTen.gameObject.SetActive(false);
            spriteRendererDigits.sprite = listSprite[number];
        }
    }

    public void HideNum()
    {
        spriteRendererDigits.gameObject.SetActive(false);
        spriteRendererTen.gameObject.SetActive(false);
    }

    public void ShowNum()
    {
        spriteRendererDigits.gameObject.SetActive(true);
        spriteRendererTen.gameObject.SetActive(true);
    }
}
