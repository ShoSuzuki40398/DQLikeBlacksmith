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
    /// 子要素の指定したコンポーネントをすべて取得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static List<T> GetComponentsOnlyChildren<T>(this GameObject self) where T: Component
    {
        var children = self.GetComponentsInChildren<T>();

        List<T> res = new List<T>(children);

        foreach(var com in res)
        {
            if(com.gameObject.GetInstanceID() == self.GetInstanceID())
            {
                res.Remove(com);
                break;
            }
        }

        return res;
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

    /// <summary>
    /// 子オブジェクトをすべて削除1
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    public static void DestroyChildren(this GameObject self)
    {
        foreach (Transform child in self.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
