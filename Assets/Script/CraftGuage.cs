using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���앨�i���Q�[�W�I�u�W�F�N�g
/// </summary>
public class CraftGuage : MonoBehaviour
{
    public enum GUAGE_STATUS
    {
        IDEAL,      // ���z
        SUCCESS,    // ����
        NORMAL,     // ����
    }


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

    // ���ݐi���x�e�L�X�g
    [SerializeField]
    private Text currentValueText;

    [SerializeField]
    private ImageFakeBloom fakeBloom;

    [SerializeField]
    private float maxBlurSig = 10.0f;

    // �X���C�_�[�l
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
        barImage.color = color;
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

    /// <summary>
    /// ���ݐi���x��\������
    /// </summary>
    public void ShowValue()
    {
        currentValueText.text = slider.value.ToString();
        currentValueText.gameObject.SetActive(true);
    }

    /// <summary>
    /// ���ݐi���x���B��
    /// </summary>
    public void HideValue()
    {
        currentValueText.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���z�ʒu�_��
    /// </summary>
    public void LightOnIdealPoint()
    {
        SetIdealBlurSig(maxBlurSig);
    }

    /// <summary>
    /// ���z�ʒu����
    /// </summary>
    public void LightOffIdealPoint()
    {
        SetIdealBlurSig(0);
    }

    /// <summary>
    /// ���z�ʒu�̃u���[�l�ݒ�
    /// </summary>
    /// <param name="value"></param>
    public void SetIdealBlurSig(float value)
    {
        fakeBloom.BlurSig = value;
        fakeBloom.UpdateGlow();
    }

    /// <summary>
    /// �i���Q�[�W�̏�Ԏ擾
    /// </summary>
    /// <param name="cellProperty"></param>
    /// <returns></returns>
    public GUAGE_STATUS GetGuageStatus(ItemCellProperty cellProperty)
    {
        GUAGE_STATUS res = GUAGE_STATUS.NORMAL;

        // �F�ς�
        float idealRangeValue = Define.idealRangeValue;
        if (SliderValue <= (cellProperty.IdealValue + idealRangeValue) && SliderValue >= (cellProperty.IdealValue - idealRangeValue))
        {   // ���z�l�͈�
            res = GUAGE_STATUS.IDEAL;
        }
        else if (SliderValue <= cellProperty._SuccessArea.max && SliderValue >= cellProperty._SuccessArea.min)
        {   // �����l�͈�
            res = GUAGE_STATUS.SUCCESS;
        }
        else
        {   // ���ʔ͈�
            res = GUAGE_STATUS.NORMAL;
        }

        return res;
    }
}
