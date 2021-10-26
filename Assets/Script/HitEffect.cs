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

    // �ړ���Y
    [SerializeField]
    private float moveYValue = 30f;

    // Y���ړ��ɂ����鎞��
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
    /// �e�L�X�g�ɐ��l��ݒ�
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
