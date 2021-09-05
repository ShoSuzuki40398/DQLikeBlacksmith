using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    // ステージ情報を初期値とする進行中ステージ情報
    private StageProperty currentStageProperty;

    // 温度テキスト
    [SerializeField]
    private Text heatLevelText;

    // 製作物イメージ（仮）のちのち分割したものを表示することになる
    [SerializeField]
    private Image craftImage;

    /// <summary>
    /// ステージ情報設定
    /// </summary>
    public void SetPropertyAsset(StagePropertyAsset propertyAsset)
    {
        currentStageProperty = propertyAsset.stageProperty;
        heatLevelText.text = currentStageProperty.HeatLevel.ToString() + "℃";
        craftImage.sprite = currentStageProperty.ItemImage;
    }
}
