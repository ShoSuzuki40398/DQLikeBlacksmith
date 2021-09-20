using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CommandProperty
{
    // �R�}���h���
    public enum COMMAND_TYPE
    {
        HIT,    // ������
        SKILL,  // ���Z
        CHECK,  // ���킵������
        FINISH  // ��������
    }

    // ���
    [SerializeField]
    private COMMAND_TYPE type;
    public COMMAND_TYPE Type { get { return type; } }
    
    // �R�}���h��
    [SerializeField]
    private string commandStr;
    public string CommandStr { get { return commandStr; } }

    // �R�}���h����
    [SerializeField]
    private string commandDiscriptionStr;
    public string CommandDiscriptionStr { get { return commandDiscriptionStr; } }

    // �R�}���h���s�ɕK�v�ȑ̗�
    [SerializeField]
    private int needHp = 0;
    public int NeedHp { get { return needHp; } }
}
