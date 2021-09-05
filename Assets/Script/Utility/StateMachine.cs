using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T,TEnum>
{
    private State<T> currentState;
    private TEnum currentStateKey;

    private Dictionary<TEnum, State<T>> stateList = new Dictionary<TEnum, State<T>>();

    public StateMachine()
    {
        currentState = null;
    }

    /// <summary>
    /// 状態追加
    /// </summary>
    /// <param name="key"></param>
    /// <param name="state"></param>
    public void AddState(TEnum key, State<T> state)
    {
        stateList[key] = state;
    }

    public void ChangeState(TEnum key)
    {
        if (!stateList.ContainsKey(key))
        {
            return;
        }

        if(currentState != null)
        {
            currentState.Exit();
        }

        currentStateKey = key;
        currentState = stateList[key];
        currentState.Enter();
    }

    public void Update()
    {
        if(currentState != null)
        {
            currentState.Execute();
        }
    }

    public bool IsCurrentState(TEnum key)
    {
        return currentState == stateList[key];
    }

    public TEnum GetCurrentStateKey()
    {
        return currentStateKey;
    }
}
