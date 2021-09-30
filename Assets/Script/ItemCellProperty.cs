using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// �C���Q�[�����앨�̃}�X���
/// </summary>
[Serializable]
public class ItemCellProperty
{
    // �����͈�
    public struct SuccessArea
    {
        public float min;
        public float max;
        public float Width { get { return max - min; } }
    }

    // �����摜
    [SerializeField]
    private Sprite itemSliceSprite;
    public Sprite ItemSliceSprite { get { return itemSliceSprite; } }

    // ���z�l
    [SerializeField]
    private int idealValue;
    public int IdealValue { get { return idealValue; } }
    public float NormalizedIdealValue { get { return idealValue / LimitValuef; } }
    
    // ���E�l
    [SerializeField,Tooltip("idealValue��荂���ݒ肷�邱��")]
    public int limitValue;
    public float LimitValuef  { get { return limitValue; } }

    // �������e�l�i���z�l�̂���O��Ă�������x�͐�������Ƃ��邽�߂̒l�A���E�l�Ɠ������{�������߂ČŒ�j
    public SuccessArea _SuccessArea
    {
        get
        {
            SuccessArea res = new SuccessArea();
            res.max = idealValue * Define.craftSuccessMagnification;
            res.min = idealValue * (1.0f - (Define.craftSuccessMagnification - 1.0f));

            return res;
        }
    }
}
