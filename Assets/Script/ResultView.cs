using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [SerializeField]
    private GameObject resultPanel;

    // ���w�i�C���[�W
    [SerializeField]
    private Image backgroundImage;

    // ���앨�C���[�W
    [SerializeField]
    private Image itemImage;

    // ���摜
    [SerializeField]
    private List<Sprite> starSprites;
    private Sprite YellowStar { get { return starSprites[0]; } }
    private Sprite GrayStar { get { return starSprites[1]; } }

    // ���I�u�W�F�N�g
    [SerializeField]
    private List<Image> starImages;

    // �t�b�^�[
    [SerializeField]
    private GameObject footer;

    // �t�b�^�[�\���܂ł̎���
    [SerializeField]
    private float timeToActiveFooter = 2.0f;

    // ���U���g�\���܂ł̎���
    private float timeToActiveResult = 2.0f;

    // ���U���g�\����̍��w�i�A���t�@�l
    private float alphaToActiveResult = 0.6f;

    // ���U���g�\�����I��������
    public bool isFinishResult { get; private set; } = false;


    /// <summary>
    /// ���U���g�\��
    /// </summary>
    public void DisplayResult(Sprite itemSprite,int revue, Action onComplete = null)
    {
        this.gameObject.SetActive(true);
        resultPanel.SetActive(false);
        footer.SetActive(false);
        itemImage.sprite = itemSprite;
        LightOnStar(revue);
        StartCoroutine(ExecActiveBackground(onComplete));
    }

    /// <summary>
    /// ���w�i�\��
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    private IEnumerator ExecActiveBackground(Action onComplete = null)
    {
        float time = timeToActiveResult;
        float startTime = Time.timeSinceLevelLoad;

        var startValue = 0;

        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;
            var rate = diff / time;
            float alpha = startValue + (alphaToActiveResult * rate);
            backgroundImage.color = new Color(0, 0, 0, alpha);

            // �ύX�I��
            if (diff > time)
            {
                backgroundImage.color = new Color(0, 0, 0, alphaToActiveResult);
                resultPanel.SetActive(true);
                StartCoroutine(ExecActiveFooter());
                onComplete?.Invoke();
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// �t�b�^�[�\��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecActiveFooter()
    {
        float time = timeToActiveFooter;
        float startTime = Time.timeSinceLevelLoad;

        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;

            // �ύX�I��
            if (diff > time)
            {
                footer.SetActive(true);
                isFinishResult = true;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    public void Reset()
    {
        isFinishResult = false;
        footer.SetActive(false);
        ClearBackground();
        ClearStar();
        resultPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    // �w�i���摜�𓧖��ɂ���
    private void ClearBackground()
    {
        backgroundImage.color = new Color(0, 0, 0, 0);
    }

    /// <summary>
    /// �����D�F�摜�ɂ���
    /// </summary>
    public void ClearStar()
    {
        foreach(var star in starImages)
        {
            star.sprite = GrayStar;
        }
    }

    /// <summary>
    /// �w��ʒu�܂Ł������F�ɂ���
    /// </summary>
    public void LightOnStar(int index)
    {
        if(starImages.Count < index + 1)
        {
            return;
        }

        // �Œ�ۏ� ��1��(��������)
        for (int i = 0;i < index + 1;i++)
        {
            starImages[i].sprite = YellowStar;
        }
    }

    /// <summary>
    /// ���앨�C���[�W�ݒ�
    /// </summary>
    public void SetItemImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
    }
}
