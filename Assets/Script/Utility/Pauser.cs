using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : SingletonMonoBehaviour<Pauser>
{
    // ポーズ状態定義
    public enum STATE
    {
        PAUSE,
        RESUME
    }

    // ポーズ状態
    private STATE state = STATE.RESUME;

    /// <summary>
    /// ポーズ実行
    /// </summary>
    public void Pause()
    {
        if(state == STATE.PAUSE)
        {
            return;
        }
        state = STATE.PAUSE;

        Time.timeScale = 0;
        
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    public void Resume()
    {
        if (state == STATE.RESUME)
        {
            return;
        }
        state = STATE.RESUME;

        Time.timeScale = 1;
        
    }

    /// <summary>
    /// 状態取得
    /// </summary>
    /// <returns></returns>
    public STATE GetState()
    {
        return state;
    }
}
