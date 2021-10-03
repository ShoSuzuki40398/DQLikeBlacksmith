using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillProperty
{
    public enum SKILL_TYPE
    {
        STANDARD_HIT,   // 叩く
        LARGE_HIT,      // 強く叩く
        SMALL_HIT,      // 弱く叩く
        RANDOM_HIT      // ランダムに数回叩く
    }

    // 種類
    [SerializeField]
    private SKILL_TYPE type;
    public SKILL_TYPE Type { get { return type; } }

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

    // 進捗値
    [SerializeField]
    private int value = 0;
    public int Value { get { return value; } }

    // 進捗値振れ幅（重み）
    [SerializeField]
    private float weightValue = 0;
    public float WeightValue { get { return weightValue; } }

    // 実行回数
    [SerializeField]
    private int count = 1;
    public int Count { get { return count; } }

    // セル選択に移るか（trueで移る）
    // （ランダム打ちなどは選択する必要がないため）
    [SerializeField]
    private bool isCellSelected = true;
    public bool IsCellSelected { get { return isCellSelected; } }
}
