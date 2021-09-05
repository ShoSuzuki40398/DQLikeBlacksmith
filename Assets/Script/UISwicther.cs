using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class UISwicther : MonoBehaviour
{
    public enum UI_INDEX
    {
        TITLE,
        SELECT,
        MAIN
    }



    [SerializeField]
    private List<GameObject> mainUIs = new List<GameObject>();    

    private Dictionary<UI_INDEX, GameObject> uiPairs = new Dictionary<UI_INDEX, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        var enumNames = Enum.GetNames(typeof(UI_INDEX));

        if(enumNames.Length <= mainUIs.Count)
        {
            SetUIinDic();
        }
    }

    /// <summary>
    /// ListÇDicÇ…ãlÇﬂÇÈ
    /// </summary>
    private void SetUIinDic()
    {
        int index = 0;
        foreach (var number in Enum.GetValues(typeof(UI_INDEX)))
        {
            uiPairs.Add((UI_INDEX)number,mainUIs[index]);
            index++;
        }
    }

    /// <summary>
    /// UIêÿë÷
    /// </summary>
    public void switchUI(UI_INDEX number)
    {
        if(uiPairs.ContainsKey(number))
        {
            foreach(var obj in uiPairs.Values)
            {
                obj.SetActive(false);
            }

            uiPairs[number].SetActive(true);
        }
    }

    public GameObject GetUI(UI_INDEX index)
    {
        return uiPairs[index];
    }
}
