using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 製作物マスオブジェクト
/// </summary>
public class ItemSliceImageCell : MonoBehaviour
{
    public Image maskImage;

    public Image itemImage;

    public Image backImage;

    public UISquareFrame squareFrame;

    [SerializeField]
    private Color startMaskColor;

    [SerializeField]
    private Color endMaskColor;

    public void SetMaskColor(Color color)
    {
        maskImage.color = color;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialized()
    {
        SetMaskColor(startMaskColor);
    }

    /// <summary>
    /// 進捗率でマスクの色を変える
    /// </summary>
    /// <param name="value"></param>
    public void ChangeMaskColorFromGuageRatio(float ratio)
    {
        float t = Mathf.Clamp01(ratio);
        float r = Mathf.Lerp(startMaskColor.r, endMaskColor.r, t);
        float g = Mathf.Lerp(startMaskColor.g, endMaskColor.g, t);
        float b = Mathf.Lerp(startMaskColor.b, endMaskColor.b, t);
        SetMaskColor(new Color(r, g, b, 1));
    }
}
