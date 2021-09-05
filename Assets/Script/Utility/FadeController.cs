using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FadeController : SingletonMonoBehaviour<FadeController>
{
    enum FADE_STATE
    {
        IDLE,   // 待機
        FADEIN, // フェードイン
        FADEOUT, // フェードアウト
    }
    FADE_STATE state = FADE_STATE.IDLE;

    // フェード用パネル
    private Image fadePanel;

    /// <summary>
    /// フェード用パネル作成
    /// </summary>
    private void CreateFadePanel()
    {
        var canvas = GameObject.Find("Canvas");

        var obj = new GameObject("FadePanel");
        var rect = obj.AddComponent<RectTransform>();
        fadePanel = obj.AddComponent<Image>();
        fadePanel.color = Color.black;
        rect.SetParent(canvas.transform);

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.anchoredPosition = Vector2.zero;

        SetAlpha(0.0f);
    }
       
    /// <summary>
    /// フェード
    /// </summary>
    /// <param name="fadeTime">フェードにかける時間</param>
    /// <param name="startAlpha">開始アルファ値</param>
    /// <param name="endAlpha">終了アルファ値</param>
    /// <param name="action">フェード終了時イベント</param>
    /// <returns></returns>
    private IEnumerator Fade(float fadeTime, float startAlpha, float endAlpha, Action action = null)
    {
        fadePanel.gameObject.SetActive(true);

        float elapsedTime = 0.0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            var currentAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTime));
            SetAlpha(currentAlpha);
            yield return new WaitForEndOfFrame();
        }

        if(action != null)
        {
            action();
        }

        if(state == FADE_STATE.FADEIN)
        {
            fadePanel.gameObject.SetActive(false);
        }

        state = FADE_STATE.IDLE;
    }

    /// <summary>
    /// アルファ値設定
    /// </summary>
    /// <param name="a"></param>
    private void SetAlpha(float a)
    {
        var color = fadePanel.color;
        color.a = a;
        fadePanel.color = color;
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    /// <param name="time"></param>
    public void FadeIn(float time, Action action = null)
    {
        // フェード中は無効
        if (state != FADE_STATE.IDLE)
        {
            return;
        }

        if (fadePanel == null)
        {
            CreateFadePanel();
        }

        state = FADE_STATE.FADEIN;

        StartCoroutine(Fade(time, 1.0f, 0.0f, action));
    }

    /// <summary>
    /// フェードアウト開始
    /// </summary>
    /// <param name="time"></param>
    public void FadeOut(float time, Action action = null)
    {
        // フェード中は無効
        if (state != FADE_STATE.IDLE)
        {
            return;
        }

        if (fadePanel == null)
        {
            CreateFadePanel();
        }

        state = FADE_STATE.FADEOUT;

        StartCoroutine(Fade(time, 0.0f, 1.0f, action));
    }
}
