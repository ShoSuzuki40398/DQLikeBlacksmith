using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 製作物進捗ゲージオブジェクト
/// </summary>
public class CraftGuage : MonoBehaviour
{
    public enum GUAGE_STATUS
    {
        IDEAL,      // 理想
        SUCCESS,    // 成功
        NORMAL,     // 普通
    }


    // ゲージ制御用スライダー
    [SerializeField]
    private Slider slider;

    // 進捗率ゲージ
    [SerializeField]
    private Image barImage;

    // 成功範囲
    [SerializeField]
    private Image successArea;

    // 理想位置
    [SerializeField]
    private Image idealPoint;

    // 現在進捗度テキスト
    [SerializeField]
    private Text currentValueText;

    [SerializeField]
    private ImageFakeBloom fakeBloom;

    [SerializeField]
    private float maxBlurSig = 10.0f;

    // スライダー値
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
    /// 現在進捗度を表示する
    /// </summary>
    public void ShowValue()
    {
        currentValueText.text = slider.value.ToString();
        currentValueText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 現在進捗度を隠す
    /// </summary>
    public void HideValue()
    {
        currentValueText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 理想位置点灯
    /// </summary>
    public void LightOnIdealPoint()
    {
        SetIdealBlurSig(maxBlurSig);
    }

    /// <summary>
    /// 理想位置消灯
    /// </summary>
    public void LightOffIdealPoint()
    {
        SetIdealBlurSig(0);
    }

    /// <summary>
    /// 理想位置のブラー値設定
    /// </summary>
    /// <param name="value"></param>
    public void SetIdealBlurSig(float value)
    {
        fakeBloom.BlurSig = value;
        fakeBloom.UpdateGlow();
    }

    /// <summary>
    /// 進捗ゲージの状態取得
    /// </summary>
    /// <param name="cellProperty"></param>
    /// <returns></returns>
    public GUAGE_STATUS GetGuageStatus(ItemCellProperty cellProperty)
    {
        GUAGE_STATUS res = GUAGE_STATUS.NORMAL;

        // 色変え
        float idealRangeValue = Define.idealRangeValue;
        if (SliderValue <= (cellProperty.IdealValue + idealRangeValue) && SliderValue >= (cellProperty.IdealValue - idealRangeValue))
        {   // 理想値範囲
            res = GUAGE_STATUS.IDEAL;
        }
        else if (SliderValue <= cellProperty._SuccessArea.max && SliderValue >= cellProperty._SuccessArea.min)
        {   // 成功値範囲
            res = GUAGE_STATUS.SUCCESS;
        }
        else
        {   // 普通範囲
            res = GUAGE_STATUS.NORMAL;
        }

        return res;
    }
}
