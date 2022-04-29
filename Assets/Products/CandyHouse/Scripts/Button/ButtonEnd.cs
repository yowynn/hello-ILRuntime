//******************************************************
//FileName        :ButtonEnd.cs
//Description     :游戏结束按钮
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;

public class ButtonEnd : MonoBehaviour
{
    //绘制器
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary> 按下 </summary>
    private void OnMouseDown()
    {
        if (Game.Instance.IsState(GameState.END)) //判断游戏状态
        {
            spriteRenderer.color = Color.grey; //按下效果
            Invoke(Constants.stringQuit, 0.2f); //游戏重启
        }
    }

    /// <summary> 游戏重启 </summary>
    private void Quit()
    {
        Game.Instance.GameInit(); //游戏重置
        Game.Instance.SetState(GameState.NARRATOR); //设置游戏状态
        spriteRenderer.color = Color.white; //按钮还原
    }
}
