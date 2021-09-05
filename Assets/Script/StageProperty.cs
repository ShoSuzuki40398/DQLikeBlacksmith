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

    // 名前
    [SerializeField]
    private string stageName;

    // 横の長さ
    [SerializeField]
    private float width;

    // 縦の長さ
    [SerializeField]
    private float height;
    
    // 横方向のマスの数
    [SerializeField]
    private int horizontalGridNum;
    
    // 縦方向のマスの数
    [SerializeField]
    private int verticalGridNum;

    // 作成する道具の画像
    [SerializeField]
    private Sprite itemImage;
    public Sprite ItemImage {  get{ return itemImage; } }

    // 熱度(炉の熱さ、打った時の進捗率に影響)
    [SerializeField]
    private int heatLevel;
    public int HeatLevel { get { return heatLevel; } }

    // 体力(残りの打てる目安)
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }

    /// その他必要なパラメーターがあれば追加していく
}
