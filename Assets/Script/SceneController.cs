using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    private enum SCENE_STATE
    {
        TITLE,
        SELECT,
        MAIN
    }

    [SerializeField]
    private GameObject icon;

    // ���C��UI�ؑ�
    private UISwicther swicther;

    // �I�����Ă���X�e�[�W
    private int currentSelectIndex = 0;
    private int stageNum = 3;

    // �^�C�g������
    private TitleController titleController;
    
    // �X�e�[�W�I�𐧌�
    private SelectController selectController;

    // �X�e�[�W����
    private StageController stageController;

    // �V�[����Ԑ���
    [SerializeField]
    private StateMachine<SceneController, SCENE_STATE> stateMachine = new StateMachine<SceneController, SCENE_STATE>();

    // ���͉\�t���O
    private bool isInput = true;
    
    // Start is called before the first frame update
    void Start()
    {
        titleController = GetComponent<TitleController>();
        selectController = GetComponent<SelectController>();
        stageController = GetComponent<StageController>();
        swicther = GetComponent<UISwicther>();

        // ��ԓo�^
        stateMachine.AddState(SCENE_STATE.TITLE, new TitleState(this));
        stateMachine.AddState(SCENE_STATE.SELECT, new SelectState(this));
        stateMachine.AddState(SCENE_STATE.MAIN, new MainState(this));

        TransScene(SCENE_STATE.TITLE);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    /// <summary>
    /// �V�[���J��
    /// </summary>
    /// <param name="state"></param>
    private void TransScene(SCENE_STATE state)
    {
        stateMachine.ChangeState(state);
    }


    //===============================================================================================================
    //===����Ԓ�`��================================================================================================
    //===============================================================================================================

    /// <summary>
    /// �^�C�g����ʏ��
    /// </summary>
    private class TitleState : State<SceneController>
    {
        public TitleState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            owner.isInput = true;
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.TITLE);
            AudioManager.Instance.PlayBGM(Define.BGM.MAIN);
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            if(!owner.isInput)
            {
                return;
            }

            if (Define.InputEnterButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.HAMMER_HIT01);
                switch (owner.titleController.GetSelectIndex())
                {
                    case TitleController.TITLE_INDEX.START: // �Q�[���J�n
                        MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => 
                        
                        {
                            owner.titleController.ResetFrameColor();
                            owner.TransScene(SCENE_STATE.SELECT);
                        }));
                        owner.titleController.FlashingFrame();
                        owner.isInput = false;
                        break;
                    case TitleController.TITLE_INDEX.EXIT:  // �Q�[���I��
                        Define.EndGame();
                        break;
                }
            }
            else if(Define.InputUpButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                owner.titleController.FocusUp();
            }
            else if(Define.InputDownButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                owner.titleController.FocusDown();
            }

        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// �I����ʏ��
    /// </summary>
    private class SelectState : State<SceneController>
    {
        public SelectState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            owner.isInput = true;
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.SELECT);
            owner.currentSelectIndex = 0;

            // �I��g�̏����ʒu��ݒ�ilayoutgroup�̌v�Z��ɂ���������1�t���[���x���j
            MonoBehaviourExtention.Delay(owner, 1, ()=>owner.selectController.FocusFrame(0));
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            if (Define.InputLeftButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                owner.currentSelectIndex = Mathf.Clamp(owner.currentSelectIndex - 1, 0, owner.stageNum - 1);
                owner.selectController.FocusFrame(owner.currentSelectIndex);
            }
            else if (Define.InputRightButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                owner.currentSelectIndex = Mathf.Clamp(owner.currentSelectIndex + 1, 0, owner.stageNum - 1);
                owner.selectController.FocusFrame(owner.currentSelectIndex);
            }
            else if (Define.InputEnterButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.HAMMER_HIT01);
                owner.selectController.FlashingFrame();
                owner.stageController.SetPropertyAsset(owner.selectController.GetStageProperty(owner.currentSelectIndex));
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => owner.TransScene(SCENE_STATE.MAIN)));
            }
            else if(Define.InputBackButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.CANCEL_SOUND01);
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => owner.TransScene(SCENE_STATE.TITLE)));
            }

        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            owner.selectController.FocusFrame(0);
        }
    }

    /// <summary>
    /// ���C����ʏ��
    /// </summary>
    private class MainState : State<SceneController>
    {
        public MainState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.MAIN);
            owner.stageController.StageEntry();
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            if(owner.stageController.Execute())
            {
                owner.TransScene(SCENE_STATE.TITLE);
            }
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            owner.stageController.StageExit();
        }
    }
}
