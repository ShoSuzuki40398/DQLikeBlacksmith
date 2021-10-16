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

    // �I��g
    [SerializeField]
    private UISquareFrame selectFrame;

    // �I��ΏۃI�u�W�F�N�g
    [SerializeField]
    private List<GameObject> selectObjects;
    private int SelectNum { get { return selectObjects.Count; } }

    // �I�𒆃C���f�b�N�X
    private int currentSelectIndex = 0;

    // �g�̓_�Ŏ���
    [SerializeField]
    private float frameFlashTime = 1.0f;

    private void Start()
    {
        // �I��g�������ʒu�Ɉړ�
        MonoBehaviourExtention.Delay(this,1,()=> selectFrame.transform.position = selectObjects[currentSelectIndex].transform.position);
    }

    /// <summary>
    /// �t�H�[�J�X�������A�C�R���Ƀt���[�������킹��
    /// </summary>
    public void FocusFrame(int index)
    {
        if (index < 0 || index >= selectObjects.Count)
            return;

        selectFrame.transform.position = selectObjects[index].transform.position;
    }

    /// <summary>
    /// �I��g�_��
    /// </summary>
    public void FlashingFrame()
    {
        selectFrame.Flashing(frameFlashTime);
    }

    /// <summary>
    /// �I��g�̐F��߂�
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
