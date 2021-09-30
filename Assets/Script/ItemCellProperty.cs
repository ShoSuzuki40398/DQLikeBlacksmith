using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// インゲーム製作物のマス情報
/// </summary>
[Serializable]
public class ItemCellProperty
{
    // 成功範囲
    public struct SuccessArea
    {
        public float min;
        public float max;
        public float Width { get { return max - min; } }
    }

    // 分割画像
    [SerializeField]
    private Sprite itemSliceSprite;
    public Sprite ItemSliceSprite { get { return itemSliceSprite; } }

    // 理想値
    [SerializeField]
    private int idealValue;
    public int IdealValue { get { return idealValue; } }
    public float NormalizedIdealValue { get { return idealValue / LimitValuef; } }
    
    // 限界値
    [SerializeField,Tooltip("idealValueより高く設定すること")]
    public int limitValue;
    public float LimitValuef  { get { return limitValue; } }

    // 成功許容値（理想値のから外れてもある程度は成功判定とするための値、限界値と同じく倍率を決めて固定）
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
