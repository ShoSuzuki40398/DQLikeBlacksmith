using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���앨�i���Q�[�W�I�u�W�F�N�g
/// </summary>
public class CraftGuage : MonoBehaviour
{
    // �Q�[�W����p�X���C�_�[
    [SerializeField]
    private Slider slider;

    // �i�����Q�[�W
    [SerializeField]
    private Image barImage;

    // �����͈�
    [SerializeField]
    private Image successArea;

    // ���z�ʒu
    [SerializeField]
    private Image idealPoint;

    public float SliderValue { get { return slider.value; } set { slider.value = value; } }

    public void SetSliderMaxValue(float value)
    {
        slider.maxValue = value;
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public void SetSliderColor(Color color)
    {
        ColorBlock block = new ColorBlock();
        block.normalColor = color;
        slider.colors = block;
    }

    public void SetDirection(Slider.Direction dir)
    {
        slider.direction = dir;
    }

    public void SetSliderWidth(float width)
    {
        var size = slider.GetComponent<RectTransform>().sizeDelta;
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(width, size.y);
    }

    public void SetSuccessAreaWidth(float width)
    {
        var size = successArea.GetComponent<RectTransform>().sizeDelta;
        successArea.GetComponent<RectTransform>().sizeDelta = new Vector2(width, size.y);
    }

    public void SetIdealPointX(float x)
    {
        idealPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(x,0);
    }

    public void SetIdealPointAnchorRight()
    {
        Vector2 max = new Vector2(1.0f, 1.0f);
        Vector2 min = new Vector2(1.0f, 1.0f);
        SetIdealPointAnchor(max,min);
    }

    public void SetIdealPointAnchorLeft()
    {
        Vector2 max = new Vector2(0.0f, 1.0f);
        Vector2 min = new Vector2(0.0f, 1.0f);
        SetIdealPointAnchor(max, min);
    }

    public void SetIdealPointAnchor(Vector2 max, Vector2 min)
    {
        idealPoint.GetComponent<RectTransform>().anchorMax = max;
        idealPoint.GetComponent<RectTransform>().anchorMin = min;
    }
}
