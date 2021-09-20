using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;
using System;
using DG.Tweening;

/// <summary>
/// マスク処理したImageでのフェードを制御するクラス
/// </summary>
public class MaskFadeController : SingletonMonoBehaviour<MaskFadeController>
{
    // 穴あき型にする画像
    [SerializeField]
    private Unmask mask;

    // マスクが画面外まで見えなくなるスケール
    [SerializeField]
    private Vector3 outScale;

    // フェードにかかる時間
    [SerializeField]
    private float duration = 1.0f;

    public enum FADE_STATE
    {
        IDLE,   // 待機
        FADEIN, // フェードイン
        FADEOUT, // フェードアウト
    }

    // 開始状態
    [SerializeField]
    private FADE_STATE startState = FADE_STATE.IDLE;

    // 状態制御
    private StateMachine<MaskFadeController, FADE_STATE> stateMachine = new StateMachine<MaskFadeController, FADE_STATE>();

    // フェード終了時処理
    private Action onComplete = null;

    // Start is called before the first frame update

    void Start()
    {
        // 状態登録
        stateMachine.AddState(FADE_STATE.IDLE, new IdleState(this));
        stateMachine.AddState(FADE_STATE.FADEIN, new FadeInState(this));
        stateMachine.AddState(FADE_STATE.FADEOUT, new FadeOutState(this));

        switch (startState)
        {
            case FADE_STATE.FADEOUT: mask.transform.localScale = Vector2.zero;break;
            case FADE_STATE.FADEIN: mask.transform.localScale = outScale; break;
        }

        stateMachine.ChangeState(FADE_STATE.IDLE);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    /// <summary>
    /// フェードイン開始
    /// </summary>
    /// <param name="time"></param>
    public void FadeIn(float time, Action action = null)
    {
        // フェード中は無効
        if (!stateMachine.IsCurrentState(FADE_STATE.IDLE))
        {
            return;
        }

        stateMachine.ChangeState(FADE_STATE.FADEIN);
        onComplete = action;
    }

    /// <summary>
    /// フェードアウト開始
    /// </summary>
    /// <param name="time"></param>
    public void FadeOut(float time, Action action = null)
    {
        // フェード中は無効
        if (!stateMachine.IsCurrentState(FADE_STATE.IDLE))
        {
            return;
        }

        stateMachine.ChangeState(FADE_STATE.FADEOUT);
        onComplete = action;
    }

    //===============================================================================================================
    //===↓状態定義↓================================================================================================
    //===============================================================================================================

    /// <summary>
    /// 待機状態
    /// </summary>
    private class IdleState : State<MaskFadeController>
    {
        public IdleState(MaskFadeController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// 待機状態
    /// </summary>
    private class FadeInState : State<MaskFadeController>
    {
        public FadeInState(MaskFadeController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            owner.mask.transform.localScale = Vector2.zero;
            var tween = owner.mask.transform.DOScale(owner.outScale, owner.duration);
            tween.OnComplete(() => {
                owner.stateMachine.ChangeState(FADE_STATE.IDLE);
            });
            tween.Play();
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 待機状態
    /// </summary>
    private class FadeOutState : State<MaskFadeController>
    {
        public FadeOutState(MaskFadeController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            owner.mask.transform.localScale = owner.outScale;
            var tween = owner.mask.transform.DOScale(Vector2.zero, owner.duration);
            tween.OnComplete(() => {
                owner.stateMachine.ChangeState(FADE_STATE.IDLE);
            });
            tween.Play();
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.onComplete?.Invoke();
        }
    }
}
