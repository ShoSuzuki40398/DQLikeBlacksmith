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
    
    // �X�e�[�W�I�𐧌�
    private SelectController selectController;

    // �X�e�[�W����
    private StageController stageController;

    // �V�[����Ԑ���
    [SerializeField]
    private StateMachine<SceneController, SCENE_STATE> stateMachine = new StateMachine<SceneController, SCENE_STATE>();

    [SerializeField]
    private  SCENE_STATE initState = SCENE_STATE.TITLE;
    
    // Start is called before the first frame update
    void Start()
    {
        selectController = GetComponent<SelectController>();
        stageController = GetComponent<StageController>();
        swicther = GetComponent<UISwicther>();

        // ��ԓo�^
        stateMachine.AddState(SCENE_STATE.TITLE, new TitleState(this));
        stateMachine.AddState(SCENE_STATE.SELECT, new SelectState(this));
        stateMachine.AddState(SCENE_STATE.MAIN, new MainState(this));
        

        TransScene(initState);
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
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.TITLE);
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner,0.5f, ()=>owner.TransScene(SCENE_STATE.SELECT)));
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
        //bool isFirstFrame = true;

        public SelectState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
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
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                owner.currentSelectIndex = Mathf.Clamp(owner.currentSelectIndex - 1, 0, owner.stageNum - 1);
                owner.selectController.FocusFrame(owner.currentSelectIndex);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                owner.currentSelectIndex = Mathf.Clamp(owner.currentSelectIndex + 1, 0, owner.stageNum - 1);
                owner.selectController.FocusFrame(owner.currentSelectIndex);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                owner.stageController.SetPropertyAsset(owner.selectController.GetStageProperty(owner.currentSelectIndex));
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => owner.TransScene(SCENE_STATE.MAIN)));
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
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => owner.TransScene(SCENE_STATE.TITLE)));
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
