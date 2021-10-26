using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillProperty
{
    // �X�L�����
    public enum SKILL_TYPE
    {
        STANDARD_HIT,   // �@��
        LARGE_HIT,      // �����@��
        SMALL_HIT,      // �キ�@��
        RANDOM_HIT,     // �����_���ɐ���@��
        HEAT_UP,        // ���x�㏸
        COOL_DOWN,      // ���x���~
        WIDE_HIT,       // ���E�ł�
        LONG_HIT        // �㉺�ł�
    }

    // �X�L�����s���
    public enum SKILL_EXEC_TYPE
    {
        NORMAL,     // �w��̃}�X��I�����Ă������n
        RANDOM,     // �����_���ȃ}�X��I�����Ă������n
        MULTI,      // �����͈͂�I�����Ă������n
        HEAT_LEVEL  // ���x�ύX
    }


    // ���
    [SerializeField]
    private SKILL_TYPE type;
    public SKILL_TYPE Type { get { return type; } }

    // ���s���
    [SerializeField]
    private SKILL_EXEC_TYPE eType;
    public SKILL_EXEC_TYPE Etype { get { return eType; } }

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

    // �X�L�����s�ɕK�v�ȉ��x���x��
    [SerializeField]
    private int needHeatLevel = 0;
    public int NeedHeatLevel { get { return needHeatLevel; } }

    // �i���l
    [SerializeField]
    private int value = 0;
    public int Value { get { return value; } }

    // �i���l�U�ꕝ�i�d�݁j
    [SerializeField]
    private float weightValue = 0;
    public float WeightValue { get { return weightValue; } }

    /// <summary>
    /// �d�ݍ��݂̐i���l
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

    // ���s��
    [SerializeField]
    private int count = 1;
    public int Count { get { return count; } }
    
    public SkillProperty Clone()
    {
        return (SkillProperty)MemberwiseClone();
    }
}
