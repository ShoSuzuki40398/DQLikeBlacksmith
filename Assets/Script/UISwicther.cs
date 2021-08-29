using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class UISwicther : MonoBehaviour
{
    public enum UI_NUMBER
    {
        TITLE,
        INAGAME,
        OUTGAME
    }



    [SerializeField]
    private List<GameObject> mainUIs = new List<GameObject>();    

    private Dictionary<UI_NUMBER, GameObject> uiPairs = new Dictionary<UI_NUMBER, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        var enumNames = Enum.GetNames(typeof(UI_NUMBER));

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
        foreach (var number in Enum.GetValues(typeof(UI_NUMBER)))
        {
            uiPairs.Add((UI_NUMBER)number,mainUIs[index]);
            index++;
        }
    }

    /// <summary>
    /// UIêÿë÷
    /// </summary>
    public void switchUI(UI_NUMBER number)
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
}
