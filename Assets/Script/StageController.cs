using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System;

public class StageController : MonoBehaviour
{
    // ステージ状態
    public enum STAGE_STATE
    {
        IDEL,   // 待機
        HIT,    // たたく
        SKILL,  // 特技
        CHECK,  // くわしく見る
        FINISH,  // しあげる
        STAGE_END // ステージ終了
    }

    // 製作物評価
    public enum DETAIL_STATUS
    {
        BAD,
        NORMAL,
        GOOD,
        GREAT
    }


    // ステージ状態制御
    public readonly StateMachine<StageController, STAGE_STATE> stateMachine = new StateMachine<StageController, STAGE_STATE>();



    /// -----------------------------------------------
    /// Leftパネル
    /// -----------------------------------------------
    // ガイド表示テキスト
    private const string GUIDE_DESCRIPTION_NEUTRAL = "どうする？";

    // ステージ情報を初期値とする進行中ステージ情報
    private StageProperty currentStageProperty;

    // ステージ情報の初期値
    private StageProperty initStageProperty;

    // ガイドテキスト
    [SerializeField]
    private Text guideText;

    // 温度テキスト
    [SerializeField]
    private Text heatLevelText;

    // コマンドテキスト
    [SerializeField]
    private GameObject commandTextParent;

    // コマンド説明テキスト
    [SerializeField]
    private Text commandDiscriptionText;

    // コマンド実行必要HP
    [SerializeField]
    private Text commandNeedHpText;

    // コマンドアセットリスト（後々動的に変更できるようにするかも）
    [SerializeField]
    private List<CommandPropertyAsset> commandPropertyAssets = new List<CommandPropertyAsset>();
    private List<CommandProperty> commandProperties = new List<CommandProperty>();

    // コマンドテキストプレハブ
    [SerializeField]
    private Text comamndTextPrefab;

    // 選択中のコマンドインデックス
    private int currentCommandIndex = 0;

    // コマンド選択枠
    [SerializeField]
    private UISquareFrame commandSelectFrame;
    // コマンドオブジェクト
    [SerializeField]
    private List<GameObject> selectIcons;


    /// -----------------------------------------------
    /// Centerパネル
    /// -----------------------------------------------

    // 製作物マスと進捗ゲージUI
    private struct CraftCell
    {
        public readonly ItemSliceImageCell cell;
        public readonly CraftGuage guage;

        public CraftCell(ItemSliceImageCell cell, CraftGuage guage)
        {
            this.cell = cell;
            this.guage = guage;
        }
    }

    // 製作物マスリスト
    private List<CraftCell> craftCells = new List<CraftCell>();
    
    // 製作物パネル
    [SerializeField]
    private GameObject itemPanel;


    [SerializeField]
    private GameObject guagePanel;

    //// ===== マス =====

    // 製作物オブジェクトのプレハブ
    [SerializeField]
    private GameObject itemSliceImagePrefab;

    // 製作物マス選択枠
    [SerializeField]
    private UISquareFrame cellSelectFrame;

    //// ===== 進捗ゲージ =====
    // 進捗ゲージオブジェクトのプレハブ
    [SerializeField]
    private GameObject guagePrefab;

    // ゲージが伸びるまでにかかる時間
    [SerializeField]
    private float changeTimeGuageValue = 1.0f;

    // ゲージ進捗時のエフェクト
    [SerializeField]
    private GameObject hitEffectPefab;

    /// -----------------------------------------------
    /// Footerパネル
    /// -----------------------------------------------

    // フッターUI
    [SerializeField]
    private FooterUI footer;

    // フッター情報
    [SerializeField]
    private FooterPropertyAsset footerPropertyAsset;
    private FooterProperty footerProperty;

    private void Start()
    {
        // 状態登録
        stateMachine.AddState(STAGE_STATE.IDEL, new IdleState(this));
        stateMachine.AddState(STAGE_STATE.HIT, new HitState(this));
        stateMachine.AddState(STAGE_STATE.SKILL, new SkillState(this));
        stateMachine.AddState(STAGE_STATE.CHECK, new CheckState(this));
        stateMachine.AddState(STAGE_STATE.FINISH, new FinishState(this));
        stateMachine.AddState(STAGE_STATE.STAGE_END, new StageEndState(this));

        footerProperty = footerPropertyAsset.footerProperty;
    }

