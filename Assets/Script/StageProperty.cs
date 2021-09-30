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
    [SerializeField,Min(1)]
    private int horizontalCellNum;
    public int HorizontalCellNum { get { return horizontalCellNum; } }

    // 縦方向のマスの数
    [SerializeField, Min(1)]
    private int verticalCellNum;
    public int VerticalCellNum { get { return verticalCellNum; } }

    // 作成する成果物の情報
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

    // 熱度(炉の熱さ、打った時の進捗率に影響)
    [SerializeField]
    private int heatLevel;
    public int HeatLevel { get { return heatLevel; } }

    // 体力(残りの打てる目安)
    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }

    // 普通の出来のボーダー
    [SerializeField]
    private int normalDetailValue;
    public int NormalDetailValue { get { return normalDetailValue; } }

    // 良い出来のボーダー
    [SerializeField]
    private int goodDetailValue;
    public int GoodDetailValue { get { return goodDetailValue; } }

    // すごい出来のボーダー
    [SerializeField]
    private int greatDetailValue;
    public int GreatDetailValue { get { return greatDetailValue; } }

    // 総マス数
    public int TotalCellCount { get { return horizontalCellNum * verticalCellNum; } }
}
