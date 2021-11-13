using System;
using UnityEngine;

public static class Define
{


    //　ゲーム終了ボタンを押したら実行する
    public static void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		UnityEngine.Application.OpenURL("https://unityroom.com/");
#else
		UnityEngine.Application.Quit();
#endif
    }

    // BGMパス
    public const string bgmPath = "Sound/bgm/";
    // SEパス
    public const string sePath = "Sound/se/";
    
    // BGM定義
    public enum BGM
    {
        MAIN
    }

    // SE定義
    public enum SE
    {
        HAMMER_HIT01,
        SELECT_SOUND01,
        CANCEL_SOUND01,
        DECISION_SOUND01,
        ACCENT01,
        RESULT01,
        RESULT02,
        RESULT03,
        HEATUP01,
        COOLDOWN01
    }

    // ステージ定義
    public enum STAGE_INDEX
    {
        DAGGER,
        SHIELD,
        AXE
    }

    
    // 成功範囲値倍率
    public static float craftSuccessMagnification = 1.1f;

    // 成功範囲内ゲージ色(オレンジ)
    public static Color craftGuageSuccessColor = new Color32(255,207,76,255);

    // 理想範囲内ゲージ色
    public static Color craftGuageIdealColor = Color.yellow;

    // 普通範囲内ゲージ色
    public static Color craftGuageNormalColor = Color.cyan;

    // 理想値の許容範囲(理想値ピッタリはエグいのである程度ずれてもOKとする値)
    public static float idealRangeValue = 1.0f;

    // 進捗（普通）の評価値
    public static int normalDetailValue = 1;
    // 進捗（成功）の評価値
    public static int successDetailValue = 2;
    // 進捗（理想）の評価値
    public static int idealDetailValue = 3;

    public static bool InputUpButton()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            return true;
        }
        return false;
    }

    public static bool InputDownButton()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            return true;
        }
        return false;
    }

    public static bool InputRightButton()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            return true;
        }
        return false;
    }

    public static bool InputLeftButton()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            return true;
        }
        return false;
    }

    public static bool InputEnterButton()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            return true;
        }
        return false;
    }

    public static bool InputBackButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
        {
            return true;
        }
        return false;
    }
}
