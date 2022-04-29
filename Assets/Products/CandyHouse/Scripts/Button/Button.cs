//******************************************************
//FileName        :Button.cs
//Description     :spine动画按钮
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;
using Spine.Unity;

public class Button : MonoBehaviour
{
    /// <summary> 按钮代表的糖果类型 </summary>
    public CandyType candyType;
    /// <summary> 按钮动画 </summary>
    private SkeletonAnimation skeletonAnimation;

    void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        Game.Instance.listCandyButton.Add(this); //加入糖果按钮集合
    }

    /// <summary> 初始化 </summary>
    public void ButtonInit()
    {
        skeletonAnimation.AnimationName = Constants.stringImg;
    }

    /// <summary> 鼠标按下 </summary>
    private void OnMouseDown()
    {
        //确定状态和类型
        if (Game.Instance.IsState(GameState.CHOOSECANDY) && skeletonAnimation.AnimationName == Constants.stringImg && Game.Instance.listChooseCandy.Count < 3)
        {
            skeletonAnimation.AnimationName = Constants.stringAnXia; //按下 切换皮肤
        }
    }

    /// <summary> 鼠标抬起 </summary>
    private void OnMouseUp()
    {
        //确定状态和类型
        if (Game.Instance.IsState(GameState.CHOOSECANDY) && Game.Instance.listChooseCandy.Count < 3)
        {
            if (skeletonAnimation.AnimationName == Constants.stringAnXia) //按下
            {
                Game.Instance.StopVoice();
                Game.Instance.TouchCandyButton(candyType); //选中该糖果
                skeletonAnimation.AnimationName = Constants.stringShine; //切换选中皮肤
                if (Game.Instance.listChooseCandy.Count == 3) //选中3个糖果
                {
                    Game.Instance.PlayEffect(Constants.stringAmazing);
                    Game.Instance.PlayEffect(Constants.stringCakeGood); //正确音效
                }
                else //选中不够3个糖果
                {
                    Game.Instance.PlayEffect(Constants.stringChooseCandy); //正确音效
                    Game.Instance.PlayEffect(Constants.stringGood);
                }
            }
            else if (skeletonAnimation.AnimationName == Constants.stringShine) //已经按过
            {
                Game.Instance.PlayEffect(Constants.stringError); //错误音效
                Game.Instance.ChooseCandyAgain(candyType); //再次选中糖果
            }
        }
    }

    /// <summary> 鼠标移出范围 </summary>
    private void OnMouseExit()
    {
        //确定状态和类型
        if (Game.Instance.IsState(GameState.CHOOSECANDY) && skeletonAnimation.AnimationName == Constants.stringAnXia && Game.Instance.listChooseCandy.Count < 3)
        {
            skeletonAnimation.AnimationName = Constants.stringImg; //重新设置皮肤
        }
    }
}
