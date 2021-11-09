using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特技コマンド制御
/// </summary>
public class SkillManager : MonoBehaviour
{
    // スキル情報
    [SerializeField]
    private List<SkillPropertyAsset> skillPropertyAssets = new List<SkillPropertyAsset>();
    // 「たたく」を除いて全て取得
    public List<SkillProperty> SkillProperties
    { get
        {
            List<SkillProperty> res = new List<SkillProperty>();

            foreach(var asset in skillPropertyAssets)
            {
                res.Add(asset.skillProperty);
            }
            res.RemoveAt(0);

            return res;
        }
    }

    // 「たたく」「上下打ち」を除いて全て取得
    public List<SkillProperty> SkillPropertiesWithoutLongHit
    {
        get
        {
            List<SkillProperty> res = new List<SkillProperty>();

            foreach (var asset in skillPropertyAssets)
            {
                if(asset.skillProperty.Type == SkillProperty.SKILL_TYPE.LONG_HIT || asset.skillProperty.Type == SkillProperty.SKILL_TYPE.STANDARD_HIT)
                {
                    continue;
                }
                res.Add(asset.skillProperty);
            }

            return res;
        }
    }

    // 「たたく」情報を取得
    public SkillProperty StandardHit { get { return skillPropertyAssets[0].skillProperty; } }

    // スキル数
    public int SkillCount { get { return skillPropertyAssets.Count - 1; } }

    /// <summary>
    /// スキル情報取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public SkillProperty GetProperty(int index)
    {
        if (index > SkillProperties.Count)
        {
            return null;
        }

        return SkillProperties[index];
    }

    /// <summary>
    /// スキル名をすべて取得
    /// </summary>
    /// <returns></returns>
    public List<string> GetSkillNames()
    {
        List<string> res = new List<string>();

        foreach(var property in SkillProperties)
        {
            res.Add(property.SkillStr);
        }

        return res;
    }

    /// <summary>
    /// スキル説明をすべて取得
    /// </summary>
    /// <returns></returns>
    public List<string> GetSkillDiscription()
    {
        List<string> res = new List<string>();

        foreach (var property in SkillProperties)
        {
            res.Add(property.SkillDiscriptionStr);
        }

        return res;
    }

    /// <summary>
    /// スキル名を取得
    /// </summary>
    /// <returns></returns>
    public string GetSkillName(int index)
    {
        if (index > SkillProperties.Count)
        {
            return "";
        }
        return SkillProperties[index].SkillStr;
    }

    /// <summary>
    /// スキル説明を取得
    /// </summary>
    /// <returns></returns>
    public string GetSkillDiscription(int index)
    {
        if(index > SkillProperties.Count)
        {
            return "";
        }
        return SkillProperties[index].SkillDiscriptionStr;
    }
}
