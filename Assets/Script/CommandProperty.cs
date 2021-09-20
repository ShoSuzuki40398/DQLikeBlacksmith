using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CommandProperty
{
    // コマンド種類
    public enum COMMAND_TYPE
    {
        HIT,    // たたく
        SKILL,  // 特技
        CHECK,  // くわしく見る
        FINISH  // しあげる
    }

    // 種類
    [SerializeField]
    private COMMAND_TYPE type;
    public COMMAND_TYPE Type { get { return type; } }
    
    // コマンド名
    [SerializeField]
    private string commandStr;
    public string CommandStr { get { return commandStr; } }

    // コマンド説明
    [SerializeField]
    private string commandDiscriptionStr;
    public string CommandDiscriptionStr { get { return commandDiscriptionStr; } }

    // コマンド実行に必要な体力
    [SerializeField]
    private int needHp = 0;
    public int NeedHp { get { return needHp; } }
}
