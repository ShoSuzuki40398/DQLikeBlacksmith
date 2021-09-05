using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T>
{
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    public virtual void Enter() { }

    public virtual void Execute() { }

    public virtual void Exit() { }
}
