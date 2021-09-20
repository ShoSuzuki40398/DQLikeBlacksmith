using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    // �X�e�[�W�I��
    [SerializeField]
    private UISquareFrame selectFrame;
    [SerializeField]
    private List<GameObject> selectIcons;

    // �X�e�[�W�������
    [SerializeField]
    private List<StagePropertyAsset> stagePropertyAssets = new List<StagePropertyAsset>();
        
    /// <summary>
    /// �t�H�[�J�X�������A�C�R���Ƀt���[�������킹��
    /// </summary>
    public void FocusFrame(int index)
    {
        if (index < 0 || index >= selectIcons.Count)
            return;

        selectFrame.transform.position = selectIcons[index].transform.position;
    }
    
    /// <summary>
    /// �A�N�e�B�u�ݒ�
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
    /// �X�e�[�W���擾
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public StagePropertyAsset GetStageProperty(int index)
    {
        return stagePropertyAssets[index];
    }
}
