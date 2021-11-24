using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;

public static class MonoBehaviourExtention
{
    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private static IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private static IEnumerator DelayMethod(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }

    public static void Delay(this MonoBehaviour self, float waitTime, Action action)
    {
        self.StartCoroutine(DelayMethod(waitTime,action));
    }

    public static void Delay(this MonoBehaviour self, int delayFrameCount, Action action)
    {
        self.StartCoroutine(DelayMethod(delayFrameCount, action));
    }


    ///// <summary>
    /////  MonoBehaviourを継承したクラスの内容を文字列として返します。
    ///// </summary>
    //public static string ToString2<T>(this T obj) where T : MonoBehaviour
    //{
    //    Type t = typeof(T);

    //    var txt = new System.Text.StringBuilder();

    //    //  GameObject名を取得
    //    txt.Append(((MonoBehaviour)obj).name);

    //    // Get Public Fields
    //    FieldInfo[] fields = t.GetFields();
    //    foreach (FieldInfo f in fields)
    //    {
    //        string fName = ObjectNames.NicifyVariableName(f.Name);
    //        string fValue = f.GetValue(obj).ToString();
    //        txt.Append(string.Format(" [{0}:{1}]", fName, fValue));
    //    }

    //    return txt.ToString();
    //}

    ///// <summary>
    /////  MonoBehaviourを継承したクラスの内容を文字列として返します。
    ///// </summary>
    //public static Dictionary<string,string> ToString3<T>(this T obj) where T : MonoBehaviour
    //{
    //    Type t = typeof(T);

    //    var txt = new System.Text.StringBuilder();

    //    //  GameObject名を取得
    //    txt.Append(((MonoBehaviour)obj).name);

    //    var res = new Dictionary<string, string>();

    //    // Get Public Fields
    //    FieldInfo[] fields = t.GetFields();
    //    foreach (FieldInfo f in fields)
    //    {
    //        string fName = ObjectNames.NicifyVariableName(f.Name);
    //        string fValue = f.GetValue(obj).ToString();
    //        //txt.Append(string.Format(" [{0}:{1}]", fName, fValue));
    //        res.Add(fName,fValue);
    //    }

    //    return res;
    //}
}
