using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField, Range(0, 1), Tooltip("マスタ音量")]
    private float masterVolume = 0.5f;
    [SerializeField, Range(0, 1), Tooltip("BGMの音量")]
    private float bgmVolume = 0.5f;
    [SerializeField, Range(0, 1), Tooltip("SEの音量")]
    private float seVolume = 0.5f;

    // SE最大再生個数
    [SerializeField]
    private int maxSeCount = 2;

    private int currentSeSourceIndex = 0;

    // BGNリスト
    private Dictionary<Define.BGM, AudioClip> bgmKeyValues = new Dictionary<Define.BGM, AudioClip>();
    // SEリスト
    private Dictionary<Define.SE, AudioClip> seKeyValues = new Dictionary<Define.SE, AudioClip>();

    // AudioSource
    private AudioSource bgmSource;
    private List<AudioSource> seSources = new List<AudioSource>();

    public float Volume
    {
        set
        {
            masterVolume = Mathf.Clamp01(value);
            bgmSource.volume = bgmVolume * masterVolume;
            foreach (var se in seSources)
            {
                se.volume = seVolume * masterVolume;
            }
        }
        get
        {
            return masterVolume;
        }
    }

    public float BgmVolume
    {
        set
        {
            bgmVolume = Mathf.Clamp01(value);
            bgmSource.volume = bgmVolume * masterVolume;
        }
        get
        {
            return bgmVolume;
        }
    }

    public float SeVolume
    {
        set
        {
            seVolume = Mathf.Clamp01(value);
            foreach (var se in seSources)
            {
                se.volume = seVolume * masterVolume;
            }
        }
        get
        {
            return seVolume;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        RegistBGM(Define.BGM.MAIN, "MainTheme");
        bgmSource = gameObject.ForceGetComponent<AudioSource>();

        RegistSE(Define.SE.HAMMER_HIT01, "HammerHit01");
        RegistSE(Define.SE.SELECT_SOUND01, "System01");
        RegistSE(Define.SE.CANCEL_SOUND01, "System02");
        RegistSE(Define.SE.DECISION_SOUND01, "System03"); 
        RegistSE(Define.SE.ACCENT01, "Accent01");
        RegistSE(Define.SE.RESULT01, "Drum01");
        RegistSE(Define.SE.RESULT02, "Drum02");
        RegistSE(Define.SE.RESULT03, "Drum03");
        RegistSE(Define.SE.HEATUP01, "HeatUp");
        RegistSE(Define.SE.COOLDOWN01, "CoolDown");

        for (int i = 0; i < maxSeCount; ++i)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.volume = masterVolume * seVolume;
            seSources.Add(source);
        }
    }

    /// <summary>
    /// BGM登録
    /// </summary>
    public void RegistBGM(Define.BGM key, string value)
    {
        bgmKeyValues[key] = Resources.Load<AudioClip>(Define.bgmPath + value);
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="key"></param>
    public void PlayBGM(Define.BGM key)
    {
        if (!bgmKeyValues.ContainsKey(key))
        {
            return;
        }

        if (bgmSource.isPlaying)
        {
            // 既に再生されているBGMの場合何もしない
            if(bgmSource.clip == bgmKeyValues[key])
            {
                return;
            }
            bgmSource.Stop();
        }

        AudioClip clip = bgmKeyValues[key];
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            bgmSource.clip = null;
        }
    }

    /// <summary>
    /// BGM一時停止
    /// </summary>
    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    /// <summary>
    /// bgm再開
    /// </summary>
    public void UnPause()
    {
        bgmSource.UnPause();
    }

    /// <summary>
    /// BGM削除
    /// </summary>
    public void RemoveBGM(Define.BGM key)
    {
        if (bgmKeyValues.ContainsKey(key))
        {
            bgmKeyValues.Remove(key);
        }
    }

    /// <summary>
    /// SE登録
    /// </summary>
    public void RegistSE(Define.SE key, string value)
    {
        seKeyValues[key] = Resources.Load<AudioClip>(Define.sePath + value);
    }

    /// <summary>
    /// SE削除
    /// </summary>
    public void RemoveSE(Define.SE key)
    {
        if (seKeyValues.ContainsKey(key))
        {
            seKeyValues.Remove(key);
        }
    }

    /// </summary>
    /// SE再生
    /// </summary>
    public void PlaySE(Define.SE key)
    {
        if (!seKeyValues.ContainsKey(key))
        {
            return;
        }

        AudioClip clip = seKeyValues[key];
        AudioSource source = seSources[currentSeSourceIndex];

        source.PlayOneShot(clip, seVolume * masterVolume);
        currentSeSourceIndex = Mathf.Clamp(currentSeSourceIndex + 1, 0, maxSeCount - 1);
    }

    /// <summary>
    /// SE全停止
    /// </summary>
    public void StopAllSE()
    {
        foreach(var source in seSources)
        {
            if(source != null)
            {
                source.Stop();
                Destroy(source);
            }
        }
        seSources.Clear();
    }
}