    /// <summary>
    /// ステージ更新
    /// </summary>
    /// <returns>ステージ終了でtrue</returns>
    public bool Execute()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                Craft(1, 10);
            }
            else
            {
                Craft(0, 10);
            }
                       
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(ChangeGuageValue(0, -5));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Craft(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(ChangeGuageValue(0, -1));
        }

        if (stateMachine.IsCurrentState(STAGE_STATE.STAGE_END))
        {
            return true;
        }
        stateMachine.Update();

        return false;
    }

    public void TransState(STAGE_STATE state)
    {
        stateMachine.ChangeState(state);
    }

    /// <summary>
    /// ステージ情報設定
    /// </summary>
    public void SetPropertyAsset(StagePropertyAsset propertyAsset)
    {
        currentStageProperty = initStageProperty = propertyAsset.stageProperty;
        heatLevelText.text = MakeHeatLevelText(currentStageProperty.HeatLevel);
    }

    /// <summary>
    /// ステージ開始処理
    /// </summary>
    public void StageEntry()
    {
        guideText.text = GUIDE_DESCRIPTION_NEUTRAL;

        // コマンド作成
        CreateCommand();
        currentCommandIndex = 0;

        // 製作物作成
        CreateItem();

        // 選択枠非表示
        cellSelectFrame.gameObject.SetActive(false);

        // 待機状態に遷移
        TransState(STAGE_STATE.IDEL);
    }

    /// <summary>
    /// ステージ終了処理
    /// </summary>
    public void StageExit()
    {
        // コマンド削除
        selectIcons.Clear();
        commandTextParent.DestroyChildren();

        // 製作物削除
        craftCells.Clear();
        itemPanel.DestroyChildren();
        guagePanel.DestroyChildren();
    }

    /// <summary>
    /// コマンド選択肢作成
    /// </summary>
    private void CreateCommand()
    {
        foreach (var com in commandPropertyAssets)
        {
            Text comObj = Instantiate(comamndTextPrefab, commandTextParent.transform);
            commandProperties.Add(com.commandProperty);
            comObj.text = com.commandProperty.CommandStr;
            selectIcons.Add(comObj.gameObject);
        }

        // 説明テキストと消費体力テキスト作成
        if (commandProperties.Count > 0)
        {
            UpdateDiscStrAndNeedHpStr(commandProperties[0]);
        }

        // 選択枠の初期位置を設定（layoutgroupの計算後にしたいため1フレーム遅延）
        MonoBehaviourExtention.Delay(this, 1, () => { commandSelectFrame.transform.position = selectIcons[currentCommandIndex].transform.position; });
    }

    /// <summary>
    /// 製作物イメージ作成
    /// </summary>
    private void CreateItem()
    {
        if (currentStageProperty == null)
        {
            return;
        }

        // Imageオブジェクト作成
        // StagePropertyから製作物画像取得
        foreach (var property in currentStageProperty.ItemCellProperties)
        {
            // 製作物マス
            GameObject cellObj = Instantiate(itemSliceImagePrefab, itemPanel.transform);
            ItemSliceImageCell cell = cellObj.GetComponent<ItemSliceImageCell>();
            cell.itemImage.sprite = property.ItemSliceSprite;

            // 進捗ゲージ
            GameObject guageObj = Instantiate(guagePrefab, guagePanel.transform);
            CraftGuage guage = guageObj.GetComponent<CraftGuage>();

            // 進捗ゲージの理想値や成功エリアの値設定をする
            guage.SetSliderWidth(property.LimitValuef);
            guage.SetSliderMaxValue(property.LimitValuef);
            guage.SetSliderValue(0);
            guage.SetSuccessAreaWidth(property._SuccessArea.Width);
            guage.SetSliderColor(Define.craftGuageNormalColor);

            craftCells.Add(new CraftCell(cell, guage));
        }

        // ゲージ調整(位置、向き)
        MonoBehaviourExtention.Delay(this, 1, () =>
        {
            foreach (var itr in craftCells.Select((value,index)=> new { value,index}))
            {
                var property = currentStageProperty.ItemCellProperties[itr.index];
                var guage = itr.value.guage;

                var cellRect = itr.value.cell.GetComponent<RectTransform>();
                guage.transform.position = cellRect.position;

                float x = 0;
                if ((itr.index + 1) % 2 != 0)
                {   // マスの左側
                    x -= guage.GetComponent<RectTransform>().sizeDelta.x;
                    guage.SetDirection(Slider.Direction.RightToLeft);
                    guage.SetIdealPointAnchorRight();
                    guage.SetIdealPointX(-property.IdealValue);
                }
                else
                {   // マスの右側
                    x += guage.GetComponent<RectTransform>().sizeDelta.x;
                    guage.SetDirection(Slider.Direction.LeftToRight);
                    guage.SetIdealPointAnchorLeft();
                    guage.SetIdealPointX(property.IdealValue);
                }
                guage.transform.position += new Vector3(x, 0, 0);
            }
        });
    }

    /// <summary>
    /// 消費体力テキスト作成
    /// </summary>
    /// <param name="hp"></param>
    /// <returns></returns>
    private string MakeNeedHpText(int hp)
    {
        var result = new StringBuilder();
        result.Append(hp);
        result.Append("/");
        result.Append(initStageProperty.Hp);

        return result.ToString();
    }

    /// <summary>
    /// 消費体力テキスト作成
    /// </summary>
    /// <param name="hp"></param>
    /// <returns></returns>
    private string MakeHeatLevelText(int heatLevel)
    {
        var result = new StringBuilder();
        result.Append(heatLevel);
        result.Append("℃");

        return result.ToString();
    }

    /// <summary>
    /// 説明と消費体力テキストの更新
    /// </summary>
    /// <returns></returns>
    private void UpdateDiscStrAndNeedHpStr(CommandProperty property)
    {
        commandDiscriptionText.text = property.CommandDiscriptionStr;
        commandNeedHpText.text = MakeNeedHpText(property.NeedHp);
    }

    /// <summary>
    /// 製作を進める
    /// </summary>
    private void Craft(int cellIndex, float value)
    {
        CreateHitEffect(cellIndex,value);
        StartCoroutine(ChangeGuageValue(cellIndex,value));
    }

    /// <summary>
    /// 進捗エフェクト作成
    /// </summary>
    /// <param name="value"></param>
    private void CreateHitEffect(int cellIndex, float value)
    {
        var canvas = GameObject.Find("Canvas");
        var guage = craftCells[cellIndex].guage;
        Vector2 pos = guage.GetComponent<RectTransform>().position;
        GameObject obj = Instantiate(hitEffectPefab, pos, Quaternion.identity, canvas.transform);
        obj.GetComponent<HitEffect>().SetValue(value);
        obj.GetComponent<HitEffect>().Fire();
    }

    /// <summary>
    /// 進捗ゲージを伸ばす
    /// </summary>
    /// <param name="cellIndex">指定のセル</param>
    /// <param name="value">伸ばす数値（valueを加えた値になる）</param>
    /// <returns></returns>
    private IEnumerator ChangeGuageValue(int cellIndex,float value)
    {
        var property = currentStageProperty.ItemCellProperties[cellIndex];
        var guage = craftCells[cellIndex].guage;
        // 既に上限までいっていたら何もしない
        if (property.LimitValuef < guage.SliderValue)
        {
            yield break;
        }

        float time = changeTimeGuageValue;
        float startTime = Time.timeSinceLevelLoad;
        
        var startGuageValue = guage.SliderValue;
        Debug.Log("SliderValue :" + (guage.SliderValue + value));
        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;
            var rate = diff / time;
            guage.SliderValue = startGuageValue + (value * rate);

            // ゲージ変更終了
            if (diff > time)
            {
                guage.SliderValue = startGuageValue + value;

                // 色変え
                switch (guage.GetGuageStatus(property))
                {
                    case CraftGuage.GUAGE_STATUS.IDEAL: guage.SetSliderColor(Define.craftGuageIdealColor); break;
                    case CraftGuage.GUAGE_STATUS.SUCCESS: guage.SetSliderColor(Define.craftGuageSuccessColor); break;
                    case CraftGuage.GUAGE_STATUS.NORMAL: guage.SetSliderColor(Define.craftGuageNormalColor); break;
                }

                //// 色変え
                //float idealRangeValue = Define.idealRangeValue;
                //if (guage.SliderValue <= (property.IdealValue + idealRangeValue) && guage.SliderValue >= (property.IdealValue - idealRangeValue))
                //{   // 理想値範囲
                //    Debug.Log("理想値");
                //    guage.SetSliderColor(Define.craftGuageIdealColor);
                //}
                //else if (guage.SliderValue <= property._SuccessArea.max && guage.SliderValue >= property._SuccessArea.min)
                //{   // 成功値範囲
                //    Debug.Log("成功値");
                //    guage.SetSliderColor(Define.craftGuageSuccessColor);
                //}
                //else
                //{   // 普通範囲
                //    guage.SetSliderColor(Define.craftGuageNormalColor);
                //}
                
                break;
            }
            

            yield return null;
        }
    }

    //===============================================================================================================
    //===↓状態定義↓================================================================================================
    //===============================================================================================================

    /// <summary>
    /// 待機状態
    /// </summary>
    private class IdleState : State<StageController>
    {
        public IdleState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            Debug.Log("待機");
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CommandFocusUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CommandFocusDown();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (owner.commandProperties[owner.currentCommandIndex].Type)
                {
                    case CommandProperty.COMMAND_TYPE.HIT: owner.TransState(STAGE_STATE.HIT); break;
                    case CommandProperty.COMMAND_TYPE.SKILL: owner.TransState(STAGE_STATE.SKILL); break;
                    case CommandProperty.COMMAND_TYPE.CHECK: owner.TransState(STAGE_STATE.CHECK); break;
                    case CommandProperty.COMMAND_TYPE.FINISH: owner.TransState(STAGE_STATE.FINISH); break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                owner.TransState(STAGE_STATE.STAGE_END);
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// コマンドを上に移動
        /// </summary>
        public void CommandFocusUp()
        {
            owner.currentCommandIndex = Mathf.Clamp(owner.currentCommandIndex - 1, 0, owner.commandProperties.Count - 1);
            owner.commandSelectFrame.transform.position = owner.selectIcons[owner.currentCommandIndex].transform.position;
            owner.UpdateDiscStrAndNeedHpStr(owner.commandProperties[owner.currentCommandIndex]);
        }

        /// <summary>
        /// コマンドを下に移動
        /// </summary>
        public void CommandFocusDown()
        {
            owner.currentCommandIndex = Mathf.Clamp(owner.currentCommandIndex + 1, 0, owner.commandProperties.Count - 1);
            owner.commandSelectFrame.transform.position = owner.selectIcons[owner.currentCommandIndex].transform.position;
            owner.UpdateDiscStrAndNeedHpStr(owner.commandProperties[owner.currentCommandIndex]);
        }

    }


    /// <summary>
    /// たたく状態
    /// </summary>
    private class HitState : State<StageController>
    {
        // 選択中のマス
        private int currentFocusCellIndex = 0;

        public HitState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            Debug.Log("たたく");
            currentFocusCellIndex = 0;
            owner.cellSelectFrame.gameObject.SetActive(true);
            owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                owner.TransState(STAGE_STATE.IDEL);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CellFocusUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CellFocusDown();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CellFocusRight();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CellFocusLeft();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log(currentFocusCellIndex);
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.cellSelectFrame.gameObject.SetActive(false);
        }

        /// <summary>
        /// 選択セルを上に移動
        /// </summary>
        public void CellFocusUp()
        {
            int index = currentFocusCellIndex - 2;
            if (index.IsRange(0, owner.currentStageProperty.TotalCellCount - 1))
            {
                currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex - 2, 0, owner.currentStageProperty.TotalCellCount - 1);
                owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
            }
        }

        /// <summary>
        /// 選択セルを下に移動
        /// </summary>
        public void CellFocusDown()
        {
            int index = currentFocusCellIndex + 2;
            if (index.IsRange(0, owner.currentStageProperty.TotalCellCount - 1))
            {
                currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex + 2, 0, owner.currentStageProperty.TotalCellCount - 1);
                owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
            }
        }

        /// <summary>
        /// 選択セルを右に移動
        /// </summary>
        public void CellFocusRight()
        {
            int index = currentFocusCellIndex + 1;

            if (index.IsRange(0, owner.currentStageProperty.TotalCellCount - 1))
            {
                currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex + 1, 0, owner.currentStageProperty.TotalCellCount - 1);
                owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
            }
        }

        /// <summary>
        /// 選択セルを左に移動
        /// </summary>
        public void CellFocusLeft()
        {
            int index = currentFocusCellIndex - 1;


            if (index.IsRange(0, owner.currentStageProperty.TotalCellCount - 1))
            {
                currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex - 1, 0, owner.currentStageProperty.TotalCellCount - 1);
                owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
            }
        }
    }

    /// <summary>
    /// とくぎ状態
    /// </summary>
    private class SkillState : State<StageController>
    {
        public SkillState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            Debug.Log("とくぎ");
            owner.TransState(STAGE_STATE.IDEL);
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
    /// くわしく見る状態
    /// </summary>
    private class CheckState : State<StageController>
    {
        public CheckState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            Debug.Log("くわしく見る");

            int detailValue = CalcDetailValue();
            ShowDetail(detailValue);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if(Define.InputEnterButton() || Define.InputBackButton())
            {
                owner.TransState(STAGE_STATE.IDEL);
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            HideDetail();
        }

        /// <summary>
        /// 詳細表示
        /// </summary>
        private void ShowDetail(int detailValue)
        {
            var craftCells = owner.craftCells;
            foreach(var cell in craftCells)
            {
                cell.guage.ShowValue();
            }

            // 出来高表示
            switch (CheckDetail(detailValue))
            {
                case DETAIL_STATUS.BAD: owner.footer.SetFooterText(owner.footerProperty.DetailTextBad); break;
                case DETAIL_STATUS.NORMAL: owner.footer.SetFooterText(owner.footerProperty.DetailTextNormal); break;
                case DETAIL_STATUS.GOOD: owner.footer.SetFooterText(owner.footerProperty.DetailTextGood); break;
                case DETAIL_STATUS.GREAT: owner.footer.SetFooterText(owner.footerProperty.DetailTextGreat); break;
            }
        }

        /// <summary>
        /// 詳細非表示
        /// </summary>
        private void HideDetail()
        {
            var craftCells = owner.craftCells;
            foreach (var cell in craftCells)
            {
                cell.guage.HideValue();
            }

            owner.footer.ClearFooterText();
        }

        /// <summary>
        /// 評価値計算
        /// </summary>
        /// <returns></returns>
        private int CalcDetailValue()
        {
            int res = 0;
            var stageProperty = owner.currentStageProperty;
            foreach (var itr in owner.craftCells.Select((value, index) => new { value, index }))
            {
                var property = stageProperty.ItemCellProperties[itr.index];
                var guage = itr.value.guage;

                switch (guage.GetGuageStatus(property))
                {
                    case CraftGuage.GUAGE_STATUS.IDEAL: res += Define.idealDetailValue; break;
                    case CraftGuage.GUAGE_STATUS.SUCCESS: res += Define.successDetailValue; break;
                    case CraftGuage.GUAGE_STATUS.NORMAL: res += Define.normalDetailValue; break;
                }
            }

            return res;
        }

        /// <summary>
        /// 評価値チェック
        /// </summary>
        /// <param name="value">各ゲージの合算評価値</param>
        /// <returns></returns>
        private DETAIL_STATUS CheckDetail(int value)
        {
            DETAIL_STATUS res = DETAIL_STATUS.BAD;
            var stageProperty = owner.currentStageProperty;

            if(value >= stageProperty.GreatDetailValue)
            {
                res = DETAIL_STATUS.GREAT;
            }
            else if(value >= stageProperty.GoodDetailValue)
            {
                res = DETAIL_STATUS.GOOD;
            }
            else if(value >= stageProperty.NormalDetailValue)
            {
                res = DETAIL_STATUS.NORMAL;
            }

            return res;
        }


    }

    /// <summary>
    /// しあげる状態
    /// </summary>
    private class FinishState : State<StageController>
    {
        public FinishState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            Debug.Log("しあげる");
            owner.TransState(STAGE_STATE.IDEL);
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
    /// ステージ終了状態
    /// </summary>
    private class StageEndState : State<StageController>
    {
        public StageEndState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            Debug.Log("終わり");
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
}
