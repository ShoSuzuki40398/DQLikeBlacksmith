using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;

[CreateAssetMenu(fileName = "StageProperty", menuName = "ScriptableObjects/CreateStagePropertyAsset")]
public class StagePropertyAsset : ScriptableObject
{
    // ステージ情報
    public StageProperty stageProperty;

    public void Console()
    {
        Debug.Log("stageIndex：" + stageProperty.StageIndex);
    }
}
