using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���Z�R�}���h����
/// </summary>
public class SkillManager : MonoBehaviour
{
    // �X�L�����
    [SerializeField]
    private List<SkillPropertyAsset> skillPropertyAssets = new List<SkillPropertyAsset>();
    // �u�������v�������đS�Ď擾
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

    // �u�������v�u�㉺�ł��v�������đS�Ď擾
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

    // �u�������v�����擾
    public SkillProperty StandardHit { get { return skillPropertyAssets[0].skillProperty; } }

    // �X�L����
    public int SkillCount { get { return skillPropertyAssets.Count - 1; } }

    /// <summary>
    /// �X�L�����擾
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
    /// �X�L���������ׂĎ擾
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
    /// �X�L�����������ׂĎ擾
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
    /// �X�L�������擾
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
    /// �X�L���������擾
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
