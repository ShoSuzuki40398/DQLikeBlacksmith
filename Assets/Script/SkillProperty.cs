using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillProperty
{
    // スキル種類
    public enum SKILL_TYPE
    {
        STANDARD_HIT,   // 叩く
        LARGE_HIT,      // 強く叩く
        SMALL_HIT,      // 弱く叩く
        RANDOM_HIT,     // ランダムに数回叩く
        HEAT_UP,        // 温度上昇
        COOL_DOWN,      // 温度下降
        WIDE_HIT,       // 左右打ち
        LONG_HIT        // 上下打ち
    }

    // スキル実行種類
    public enum SKILL_EXEC_TYPE
    {
        NORMAL,     // 指定のマスを選択してたたく系
        RANDOM,     // ランダムなマスを選択してたたく系
        MULTI,      // 複数範囲を選択してたたく系
        HEAT_LEVEL  // 温度変更
    }


    // 種類
    [SerializeField]
    private SKILL_TYPE type;
    public SKILL_TYPE Type { get { return type; } }

    // 実行種類
    [SerializeField]
    private SKILL_EXEC_TYPE eType;
    public SKILL_EXEC_TYPE Etype { get { return eType; } }

    // スキル名
    [SerializeField]
    private string skillStr;
    public string SkillStr { get { return skillStr; } }

    // スキル説明
    [SerializeField]
    private string skillDiscriptionStr;
    public string SkillDiscriptionStr { get { return skillDiscriptionStr; } }

    // スキル実行に必要な体力
    [SerializeField]
    private int needHp = 0;
    public int NeedHp { get { return needHp; } }

    // スキル実行に必要な温度レベル
    [SerializeField]
    private int needHeatLevel = 0;
    public int NeedHeatLevel { get { return needHeatLevel; } }

    // 進捗値
    [SerializeField]
    private int value = 0;
    public int Value { get { return value; } }

    // 進捗値振れ幅（重み）
    [SerializeField]
    private float weightValue = 0;
    public float WeightValue { get { return weightValue; } }

    /// <summary>
    /// 重み込みの進捗値
    /// </summary>
    public int ValueWithWeight
    {
        get
        {
            int min = (int)(value * (1.0f - (weightValue -1.0f)));
            int max = (int)(value * weightValue);
            return UnityEngine.Random.Range(min,max);
        }
    }

    // 実行回数
    [SerializeField]
    private int count = 1;
    public int Count { get { return count; } }
    
    public SkillProperty Clone()
    {
        return (SkillProperty)MemberwiseClone();
    }
}
