using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{
    // �X�e�[�W���������l�Ƃ���i�s���X�e�[�W���
    private StageProperty currentStageProperty;

    // ���x�e�L�X�g
    [SerializeField]
    private Text heatLevelText;

    // ���앨�C���[�W�i���j�̂��̂������������̂�\�����邱�ƂɂȂ�
    [SerializeField]
    private Image craftImage;

    /// <summary>
    /// �X�e�[�W���ݒ�
    /// </summary>
    public void SetPropertyAsset(StagePropertyAsset propertyAsset)
    {
        currentStageProperty = propertyAsset.stageProperty;
        heatLevelText.text = currentStageProperty.HeatLevel.ToString() + "��";
        craftImage.sprite = currentStageProperty.ItemImage;
    }
}
