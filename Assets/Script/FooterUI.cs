using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterUI : MonoBehaviour
{
    [SerializeField]
    private Text footerText;

    public void SetFooterText(string str)
    {
        footerText.text = str;
    }

    public void ClearFooterText()
    {
        footerText.text = "";
    }
}
