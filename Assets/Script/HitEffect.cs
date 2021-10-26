using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    private const string endAnimName = "Base Layer.FadeOutAnimation";

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Text text;

    private bool fire = false;

    // 移動量Y
    [SerializeField]
    private float moveYValue = 30f;

    // Y軸移動にかかる時間
    [SerializeField]
    private float moveYTime = 0.8f;

    // Update is called once per frame
    void Update()
    {
        if (!fire)
        {
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash(endAnimName))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// テキストに数値を設定
    /// </summary>
    public void SetValue(float value)
    {
        text.text = ((int)value).ToString();
    }

    public void Fire()
    {
        fire = true;
        this.transform.DOMoveY(transform.position.y +  moveYValue,moveYTime);
    }
}
