using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillProperty
{
    public enum SKILL_TYPE
    {
        STANDARD_HIT,   // �@��
        LARGE_HIT,      // �����@��
        SMALL_HIT,      // �キ�@��
        RANDOM_HIT      // �����_���ɐ���@��
    }

    // ���
    [SerializeField]
    private SKILL_TYPE type;
    public SKILL_TYPE Type { get { return type; } }

    // �X�L����
    [SerializeField]
    private string skillStr;
    public string SkillStr { get { return skillStr; } }

    // �X�L������
    [SerializeField]
    private string skillDiscriptionStr;
    public string SkillDiscriptionStr { get { return skillDiscriptionStr; } }

    // �X�L�����s�ɕK�v�ȑ̗�
    [SerializeField]
    private int needHp = 0;
    public int NeedHp { get { return needHp; } }

    // �i���l
    [SerializeField]
    private int value = 0;
    public int Value { get { return value; } }

    // �i���l�U�ꕝ�i�d�݁j
    [SerializeField]
    private float weightValue = 0;
    public float WeightValue { get { return weightValue; } }

    // ���s��
    [SerializeField]
    private int count = 1;
    public int Count { get { return count; } }

    // �Z���I���Ɉڂ邩�itrue�ňڂ�j
    // �i�����_���ł��Ȃǂ͑I������K�v���Ȃ����߁j
    [SerializeField]
    private bool isCellSelected = true;
    public bool IsCellSelected { get { return isCellSelected; } }
}
