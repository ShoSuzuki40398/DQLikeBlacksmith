using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtention
{
    /// <summary>
    /// 指定されたコンポーネントがアタッチされているか
    /// </summary>
    public static bool HasComponent<T>(this GameObject self) where T : Component
    {
        return self.GetComponent<T>() != null;
    }

    /// <summary>
    /// 指定されたコンポーネントを取得
    /// アタッチされていない場合は追加する
    /// </summary>
    public static T ForceGetComponent<T>(this GameObject self)where T : Component
    {
        if(!self.HasComponent<T>())
        {
            return self.AddComponent<T>();
        }

        return self.GetComponent<T>();
    }

    /// <summary>
    /// 指定されたコンポーネントをすべて破棄する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    public static void DestroyAllComponent<T>(this GameObject self) where T : Component
    {
        var targets = self.GetComponents<T>();

        foreach(var target in targets)
        {
            GameObject.Destroy(target);
        }
    }
}
