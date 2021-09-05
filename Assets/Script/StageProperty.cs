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
    [SerializeField]
    private int horizontalGridNum;
    
    // �c�����̃}�X�̐�
    [SerializeField]
    private int verticalGridNum;

    // �쐬���铹��̉摜
    [SerializeField]
    private Sprite itemImage;
    public Sprite ItemImage {  get{ return itemImage; } }

    // �M�x(�F�̔M���A�ł������̐i�����ɉe��)
    [SerializeField]
    private int heatLevel;
    public int HeatLevel { get { return heatLevel; } }

    // �̗�(�c��̑łĂ�ڈ�)
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }

    /// ���̑��K�v�ȃp�����[�^�[������Βǉ����Ă���
}
