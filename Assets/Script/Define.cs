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
        TITLE,
        MAIN
    }

    // SE定義
    public enum SE
    {
        PLAYER_SHIFT,
        PLAYER_ATTACK_HIT,
        PLAYER_BURST,
        ENEMY_BURST,
        BUTTON_CLICK,
        BUTTON_HIGHLIGHT,
        SCORE_DISPLAY
    }

    // ステージ定義
    public enum STAGE_INDEX
    {
        DAGGER,
        SHIELD,
        AXE
    }


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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            return true;
        }
        return false;
    }

}
