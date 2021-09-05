using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;

[CreateAssetMenu(fileName = "StageProperty", menuName = "ScriptableObjects/CreateStagePropertyAsset")]
public class StagePropertyAsset : ScriptableObject
{
    // �X�e�[�W���
    public StageProperty stageProperty;

    public void Console()
    {
        Debug.Log("stageIndex�F" + stageProperty.StageIndex);
    }
}
