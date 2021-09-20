using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIExtensions;
using System;
using DG.Tweening;

/// <summary>
/// �}�X�N��������Image�ł̃t�F�[�h�𐧌䂷��N���X
/// </summary>
public class MaskFadeController : SingletonMonoBehaviour<MaskFadeController>
{
    // �������^�ɂ���摜
    [SerializeField]
    private Unmask mask;

    // �}�X�N����ʊO�܂Ō����Ȃ��Ȃ�X�P�[��
    [SerializeField]
    private Vector3 outScale;

    // �t�F�[�h�ɂ����鎞��
    [SerializeField]
    private float duration = 1.0f;

    public enum FADE_STATE
    {
        IDLE,   // �ҋ@
        FADEIN, // �t�F�[�h�C��
        FADEOUT, // �t�F�[�h�A�E�g
    }

    // �J�n���
    [SerializeField]
    private FADE_STATE startState = FADE_STATE.IDLE;

    // ��Ԑ���
    private StateMachine<MaskFadeController, FADE_STATE> stateMachine = new StateMachine<MaskFadeController, FADE_STATE>();

    // �t�F�[�h�I��������
    private Action onComplete = null;

    // Start is called before the first frame update

    void Start()
    {
        // ��ԓo�^
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
    /// �t�F�[�h�C���J�n
    /// </summary>
    /// <param name="time"></param>
    public void FadeIn(float time, Action action = null)
    {
        // �t�F�[�h���͖���
        if (!stateMachine.IsCurrentState(FADE_STATE.IDLE))
        {
            return;
        }

        stateMachine.ChangeState(FADE_STATE.FADEIN);
        onComplete = action;
    }

    /// <summary>
    /// �t�F�[�h�A�E�g�J�n
    /// </summary>
    /// <param name="time"></param>
    public void FadeOut(float time, Action action = null)
    {
        // �t�F�[�h���͖���
        if (!stateMachine.IsCurrentState(FADE_STATE.IDLE))
        {
            return;
        }

        stateMachine.ChangeState(FADE_STATE.FADEOUT);
        onComplete = action;
    }

    //===============================================================================================================
    //===����Ԓ�`��================================================================================================
    //===============================================================================================================

    /// <summary>
    /// �ҋ@���
    /// </summary>
    private class IdleState : State<MaskFadeController>
    {
        public IdleState(MaskFadeController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// �ҋ@���
    /// </summary>
    private class FadeInState : State<MaskFadeController>
    {
        public FadeInState(MaskFadeController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
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
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            owner.onComplete?.Invoke();
        }
    }

    /// <summary>
    /// �ҋ@���
    /// </summary>
    private class FadeOutState : State<MaskFadeController>
    {
        public FadeOutState(MaskFadeController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
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
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            owner.onComplete?.Invoke();
        }
    }
}
