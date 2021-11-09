using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���앨�}�X�I�u�W�F�N�g
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
    /// ������
    /// </summary>
    public void Initialized()
    {
        SetMaskColor(startMaskColor);
    }

    /// <summary>
    /// �i�����Ń}�X�N�̐F��ς���
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
