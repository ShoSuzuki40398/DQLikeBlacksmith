using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class FooterProperty
{
    // くわしく見る（悪い出来）
    public string DetailTextBad;
    // くわしく見る（普通の出来）
    public string DetailTextNormal;
    // くわしく見る（良い出来）
    public string DetailTextGood;
    // くわしく見る（とても良い出来）
    public string DetailTextGreat;

    // 温度レベル不足
    public string NoHeatLevel;

    // 体力不足
    public string NoHp;

    // これ以上冷やせない
    public string NoCoolDown;
}
