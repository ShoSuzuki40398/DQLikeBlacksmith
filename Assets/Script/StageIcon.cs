using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIcon : MonoBehaviour
{
    [SerializeField]
    private Image mask;
    private Image Mask { get { return mask; } }

    public void SetEnableMask(bool value)
    {
        mask.enabled = value;
    }
}
