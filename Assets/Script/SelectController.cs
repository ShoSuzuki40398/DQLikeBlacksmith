using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    // ステージ選択
    [SerializeField]
    private UISquareFrame selectFrame;
    [SerializeField]
    private List<GameObject> selectIcons;

    // ステージ初期情報
    [SerializeField]
    private List<StagePropertyAsset> stagePropertyAssets = new List<StagePropertyAsset>();
        
    /// <summary>
    /// フォーカスしたいアイコンにフレームを合わせる
    /// </summary>
    public void FocusFrame(int index)
    {
        if (index < 0 || index >= selectIcons.Count)
            return;

        selectFrame.transform.position = selectIcons[index].transform.position;
    }
    
    /// <summary>
    /// アクティブ設定
    /// </summary>
    /// <param name="enable"></param>
    public void SetActive(bool enable)
    {
        selectFrame.gameObject.SetActive(enable);
        foreach (var obj in selectIcons)
        {
            obj.SetActive(enable);
        }
    }
    
    /// <summary>
    /// ステージ情報取得
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public StagePropertyAsset GetStageProperty(int index)
    {
        return stagePropertyAssets[index];
    }
}
