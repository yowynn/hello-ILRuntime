//******************************************************
//FileName        :AnimEvent.cs
//Description     :动画事件回调类 处理动画事件
//Author          :zbl
//Date	          :2022/03/21
//RevisionHistory :
//******************************************************
using UnityEngine;
using DG.Tweening;

public class AnimEvent : MonoBehaviour
{
    /// <summary>
    /// 动画事件对应处理
    /// </summary>
    /// <param name="EventName">事件字符串</param>
    public void AnimatorEvent(string EventName)
    {
        switch (EventName)
        {
            case Constants.stringReadyGoEnd: //readyGo
                transform.gameObject.SetActive(false);
                Game.Instance.SetState(GameState.GAME);
                // DOTween.To(() => transform.localScale, x => transform.localScale = x, new Vector3(0, 0, 0), 1).SetEase(Ease.Linear).OnComplete(() =>
                // {
                //     transform.gameObject.SetActive(false);
                //     Game.Instance.SetState(GameState.Game);
                // });
                break;
            case Constants.stringFireEnd: //大炮开火
                if (Game.Instance.IsState(GameState.EXPLAIN1))
                {
                    Gun.Instance.Fire(0);
                }
                else
                {
                    Gun.Instance.Fire();
                }
                break;
            case Constants.stringSadEnd: //主角被炸
                Player.Instance.SadEnd();
                Player.Instance.SetState(PlayerState.IDLE);
                Game.Instance.ReplayMusic();
                break;
            case Constants.stringBombEnd: //炸弹爆炸
                transform.parent.GetComponent<Bullet>().Dead();
                break;
            case Constants.stringCelebrateEnd: //庆祝彩带
                Game.Instance.SetState(GameState.TRANSITION);
                break;
            case Constants.stringGameEnd: //游戏结束
                Invoke(Constants.stringGameEnd, 0.5f);
                break;
            case Constants.stringStarEnd: //选中糖果特效
                transform.gameObject.SetActive(false);
                break;
            case Constants.stringLogoEnd: //LOGO
                Game.Instance.SetState(GameState.NARRATOR);
                break;
            case Constants.stringCounterEnd: //计数器20的特效
                Game.Instance.animatorCounter.SetBool(Constants.stringRun, false);
                break;
            case Constants.stringCounterNumEnd: //计数器10的特效结束
                Game.Instance.counter.ShowNum();
                transform.gameObject.SetActive(false);
                break;
            case Constants.stringCounterNumEffect: //计数器10的特效
                Game.Instance.animatorCounter.transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.1f).SetEase(Ease.OutQuart).OnComplete(() =>
                {
                    Game.Instance.animatorCounter.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBounce).OnComplete(() =>
                    {
                    });
                });
                break;
        }
    }

    /// <summary>
    /// 动画事件播放音效
    /// </summary>
    /// <param name="EventName">播放音效名称</param>
    public void SoundEvent(string EventName)
    {
        Player.Instance.PlayEffect(EventName);
    }

    /// <summary> 游戏结束动画事件 显示重玩按钮 </summary>
    void GameEnd()
    {
        Game.Instance.gameObjectGameEndButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
