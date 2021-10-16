using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    public enum TITLE_INDEX
    {
        START,
        EXIT
    }

    // 選択枠
    [SerializeField]
    private UISquareFrame selectFrame;

    // 選択対象オブジェクト
    [SerializeField]
    private List<GameObject> selectObjects;
    private int SelectNum { get { return selectObjects.Count; } }

    // 選択中インデックス
    private int currentSelectIndex = 0;

    // 枠の点滅時間
    [SerializeField]
    private float frameFlashTime = 1.0f;

    private void Start()
    {
        // 選択枠を初期位置に移動
        MonoBehaviourExtention.Delay(this,1,()=> selectFrame.transform.position = selectObjects[currentSelectIndex].transform.position);
    }

    /// <summary>
    /// フォーカスしたいアイコンにフレームを合わせる
    /// </summary>
    public void FocusFrame(int index)
    {
        if (index < 0 || index >= selectObjects.Count)
            return;

        selectFrame.transform.position = selectObjects[index].transform.position;
    }

    /// <summary>
    /// 選択枠点滅
    /// </summary>
    public void FlashingFrame()
    {
        selectFrame.Flashing(frameFlashTime);
    }

    /// <summary>
    /// 選択枠の色を戻す
    /// </summary>
    public void ResetFrameColor()
    {
        selectFrame.ResetColor();
    }

    public TITLE_INDEX GetSelectIndex()
    {
        foreach (TITLE_INDEX value in Enum.GetValues(typeof(TITLE_INDEX)))
        {
            var intValue = (int)value;
            if(currentSelectIndex == intValue)
            {
                return value;
            }
        }

        return TITLE_INDEX.EXIT;
    }

    public void FocusUp()
    {
        currentSelectIndex = Mathf.Clamp(currentSelectIndex - 1, 0, SelectNum - 1);
        FocusFrame(currentSelectIndex);
    }

    public void FocusDown()
    {
        currentSelectIndex = Mathf.Clamp(currentSelectIndex + 1, 0, SelectNum - 1);
        FocusFrame(currentSelectIndex);
    }
}
