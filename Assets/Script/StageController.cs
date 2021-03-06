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

    // スキル制御
    private SkillManager skillManager;

    // 入力可能フラグ
    private bool isInput = true;

    // ステージ選択制御
    private SelectController selectController;

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
    private List<GameObject> commandSelectObjects;


    //// ===== スキル =====

    //　スキルコマンドオブジェクトの親
    [SerializeField]
    private GameObject skillCommandParent;

    // スキルコマンドオブジェクト
    [SerializeField]
    private List<GameObject> skillSelectObjects;

    // コマンド選択枠
    [SerializeField]
    private UISquareFrame skillSelectFrame;

    // 選択中のスキルコマンドインデックス
    private int currentSkillIndex = 0;

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

    // 温度テキスト
    [SerializeField]
    private Text heatLevelText;

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

    // 製作物マス選択枠(予備)
    [SerializeField]
    private UISquareFrame cellSelectFrameOther;


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

    /// -----------------------------------------------
    /// リザルト
    /// -----------------------------------------------
    [SerializeField]
    private ResultView resultView;

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

        cellSelectFrameOther.gameObject.SetActive(false);

        skillManager = GetComponent<SkillManager>();
        selectController = GetComponent<SelectController>();
    }

    /// <summary>
    /// ステージ更新
    /// </summary>
    /// <returns>ステージ終了でtrue</returns>
    public bool Execute()
    {
        if (stateMachine.IsCurrentState(STAGE_STATE.STAGE_END))
        {
            return true;
        }
        stateMachine.Update();

        return false;
    }

    /// <summary>
    /// 状態遷移
    /// </summary>
    /// <param name="state"></param>
    public void TransState(STAGE_STATE state)
    {
        stateMachine.ChangeState(state);
    }

    /// <summary>
    /// ステージ情報設定
    /// </summary>
    public void SetPropertyAsset(StagePropertyAsset propertyAsset)
    {
        currentStageProperty = initStageProperty = propertyAsset.stageProperty.Clone();
        heatLevelText.text = MakeHeatLevelText(currentStageProperty.HeatLevel);
    }

    /// <summary>
    /// ステージ開始処理
    /// </summary>
    public void StageEntry()
    {
        guideText.text = GUIDE_DESCRIPTION_NEUTRAL;

        // コマンド作成
        currentCommandIndex = 0;
        CreateCommand();

        // スキル作成
        currentSkillIndex = 0;
        CreateSkillCommand();

        // 製作物作成
        CreateItem();

        // 選択枠非表示
        cellSelectFrame.gameObject.SetActive(false);
        cellSelectFrameOther.gameObject.SetActive(false);

        resultView.Reset();

        // 待機状態に遷移
        TransState(STAGE_STATE.IDEL);
    }

    /// <summary>
    /// ステージ終了処理
    /// </summary>
    public void StageExit()
    {
        // コマンド削除
        commandSelectObjects.Clear();
        commandTextParent.DestroyChildren();

        // スキル削除
        skillSelectObjects.Clear();
        skillCommandParent.DestroyChildren();

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
            commandSelectObjects.Add(comObj.gameObject);
        }

        // 説明テキストと消費体力テキスト作成
        if (commandProperties.Count > 0)
        {
            UpdateDiscStrAndNeedHpStr(commandProperties[0]);
        }

        // 選択枠の初期位置を設定（layoutgroupの計算後にしたいため1フレーム遅延）
        MonoBehaviourExtention.Delay(this, 1, () => { commandSelectFrame.transform.position = commandSelectObjects[currentCommandIndex].transform.position; });
    }

    /// <summary>
    /// スキルコマンド作成
    /// </summary>
    public void CreateSkillCommand()
    {
        // 剣の時は「上下打ち」抜きのスキル一覧を取得
        if (currentStageProperty.StageIndex == 1)
        {
            foreach (var property in skillManager.SkillPropertiesWithoutLongHit)
            {
                Text obj = Instantiate(comamndTextPrefab, skillCommandParent.transform);
                obj.text = property.SkillStr;
                skillSelectObjects.Add(obj.gameObject);
            }
        }
        else
        {
            foreach (var property in skillManager.SkillProperties)
            {
                Text obj = Instantiate(comamndTextPrefab, skillCommandParent.transform);
                obj.text = property.SkillStr;
                skillSelectObjects.Add(obj.gameObject);
            }
        }


        // 選択枠の初期位置を設定（layoutgroupの計算後にしたいため1フレーム遅延）
        MonoBehaviourExtention.Delay(this, 1, () =>
        {
            skillSelectFrame.transform.position = skillSelectObjects[currentSkillIndex].transform.position;
            skillCommandParent.SetActive(false);
            skillSelectFrame.gameObject.SetActive(false);
        });
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
            cell.Initialized();

            // 進捗ゲージ
            GameObject guageObj = Instantiate(guagePrefab, guagePanel.transform);
            CraftGuage guage = guageObj.GetComponent<CraftGuage>();

            // 進捗ゲージの理想値や成功エリアの値設定をする
            guage.SetSliderWidth(property.LimitValuef);
            guage.SetSliderMaxValue(property.LimitValuef);
            guage.SetSliderValue(0);
            guage.SetSuccessAreaWidth(property._SuccessArea.Width);
            guage.SetSliderColor(Define.craftGuageNormalColor);
            guage.LightOffIdealPoint();

            craftCells.Add(new CraftCell(cell, guage));
        }

        // ゲージ調整(位置、向き)
        MonoBehaviourExtention.Delay(this, 1, () =>
        {
            foreach (var itr in craftCells.Select((value, index) => new { value, index }))
            {
                var property = currentStageProperty.ItemCellProperties[itr.index];
                var guage = itr.value.guage;

                var cellRect = itr.value.cell.GetComponent<RectTransform>();
                guage.transform.position = cellRect.position;

                float x = 0;
                if ((itr.index + 1) % 2 != 0)
                {   // マスの左側
                    x -= guage.GetComponent<RectTransform>().sizeDelta.x + 50.0f;
                    guage.SetDirection(Slider.Direction.RightToLeft);
                    guage.SetIdealPointAnchorRight();
                    guage.SetIdealPointX(-property.IdealValue);
                }
                else
                {   // マスの右側
                    x += guage.GetComponent<RectTransform>().sizeDelta.x + 50.0f;
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
        result.Append(currentStageProperty.Hp);

        return result.ToString();
    }

    /// <summary>
    /// 温度レベルテキスト作成
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
    /// 温度レベルチェック
    /// 0℃を下回る場合true
    /// </summary>
    /// <returns></returns>
    private bool IsOverHeatLevel(int value)
    {
        int heatLevel = currentStageProperty.HeatLevel + value;
        if (heatLevel < 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 温度レベル変更処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecChangeHeatLevel(int value, Action onComplete = null)
    {
        var property = currentStageProperty;
        // 既に下限までいっていたら何もしない
        if (IsOverHeatLevel(value))
        {
            onComplete?.Invoke();
            yield break;
        }
        float time = changeTimeGuageValue;
        float startTime = Time.timeSinceLevelLoad;

        var startValue = property.HeatLevel;

        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;
            var rate = diff / time;
            property.HeatLevel = (int)(startValue + (value * rate));
            heatLevelText.text = MakeHeatLevelText(property.HeatLevel);

            // 変更終了
            if (diff > time)
            {
                property.HeatLevel = startValue + value;
                heatLevelText.text = MakeHeatLevelText(property.HeatLevel);
                onComplete?.Invoke();
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 説明と消費体力テキストの更新(コマンド)
    /// </summary>
    /// <returns></returns>
    private void UpdateDiscStrAndNeedHpStr(CommandProperty property)
    {
        commandDiscriptionText.text = property.CommandDiscriptionStr;
        commandNeedHpText.text = MakeNeedHpText(property.NeedHp);
    }

    /// <summary>
    /// 説明と消費体力テキストの更新（スキル）
    /// </summary>
    /// <returns></returns>
    private void UpdateDiscStrAndNeedHpStr(SkillProperty property)
    {
        commandDiscriptionText.text = property.SkillDiscriptionStr;
        commandNeedHpText.text = MakeNeedHpText(property.NeedHp);
    }


    /// <summary>
    /// 製作(マス選択あり)
    /// </summary>
    /// <param name="cellIndex">対象マス</param>
    /// <param name="skillProperty">使用スキル</param>
    /// <param name="onComplete">終了時処理</param>
    private void Craft(int cellIndex, SkillProperty skillProperty, Action onComplete = null)
    {
        // 製作可能か確認
        if (skillProperty.NeedHeatLevel > currentStageProperty.HeatLevel)
        {
            footer.SetFooterText(footerProperty.NoHeatLevel);
            onComplete?.Invoke();
            return;
        }

        // 体力消費
        if (skillProperty.NeedHp > currentStageProperty.Hp)
        {
            footer.SetFooterText(footerProperty.NoHp);
            onComplete?.Invoke();
            return;
        }
        ChangeHp(skillProperty.NeedHp);

        // 温度消費
        StartCoroutine(ExecChangeHeatLevel(-skillProperty.NeedHeatLevel));

        // 進捗値更新
        StartCoroutine(ExecCraft(cellIndex, skillProperty, onComplete));
    }

    /// <summary>
    /// ランダム製作
    /// </summary>
    /// <param name="skillProperty">使用スキル</param>
    /// <param name="onComplete">終了時処理</param>
    private void Craft(SkillProperty skillProperty, Action onComplete = null)
    {
        // 製作可能か確認
        if (skillProperty.NeedHeatLevel > currentStageProperty.HeatLevel)
        {
            footer.SetFooterText(footerProperty.NoHeatLevel);
            onComplete?.Invoke();
            return;
        }

        // 体力消費
        if (skillProperty.NeedHp > currentStageProperty.Hp)
        {
            footer.SetFooterText(footerProperty.NoHp);
            onComplete?.Invoke();
            return;
        }

        ChangeHp(skillProperty.NeedHp);

        // 温度消費
        StartCoroutine(ExecChangeHeatLevel(-skillProperty.NeedHeatLevel));

        // 進捗値更新
        StartCoroutine(ExecRandomCraft(skillProperty, onComplete));
    }

    /// <summary>
    /// 製作(複数マス選択)
    /// </summary>
    /// <param name="cellIndex">対象マス</param>
    /// <param name="skillProperty">使用スキル</param>
    /// <param name="onComplete">終了時処理</param>
    private void Craft(int[] cellIndicies, SkillProperty skillProperty, Action onComplete = null)
    {
        // 製作可能か確認
        if (skillProperty.NeedHeatLevel > currentStageProperty.HeatLevel)
        {
            footer.SetFooterText(footerProperty.NoHeatLevel);
            onComplete?.Invoke();
            return;
        }

        // 体力消費
        if (skillProperty.NeedHp > currentStageProperty.Hp)
        {
            footer.SetFooterText(footerProperty.NoHp);
            onComplete?.Invoke();
            return;
        }
        ChangeHp(skillProperty.NeedHp);

        // 温度消費
        StartCoroutine(ExecChangeHeatLevel(-skillProperty.NeedHeatLevel));

        // 進捗値更新
        StartCoroutine(ExecMultiCraft(cellIndicies, skillProperty, onComplete));
    }

    /// <summary>
    /// 温度変更
    /// </summary>
    /// <param name="skillProperty">使用スキル</param>
    /// <param name="onComplete">終了時処理</param>
    private void HeatAdjust(SkillProperty skillProperty, Action onComplete = null)
    {
        // 製作可能か確認
        if (skillProperty.NeedHeatLevel > currentStageProperty.HeatLevel)
        {
            footer.SetFooterText(footerProperty.NoHeatLevel);
            onComplete?.Invoke();
            return;
        }

        if (0 >= currentStageProperty.HeatLevel + skillProperty.Value)
        {
            footer.SetFooterText(footerProperty.NoCoolDown);
            return;
        }


        // 体力消費
        if (skillProperty.NeedHp > currentStageProperty.Hp)
        {
            footer.SetFooterText(footerProperty.NoHp);
            onComplete?.Invoke();
            return;
        }

        // 音再生
        if (skillProperty.Type == SkillProperty.SKILL_TYPE.HEAT_UP)
        {
            AudioManager.Instance.PlaySE(Define.SE.HEATUP01);
        }
        else if (skillProperty.Type == SkillProperty.SKILL_TYPE.COOL_DOWN)
        {
            AudioManager.Instance.PlaySE(Define.SE.COOLDOWN01);
        }

        ChangeHp(skillProperty.NeedHp);

        // 進捗値更新
        StartCoroutine(ExecChangeHeatLevel(skillProperty.Value, onComplete));
    }

    /// <summary>
    /// 体力消費処理
    /// </summary>
    private void ChangeHp(int value)
    {
        currentStageProperty.Hp -= value;
    }

    /// <summary>
    /// 製作処理
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private IEnumerator ExecCraft(int cellIndex, SkillProperty property, Action onComplete = null)
    {
        isInput = false;
        for (int i = 0; i < property.Count; ++i)
        {
            int craftValue = CalcCraftValue(property.ValueWithWeight);
            CreateHitEffect(cellIndex, craftValue);
            StartCoroutine(ChangeGuageValue(cellIndex, craftValue));

            yield return new WaitForSeconds(changeTimeGuageValue + 0.1f);
        }
        onComplete?.Invoke();
        isInput = true;
    }

    /// <summary>
    /// ランダム製作処理
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private IEnumerator ExecRandomCraft(SkillProperty property, Action onComplete = null)
    {
        isInput = false;

        int cellNum = currentStageProperty.TotalCellCount;

        for (int i = 0; i < property.Count; ++i)
        {
            int cellIndex = UnityEngine.Random.Range(0, cellNum);

            int craftValue = CalcCraftValue(property.ValueWithWeight);
            CreateHitEffect(cellIndex, craftValue);
            StartCoroutine(ChangeGuageValue(cellIndex, craftValue));

            yield return new WaitForSeconds(changeTimeGuageValue + 0.1f);
        }
        onComplete?.Invoke();
        isInput = true;
    }

    /// <summary>
    /// 製作処理
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private IEnumerator ExecMultiCraft(int[] cellIndicies, SkillProperty property, Action onComplete = null)
    {
        isInput = false;
        for (int i = 0; i < property.Count; ++i)
        {
            for (int j = 0; j < cellIndicies.Length; ++j)
            {
                int craftValue = CalcCraftValue(property.ValueWithWeight);
                CreateHitEffect(cellIndicies[j], craftValue);
                StartCoroutine(ChangeGuageValue(cellIndicies[j], craftValue));
            }

            yield return new WaitForSeconds(changeTimeGuageValue + 0.1f);
        }
        onComplete?.Invoke();
        isInput = true;
    }

    /// <summary>
    /// 進捗値計算
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int CalcCraftValue(int value)
    {
        // 温度レベルによって進捗値に重みをかける
        float heatLevelWeight = currentStageProperty.HeatLevel * 0.0012f;
        return (int)(value * heatLevelWeight);
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
        AudioManager.Instance.PlaySE(Define.SE.HAMMER_HIT01);
    }

    /// <summary>
    /// 進捗ゲージを伸ばす
    /// </summary>
    /// <param name="cellIndex">指定のセル</param>
    /// <param name="value">伸ばす数値（valueを加えた値になる）</param>
    /// <returns></returns>
    private IEnumerator ChangeGuageValue(int cellIndex, float value)
    {
        var property = currentStageProperty.ItemCellProperties[cellIndex];
        var guage = craftCells[cellIndex].guage;
        var cell = craftCells[cellIndex].cell;
        // 既に上限までいっていたら何もしない
        if (property.LimitValuef < guage.SliderValue)
        {
            yield break;
        }

        float time = changeTimeGuageValue;
        float startTime = Time.timeSinceLevelLoad;

        var startGuageValue = guage.SliderValue;

        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;
            var rate = diff / time;
            guage.SliderValue = startGuageValue + (value * rate);

            // ゲージ変更終了
            if (diff > time)
            {
                guage.SliderValue = startGuageValue + value;

                // ゲージ色変え
                switch (guage.GetGuageStatus(property))
                {
                    case CraftGuage.GUAGE_STATUS.IDEAL: AudioManager.Instance.PlaySE(Define.SE.ACCENT01); guage.SetSliderColor(Define.craftGuageIdealColor); guage.LightOnIdealPoint(); break;
                    case CraftGuage.GUAGE_STATUS.SUCCESS: guage.SetSliderColor(Define.craftGuageSuccessColor); guage.LightOffIdealPoint(); break;
                    case CraftGuage.GUAGE_STATUS.NORMAL: guage.SetSliderColor(Define.craftGuageNormalColor); guage.LightOffIdealPoint(); break;
                }

                // 製作物色変え
                float ratio = guage.SliderValue / property.IdealValue;
                cell.ChangeMaskColorFromGuageRatio(ratio);

                yield break;
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
            owner.isInput = true;
            owner.UpdateDiscStrAndNeedHpStr(owner.commandProperties[owner.currentCommandIndex]);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Define.InputUpButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                CommandFocusUp();
            }
            else if (Define.InputDownButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                CommandFocusDown();
            }

            if (Define.InputEnterButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.DECISION_SOUND01);
                switch (owner.commandProperties[owner.currentCommandIndex].Type)
                {
                    case CommandProperty.COMMAND_TYPE.HIT: owner.TransState(STAGE_STATE.HIT); break;
                    case CommandProperty.COMMAND_TYPE.SKILL: owner.TransState(STAGE_STATE.SKILL); break;
                    case CommandProperty.COMMAND_TYPE.CHECK: owner.TransState(STAGE_STATE.CHECK); break;
                    case CommandProperty.COMMAND_TYPE.FINISH: owner.TransState(STAGE_STATE.FINISH); break;
                }
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.footer.ClearFooterText();
        }

        /// <summary>
        /// コマンドを上に移動
        /// </summary>
        public void CommandFocusUp()
        {
            owner.currentCommandIndex = Mathf.Clamp(owner.currentCommandIndex - 1, 0, owner.commandProperties.Count - 1);
            owner.commandSelectFrame.transform.position = owner.commandSelectObjects[owner.currentCommandIndex].transform.position;
            owner.UpdateDiscStrAndNeedHpStr(owner.commandProperties[owner.currentCommandIndex]);
        }

        /// <summary>
        /// コマンドを下に移動
        /// </summary>
        public void CommandFocusDown()
        {
            owner.currentCommandIndex = Mathf.Clamp(owner.currentCommandIndex + 1, 0, owner.commandProperties.Count - 1);
            owner.commandSelectFrame.transform.position = owner.commandSelectObjects[owner.currentCommandIndex].transform.position;
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
            currentFocusCellIndex = 0;
            owner.cellSelectFrame.gameObject.SetActive(true);
            owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Define.InputEnterButton())
            {
                // skillmanagerから「たたく」スキル取得
                owner.Craft(currentFocusCellIndex, owner.skillManager.StandardHit, () => owner.TransState(STAGE_STATE.IDEL));
            }
            else if (Define.InputUpButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                CellFocusUp();
            }
            else if (Define.InputDownButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                CellFocusDown();
            }
            else if (Define.InputRightButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                CellFocusRight();
            }
            else if (Define.InputLeftButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                CellFocusLeft();
            }
            else if (Define.InputBackButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.CANCEL_SOUND01);
                owner.TransState(STAGE_STATE.IDEL);
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
        enum SKILL_STATE
        {
            COMMAND,    // 特技選択
            CELL,       // マス選択
            MULTI,      // 複数マス選択
        }


        private StateMachine<SkillState, SKILL_STATE> stateMachine = new StateMachine<SkillState, SKILL_STATE>();

        public SkillState(StageController owner) : base(owner)
        {
            stateMachine.AddState(SKILL_STATE.COMMAND, new CommandState(this));
            stateMachine.AddState(SKILL_STATE.CELL, new CellState(this));
            stateMachine.AddState(SKILL_STATE.MULTI, new MultiCellState(this));
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            owner.currentSkillIndex = 0;
            owner.skillCommandParent.SetActive(true);
            owner.skillSelectFrame.gameObject.SetActive(true);
            owner.skillSelectFrame.transform.position = owner.skillSelectObjects[0].transform.position;
            owner.footer.SetFooterText(owner.skillManager.GetSkillDiscription(0));
            owner.UpdateDiscStrAndNeedHpStr(owner.skillManager.GetProperty(0));
            stateMachine.ChangeState(SKILL_STATE.COMMAND);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            stateMachine.Update();
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            owner.skillCommandParent.SetActive(false);
            owner.skillSelectFrame.gameObject.SetActive(false);
            owner.cellSelectFrame.gameObject.SetActive(false);
            owner.cellSelectFrameOther.gameObject.SetActive(false);
            owner.footer.ClearFooterText();
        }

        /// <summary>
        /// 特技コマンド選択
        /// </summary>
        private class CommandState : State<SkillState>
        {
            private StageController stageController;

            public CommandState(SkillState owner) : base(owner)
            {
                stageController = owner.owner;
            }

            public override void Enter()
            {
                MonoBehaviourExtention.Delay(stageController, 1, () => stageController.skillSelectFrame.transform.position = stageController.skillSelectObjects[stageController.currentSkillIndex].transform.position);
            }

            public override void Execute()
            {
                if (!stageController.isInput)
                {
                    return;
                }

                if (Define.InputEnterButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.DECISION_SOUND01);
                    // ステージ1の場合
                    if (stageController.currentStageProperty.StageIndex == 1)
                    {
                        var skill = stageController.skillManager.GetPropertyWithoutLongHit(stageController.currentSkillIndex);
                        ExecSkill(skill);
                    }
                    else
                    {
                        var skill = stageController.skillManager.GetProperty(stageController.currentSkillIndex);
                        ExecSkill(skill);
                    }
                        
                }
                else if (Define.InputUpButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CommandFocusUp();
                }
                else if (Define.InputDownButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CommandFocusDown();
                }
                else if (Define.InputBackButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.CANCEL_SOUND01);
                    stageController.TransState(STAGE_STATE.IDEL);
                }
            }
            public override void Exit()
            {
            }

            /// <summary>
            /// スキル実行
            /// </summary>
            /// <param name="type"></param>
            private void ExecSkill(SkillProperty skill)
            {
                switch (skill.Etype)
                {
                    case SkillProperty.SKILL_EXEC_TYPE.NORMAL:
                        // マス選択に移動
                        owner.stateMachine.ChangeState(SKILL_STATE.CELL);
                        break;
                    case SkillProperty.SKILL_EXEC_TYPE.RANDOM:
                        // マス選択せずにスキル実行
                        // skillmanagerからスキル取得
                        stageController.Craft(skill, () => stageController.TransState(STAGE_STATE.IDEL));
                        break;
                    case SkillProperty.SKILL_EXEC_TYPE.MULTI:
                        // 複数マス選択に移動
                        owner.stateMachine.ChangeState(SKILL_STATE.MULTI);
                        break;
                    case SkillProperty.SKILL_EXEC_TYPE.HEAT_LEVEL:
                        stageController.HeatAdjust(skill, () => stageController.TransState(STAGE_STATE.IDEL));
                        break;
                }
            }

            /// <summary>
            /// コマンドを上に移動
            /// </summary>
            public void CommandFocusUp()
            {
                stageController.currentSkillIndex = Mathf.Clamp(stageController.currentSkillIndex - 1, 0, stageController.skillSelectObjects.Count - 1);
                stageController.skillSelectFrame.transform.position = stageController.skillSelectObjects[stageController.currentSkillIndex].transform.position;

                // ステージ1の場合
                if (stageController.currentStageProperty.StageIndex == 1)
                {
                    stageController.footer.SetFooterText(stageController.skillManager.GetSkillDiscriptionWithoutLongHit(stageController.currentSkillIndex));
                    stageController.UpdateDiscStrAndNeedHpStr(stageController.skillManager.GetPropertyWithoutLongHit(stageController.currentSkillIndex));
                }
                else
                {
                    stageController.footer.SetFooterText(stageController.skillManager.GetSkillDiscription(stageController.currentSkillIndex));
                    stageController.UpdateDiscStrAndNeedHpStr(stageController.skillManager.GetProperty(stageController.currentSkillIndex));
                }
            }

            /// <summary>
            /// コマンドを下に移動
            /// </summary>
            public void CommandFocusDown()
            {
                stageController.currentSkillIndex = Mathf.Clamp(stageController.currentSkillIndex + 1, 0, stageController.skillSelectObjects.Count - 1);
                stageController.skillSelectFrame.transform.position = stageController.skillSelectObjects[stageController.currentSkillIndex].transform.position;

                // ステージ1の場合
                if (stageController.currentStageProperty.StageIndex == 1)
                {
                    stageController.footer.SetFooterText(stageController.skillManager.GetSkillDiscriptionWithoutLongHit(stageController.currentSkillIndex));
                    stageController.UpdateDiscStrAndNeedHpStr(stageController.skillManager.GetPropertyWithoutLongHit(stageController.currentSkillIndex));
                }
                else
                {
                    stageController.footer.SetFooterText(stageController.skillManager.GetSkillDiscription(stageController.currentSkillIndex));
                    stageController.UpdateDiscStrAndNeedHpStr(stageController.skillManager.GetProperty(stageController.currentSkillIndex));
                }
            }

        }

        /// <summary>
        /// マス選択
        /// </summary>
        private class CellState : State<SkillState>
        {
            private StageController stageController;

            // 選択中のマス
            private int currentFocusCellIndex = 0;

            public CellState(SkillState owner) : base(owner)
            {
                stageController = owner.owner;
            }

            public override void Enter()
            {
                currentFocusCellIndex = 0;
                stageController.cellSelectFrame.gameObject.SetActive(true);
                stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
            }
            public override void Execute()
            {
                if (Define.InputEnterButton())
                {
                    // スキル実行処理
                    // skillmanagerからスキル取得
                    stageController.Craft(currentFocusCellIndex, stageController.skillManager.GetProperty(stageController.currentSkillIndex), () => stageController.TransState(STAGE_STATE.IDEL));
                }
                else if (Define.InputUpButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusUp();
                }
                else if (Define.InputDownButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusDown();
                }
                else if (Define.InputRightButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusRight();
                }
                else if (Define.InputLeftButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusLeft();
                }
                if (Define.InputBackButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.CANCEL_SOUND01);
                    owner.stateMachine.ChangeState(SKILL_STATE.COMMAND);
                }
            }
            public override void Exit()
            {
                stageController.cellSelectFrame.gameObject.SetActive(false);
            }

            /// <summary>
            /// 選択セルを上に移動
            /// </summary>
            public void CellFocusUp()
            {
                int index = currentFocusCellIndex - 2;
                if (index.IsRange(0, stageController.currentStageProperty.TotalCellCount - 1))
                {
                    currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex - 2, 0, stageController.currentStageProperty.TotalCellCount - 1);
                    stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
                }
            }

            /// <summary>
            /// 選択セルを下に移動
            /// </summary>
            public void CellFocusDown()
            {
                int index = currentFocusCellIndex + 2;
                if (index.IsRange(0, stageController.currentStageProperty.TotalCellCount - 1))
                {
                    currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex + 2, 0, stageController.currentStageProperty.TotalCellCount - 1);
                    stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
                }
            }

            /// <summary>
            /// 選択セルを右に移動
            /// </summary>
            public void CellFocusRight()
            {
                int index = currentFocusCellIndex + 1;

                if (index.IsRange(0, stageController.currentStageProperty.TotalCellCount - 1))
                {
                    currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex + 1, 0, stageController.currentStageProperty.TotalCellCount - 1);
                    stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
                }
            }

            /// <summary>
            /// 選択セルを左に移動
            /// </summary>
            public void CellFocusLeft()
            {
                int index = currentFocusCellIndex - 1;


                if (index.IsRange(0, stageController.currentStageProperty.TotalCellCount - 1))
                {
                    currentFocusCellIndex = Mathf.Clamp(currentFocusCellIndex - 1, 0, stageController.currentStageProperty.TotalCellCount - 1);
                    stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
                }
            }
        }

        /// <summary>
        /// 複数マス選択
        /// </summary>
        private class MultiCellState : State<SkillState>
        {
            private StageController stageController;

            // 選択中のマス
            private int currentFocusCellIndex = 0;
            private int otherCellIndex = 0;

            private SkillProperty.SKILL_TYPE type;

            public MultiCellState(SkillState owner) : base(owner)
            {
                stageController = owner.owner;
            }

            public override void Enter()
            {
                currentFocusCellIndex = 0;
                stageController.cellSelectFrame.gameObject.SetActive(true);
                stageController.cellSelectFrameOther.gameObject.SetActive(true);
                stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
                type = stageController.skillManager.GetProperty(stageController.currentSkillIndex).Type;


                // 範囲によって予備選択枠の位置を変える
                switch (type)
                {
                    case SkillProperty.SKILL_TYPE.WIDE_HIT: otherCellIndex = 1; break;
                    case SkillProperty.SKILL_TYPE.LONG_HIT: otherCellIndex = 2; break;
                    default: break;
                }
                OtherCellFocus(otherCellIndex);
            }

            public override void Execute()
            {
                if (Define.InputEnterButton())
                {
                    // スキル実行処理
                    // skillmanagerからスキル取得
                    int[] cellIndicies = { currentFocusCellIndex, otherCellIndex };
                    stageController.Craft(cellIndicies, stageController.skillManager.GetProperty(stageController.currentSkillIndex), () => stageController.TransState(STAGE_STATE.IDEL));
                }
                else if (Define.InputUpButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusUp();
                }
                else if (Define.InputDownButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusDown();
                }
                else if (Define.InputLeftButton() && type == SkillProperty.SKILL_TYPE.LONG_HIT)
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusLeft();
                }
                else if (Define.InputRightButton() && type == SkillProperty.SKILL_TYPE.LONG_HIT)
                {
                    AudioManager.Instance.PlaySE(Define.SE.SELECT_SOUND01);
                    CellFocusRight();
                }
                if (Define.InputBackButton())
                {
                    AudioManager.Instance.PlaySE(Define.SE.CANCEL_SOUND01);
                    owner.stateMachine.ChangeState(SKILL_STATE.COMMAND);
                }
            }
            public override void Exit()
            {
                stageController.cellSelectFrameOther.gameObject.SetActive(false);
                stageController.cellSelectFrame.gameObject.SetActive(false);
            }

            /// <summary>
            /// 予備フレームを移動
            /// </summary>
            /// <param name="cellIndex"></param>
            private void OtherCellFocus(int cellIndex)
            {
                if (!cellIndex.IsRange(0, stageController.craftCells.Count - 1))
                {
                    return;
                }
                var frame = stageController.cellSelectFrameOther;
                frame.transform.position = stageController.craftCells[cellIndex].cell.transform.position;
            }

            /// <summary>
            /// 選択フレームの移動
            /// </summary>
            /// <param name="dir">移動方向 [1:右],[2:下],[-1:左],[-2:上]</param>
            private void InputCellFocus(int dir)
            {
                int index = currentFocusCellIndex + dir;

                // 選択フレーム範囲チェック
                if (!index.IsRange(0, stageController.currentStageProperty.TotalCellCount - 1))
                {
                    return;
                }

                // 予備フレームの位置を設定
                switch (type)
                {
                    case SkillProperty.SKILL_TYPE.WIDE_HIT:
                        otherCellIndex = index + 1;


                        break;
                    case SkillProperty.SKILL_TYPE.LONG_HIT:
                        otherCellIndex = index + 2;
                        break;
                    default: break;
                }
                // 予備フレーム範囲チェック
                if (!otherCellIndex.IsRange(0, stageController.currentStageProperty.TotalCellCount - 1))
                {
                    return;
                }

                // 選択マスに選択フレームを移動
                currentFocusCellIndex = Mathf.Clamp(index, 0, stageController.currentStageProperty.TotalCellCount - 1);
                stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
                // 予備フレームを移動
                OtherCellFocus(otherCellIndex);
            }

            /// <summary>
            /// 選択セルを上に移動
            /// </summary>
            private void CellFocusUp()
            {
                InputCellFocus(-2);
            }

            /// <summary>
            /// 選択セルを下に移動
            /// </summary>
            private void CellFocusDown()
            {
                InputCellFocus(2);
            }

            /// <summary>
            /// 選択セルを右に移動
            /// </summary>
            private void CellFocusRight()
            {
                InputCellFocus(1);
            }

            /// <summary>
            /// 選択セルを左に移動
            /// </summary>
            private void CellFocusLeft()
            {
                InputCellFocus(-1);
            }
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
            int detailValue = CalcDetailValue();
            ShowDetail(detailValue);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Define.InputEnterButton() || Define.InputBackButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.DECISION_SOUND01);
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
            foreach (var cell in craftCells)
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

            if (value >= stageProperty.GreatDetailValue)
            {
                res = DETAIL_STATUS.GREAT;
            }
            else if (value >= stageProperty.GoodDetailValue)
            {
                res = DETAIL_STATUS.GOOD;
            }
            else if (value >= stageProperty.NormalDetailValue)
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
            int detailValue = CalcDetailValue();

            int revue = GetRevue(detailValue);

            Sprite sprite = owner.currentStageProperty.ResultSprite;
            owner.resultView.DisplayResult(sprite, revue, () => PlayResultSE(CheckDetail(detailValue)));

            AudioManager.Instance.PlaySE(Define.SE.COOLDOWN01);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (!owner.resultView.isFinishResult)
            {
                return;
            }

            if (Define.InputEnterButton())
            {
                AudioManager.Instance.PlaySE(Define.SE.HAMMER_HIT01);

                // 選択画面の評価に反映
                int detailValue = CalcDetailValue();
                int revue = GetRevue(detailValue);
                int stageIndex = owner.currentStageProperty.StageIndex;
                owner.selectController.GetRevueView(stageIndex - 1).LightOnStar(revue);
                owner.selectController.DisableMask(stageIndex - 1);
                MaskFadeController.Instance.FadeOut(1.0f, () => MonoBehaviourExtention.Delay(owner, 0.5f, () => { owner.TransState(STAGE_STATE.STAGE_END); }));
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// 結果によって再生するSEを変える
        /// </summary>
        private void PlayResultSE(DETAIL_STATUS status)
        {
            switch (status)
            {
                case DETAIL_STATUS.BAD: AudioManager.Instance.PlaySE(Define.SE.RESULT01); break;
                case DETAIL_STATUS.NORMAL: AudioManager.Instance.PlaySE(Define.SE.RESULT01); break;
                case DETAIL_STATUS.GOOD: AudioManager.Instance.PlaySE(Define.SE.RESULT02); break;
                case DETAIL_STATUS.GREAT: AudioManager.Instance.PlaySE(Define.SE.RESULT03); break;
            }
        }

        /// <summary>
        /// 評価値をもとに★の数を返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int GetRevue(int value)
        {
            int revue = 0;
            switch (CheckDetail(value))
            {
                case DETAIL_STATUS.BAD: revue = 0; break;
                case DETAIL_STATUS.NORMAL: revue = 1; break;
                case DETAIL_STATUS.GOOD: revue = 2; break;
                case DETAIL_STATUS.GREAT: revue = 3; break;
            }

            return revue;
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

            if (value >= stageProperty.GreatDetailValue)
            {
                res = DETAIL_STATUS.GREAT;
            }
            else if (value >= stageProperty.GoodDetailValue)
            {
                res = DETAIL_STATUS.GOOD;
            }
            else if (value >= stageProperty.NormalDetailValue)
            {
                res = DETAIL_STATUS.NORMAL;
            }

            return res;
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
            owner.resultView.Reset();
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
