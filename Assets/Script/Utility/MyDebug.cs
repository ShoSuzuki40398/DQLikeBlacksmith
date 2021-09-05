using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyDebug
{
    public static void Log<T>(T value)
    {
        Debug.Log(value);
    }
}
