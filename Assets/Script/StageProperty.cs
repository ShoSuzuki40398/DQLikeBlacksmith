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

    // �쐬���铹��̉摜
    [SerializeField]
    private List<Sprite> itemSprites;
    public List<Sprite> ItemSprites {  get{ return itemSprites; } }

    // �M�x(�F�̔M���A�ł������̐i�����ɉe��)
    [SerializeField]
    private int heatLevel;
    public int HeatLevel { get { return heatLevel; } }

    // �̗�(�c��̑łĂ�ڈ�)
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }

    /// ���̑��K�v�ȃp�����[�^�[������Βǉ����Ă���
    
    // ���}�X��
    public int TotalCellCount { get { return horizontalCellNum * verticalCellNum; } }
}
