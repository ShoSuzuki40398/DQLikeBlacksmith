using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StageProperty
{
    // ID
    [SerializeField]
    private int stageIndex;
    public int StageIndex { get { return stageIndex; } }

    // ���O
    [SerializeField]
    private string stageName;

    // ���̒���
    [SerializeField]
    private float width;

    // �c�̒���
    [SerializeField]
    private float height;
    
    // �������̃}�X�̐�
    [SerializeField,Min(1)]
    private int horizontalCellNum;
    public int HorizontalCellNum { get { return horizontalCellNum; } }

    // �c�����̃}�X�̐�
    [SerializeField, Min(1)]
    private int verticalCellNum;
    public int VerticalCellNum { get { return verticalCellNum; } }

    // �쐬���鐬�ʕ��̏��
    [SerializeField]
    private List<ItemCellProperty> itemCellProperties;
    public List<ItemCellProperty> ItemCellProperties { get { return itemCellProperties; } }
    public List<Sprite> ItemSprites
    {
        get
        {
            List<Sprite> res = new List<Sprite>();
            foreach(var cell in itemCellProperties)
            {
                res.Add(cell.ItemSliceSprite);
            }
            return res;
        }
    }

    // �M�x(�F�̔M���A�ł������̐i�����ɉe��)
    [SerializeField]
    private int heatLevel;
    public int HeatLevel { get { return heatLevel; } }

    // �̗�(�c��̑łĂ�ڈ�)
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }

    // ���ʂ̏o���̃{�[�_�[
    [SerializeField]
    private int normalDetailValue;
    public int NormalDetailValue { get { return normalDetailValue; } }

    // �ǂ��o���̃{�[�_�[
    [SerializeField]
    private int goodDetailValue;
    public int GoodDetailValue { get { return goodDetailValue; } }

    // �������o���̃{�[�_�[
    [SerializeField]
    private int greatDetailValue;
    public int GreatDetailValue { get { return greatDetailValue; } }

    // ���}�X��
    public int TotalCellCount { get { return horizontalCellNum * verticalCellNum; } }
}
