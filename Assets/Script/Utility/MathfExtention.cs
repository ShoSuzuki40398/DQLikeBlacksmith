using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MathfExtention
{
    /// <summary>
    /// 汎用値範囲チェック
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool IsRange<T>(this T value, T min, T max) where T : IComparable
    {
        bool res = true;

        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            res = false;
        }

        return res;
    }
}
