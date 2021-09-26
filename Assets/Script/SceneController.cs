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

    // メインUI切替
    private UISwicther swicther;

    // 選択しているステージ
    private int currentSelectIndex = 0;
    private int stageNum = 3;
    
    // ステージ選択制御
    private SelectController selectController;

    // ステージ制御
    private StageController stageController;

    // シーン状態制御
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

        // 状態登録
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
    /// シーン遷移
    /// </summary>
    /// <param name="state"></param>
    private void TransScene(SCENE_STATE state)
    {
        stateMachine.ChangeState(state);
    }


    //===============================================================================================================
    //===↓状態定義↓================================================================================================
    //===============================================================================================================

    /// <summary>
    /// タイトル画面状態
    /// </summary>
    private class TitleState : State<SceneController>
    {
        public TitleState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.TITLE);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner,0.5f, ()=>owner.TransScene(SCENE_STATE.SELECT)));
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// 選択画面状態
    /// </summary>
    private class SelectState : State<SceneController>
    {
        //bool isFirstFrame = true;

        public SelectState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.SELECT);
            owner.currentSelectIndex = 0;

            // 選択枠の初期位置を設定（layoutgroupの計算後にしたいため1フレーム遅延）
            MonoBehaviourExtention.Delay(owner, 1, ()=>owner.selectController.FocusFrame(0));
        }

        /// <summary>
        /// 状態更新
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
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.selectController.FocusFrame(0);
        }
    }

    /// <summary>
    /// メイン画面状態
    /// </summary>
    private class MainState : State<SceneController>
    {
        public MainState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            MaskFadeController.Instance.FadeIn(1.0f);
            owner.swicther.switchUI(UISwicther.UI_INDEX.MAIN);
            owner.stageController.StageEntry();
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if(owner.stageController.Execute())
            {
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => owner.TransScene(SCENE_STATE.TITLE)));
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.stageController.StageExit();
        }
    }
}
