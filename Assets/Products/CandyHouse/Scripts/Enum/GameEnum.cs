
/// <summary> 糖果类型 </summary>
public enum CandyType
{
    /// <summary> 黄 </summary>
    YELLOW,
    /// <summary> 粉 </summary>
    PINK,
    /// <summary> 绿 </summary>
    GREEN,
    /// <summary> 紫 </summary>
    PURPLE,
    /// <summary> 蓝 </summary>
    BLUE,
}

public enum GameState
{
    /// <summary> 开场 </summary>
    LOGO,
    /// <summary> 旁白 </summary>
    NARRATOR,
    /// <summary> 选择糖果 </summary>
    CHOOSECANDY,
    /// <summary> 转场 </summary>
    TRANSITION,
    /// <summary> 游戏说明1 </summary>
    EXPLAIN1,
    /// <summary> 游戏说明2 </summary>
    EXPLAIN2,
    /// <summary> 开始 </summary>
    READYGO,
    /// <summary> 游戏中 </summary>
    GAME,
    /// <summary> 旁白2 </summary>
    NARRATOR2,
    /// <summary> 结束 </summary>
    END,
}

/// <summary> 主角状态 </summary>
public enum PlayerState
{
    /// <summary> 待机 </summary>
    IDLE,
    /// <summary> 移动 </summary>
    WALK,
    /// <summary> 开心 </summary>
    HAPPY,
    /// <summary> 游戏结束 </summary>
    HAPPYEND,
    /// <summary> 被炸 </summary>
    SAD,
}

/// <summary> 炮状态 </summary>
public enum GunState
{
    /// <summary> 待机 </summary>
    IDLE,
    /// <summary> 开火 </summary>
    FIRE,
}