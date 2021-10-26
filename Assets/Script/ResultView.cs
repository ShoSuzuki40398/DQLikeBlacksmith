using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    [SerializeField]
    private GameObject resultPanel;

    // 黒背景イメージ
    [SerializeField]
    private Image backgroundImage;

    // 製作物イメージ
    [SerializeField]
    private Image itemImage;

    // ★画像
    [SerializeField]
    private List<Sprite> starSprites;
    private Sprite YellowStar { get { return starSprites[0]; } }
    private Sprite GrayStar { get { return starSprites[1]; } }

    // ★オブジェクト
    [SerializeField]
    private List<Image> starImages;

    // フッター
    [SerializeField]
    private GameObject footer;

    // フッター表示までの時間
    [SerializeField]
    private float timeToActiveFooter = 2.0f;

    // リザルト表示までの時間
    private float timeToActiveResult = 2.0f;

    // リザルト表示後の黒背景アルファ値
    private float alphaToActiveResult = 0.6f;

    // リザルト表示が終了したか
    public bool isFinishResult { get; private set; } = false;


    /// <summary>
    /// リザルト表示
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
    /// 黒背景表示
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

            // 変更終了
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
    /// フッター表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecActiveFooter()
    {
        float time = timeToActiveFooter;
        float startTime = Time.timeSinceLevelLoad;

        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;

            // 変更終了
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
    /// リセット
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

    // 背景黒画像を透明にする
    private void ClearBackground()
    {
        backgroundImage.color = new Color(0, 0, 0, 0);
    }

    /// <summary>
    /// ★を灰色画像にする
    /// </summary>
    public void ClearStar()
    {
        foreach(var star in starImages)
        {
            star.sprite = GrayStar;
        }
    }

    /// <summary>
    /// 指定位置まで★を黄色にする
    /// </summary>
    public void LightOnStar(int index)
    {
        if(starImages.Count < index + 1)
        {
            return;
        }

        // 最低保証 星1つ(☆★★★)
        for (int i = 0;i < index + 1;i++)
        {
            starImages[i].sprite = YellowStar;
        }
    }

    /// <summary>
    /// 製作物イメージ設定
    /// </summary>
    public void SetItemImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
    }
}
