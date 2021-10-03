using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System;

public class StageController : MonoBehaviour
{
    // �X�e�[�W���
    public enum STAGE_STATE
    {
        IDEL,   // �ҋ@
        HIT,    // ������
        SKILL,  // ���Z
        CHECK,  // ���킵������
        FINISH,  // ��������
        STAGE_END // �X�e�[�W�I��
    }

    // ���앨�]��
    public enum DETAIL_STATUS
    {
        BAD,
        NORMAL,
        GOOD,
        GREAT
    }


    // �X�e�[�W��Ԑ���
    public readonly StateMachine<StageController, STAGE_STATE> stateMachine = new StateMachine<StageController, STAGE_STATE>();

    private SkillManager skillManager;



    /// -----------------------------------------------
    /// Left�p�l��
    /// -----------------------------------------------
    // �K�C�h�\���e�L�X�g
    private const string GUIDE_DESCRIPTION_NEUTRAL = "�ǂ�����H";

    // �X�e�[�W���������l�Ƃ���i�s���X�e�[�W���
    private StageProperty currentStageProperty;

    // �X�e�[�W���̏����l
    private StageProperty initStageProperty;

    // �K�C�h�e�L�X�g
    [SerializeField]
    private Text guideText;

    // ���x�e�L�X�g
    [SerializeField]
    private Text heatLevelText;

    // �R�}���h�e�L�X�g
    [SerializeField]
    private GameObject commandTextParent;

    // �R�}���h�����e�L�X�g
    [SerializeField]
    private Text commandDiscriptionText;

    // �R�}���h���s�K�vHP
    [SerializeField]
    private Text commandNeedHpText;

    // �R�}���h�A�Z�b�g���X�g�i��X���I�ɕύX�ł���悤�ɂ��邩���j
    [SerializeField]
    private List<CommandPropertyAsset> commandPropertyAssets = new List<CommandPropertyAsset>();
    private List<CommandProperty> commandProperties = new List<CommandProperty>();

    // �R�}���h�e�L�X�g�v���n�u
    [SerializeField]
    private Text comamndTextPrefab;

    // �I�𒆂̃R�}���h�C���f�b�N�X
    private int currentCommandIndex = 0;

    // �R�}���h�I��g
    [SerializeField]
    private UISquareFrame commandSelectFrame;

    // �R�}���h�I�u�W�F�N�g
    [SerializeField]
    private List<GameObject> commandSelectObjects;


    //// ===== �X�L�� =====

    //�@�X�L���R�}���h�I�u�W�F�N�g�̐e
    [SerializeField]
    private GameObject skillCommandParent;

    // �X�L���R�}���h�I�u�W�F�N�g
    [SerializeField]
    private List<GameObject> skillSelectObjects;

    // �R�}���h�I��g
    [SerializeField]
    private UISquareFrame skillSelectFrame;

    // �I�𒆂̃X�L���R�}���h�C���f�b�N�X
    private int currentSkillIndex = 0;

    /// -----------------------------------------------
    /// Center�p�l��
    /// -----------------------------------------------

    // ���앨�}�X�Ɛi���Q�[�WUI
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

    // ���앨�}�X���X�g
    private List<CraftCell> craftCells = new List<CraftCell>();

    // ���앨�p�l��
    [SerializeField]
    private GameObject itemPanel;


    [SerializeField]
    private GameObject guagePanel;

    //// ===== �}�X =====

    // ���앨�I�u�W�F�N�g�̃v���n�u
    [SerializeField]
    private GameObject itemSliceImagePrefab;

    // ���앨�}�X�I��g
    [SerializeField]
    private UISquareFrame cellSelectFrame;

    //// ===== �i���Q�[�W =====
    // �i���Q�[�W�I�u�W�F�N�g�̃v���n�u
    [SerializeField]
    private GameObject guagePrefab;

    // �Q�[�W���L�т�܂łɂ����鎞��
    [SerializeField]
    private float changeTimeGuageValue = 1.0f;

    // �Q�[�W�i�����̃G�t�F�N�g
    [SerializeField]
    private GameObject hitEffectPefab;

    /// -----------------------------------------------
    /// Footer�p�l��
    /// -----------------------------------------------

    // �t�b�^�[UI
    [SerializeField]
    private FooterUI footer;

    // �t�b�^�[���
    [SerializeField]
    private FooterPropertyAsset footerPropertyAsset;
    private FooterProperty footerProperty;

    private void Start()
    {
        // ��ԓo�^
        stateMachine.AddState(STAGE_STATE.IDEL, new IdleState(this));
        stateMachine.AddState(STAGE_STATE.HIT, new HitState(this));
        stateMachine.AddState(STAGE_STATE.SKILL, new SkillState(this));
        stateMachine.AddState(STAGE_STATE.CHECK, new CheckState(this));
        stateMachine.AddState(STAGE_STATE.FINISH, new FinishState(this));
        stateMachine.AddState(STAGE_STATE.STAGE_END, new StageEndState(this));

        footerProperty = footerPropertyAsset.footerProperty;

        skillManager = GetComponent<SkillManager>();
    }

    /// <summary>
    /// �X�e�[�W�X�V
    /// </summary>
    /// <returns>�X�e�[�W�I����true</returns>
    public bool Execute()
    {
        //if(Input.GetKeyDown(KeyCode.D))
        //{
        //    if(Input.GetKey(KeyCode.LeftShift))
        //    {
        //        Craft(1, 10);
        //    }
        //    else
        //    {
        //        Craft(0, 10);
        //    }

        //}
        //else if(Input.GetKeyDown(KeyCode.A))
        //{
        //    StartCoroutine(ChangeGuageValue(0, -5));
        //}
        //else if (Input.GetKeyDown(KeyCode.W))
        //{
        //    Craft(0, 1);
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{
        //    StartCoroutine(ChangeGuageValue(0, -1));
        //}

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
    /// �X�e�[�W���ݒ�
    /// </summary>
    public void SetPropertyAsset(StagePropertyAsset propertyAsset)
    {
        currentStageProperty = initStageProperty = propertyAsset.stageProperty;
        heatLevelText.text = MakeHeatLevelText(currentStageProperty.HeatLevel);
    }

    /// <summary>
    /// �X�e�[�W�J�n����
    /// </summary>
    public void StageEntry()
    {
        guideText.text = GUIDE_DESCRIPTION_NEUTRAL;

        // �R�}���h�쐬
        currentCommandIndex = 0;
        CreateCommand();

        // �X�L���쐬
        currentSkillIndex = 0;
        CreateSkillCommand();

        // ���앨�쐬
        CreateItem();

        // �I��g��\��
        cellSelectFrame.gameObject.SetActive(false);

        // �ҋ@��ԂɑJ��
        TransState(STAGE_STATE.IDEL);
    }

    /// <summary>
    /// �X�e�[�W�I������
    /// </summary>
    public void StageExit()
    {
        // �R�}���h�폜
        commandSelectObjects.Clear();
        commandTextParent.DestroyChildren();

        // �X�L���폜
        skillSelectObjects.Clear();
        skillCommandParent.DestroyChildren();

        // ���앨�폜
        craftCells.Clear();
        itemPanel.DestroyChildren();
        guagePanel.DestroyChildren();
    }

    /// <summary>
    /// �R�}���h�I�����쐬
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

        // �����e�L�X�g�Ə���̗̓e�L�X�g�쐬
        if (commandProperties.Count > 0)
        {
            UpdateDiscStrAndNeedHpStr(commandProperties[0]);
        }

        // �I��g�̏����ʒu��ݒ�ilayoutgroup�̌v�Z��ɂ���������1�t���[���x���j
        MonoBehaviourExtention.Delay(this, 1, () => { commandSelectFrame.transform.position = commandSelectObjects[currentCommandIndex].transform.position; });
    }

    /// <summary>
    /// �X�L���R�}���h�쐬
    /// </summary>
    public void CreateSkillCommand()
    {
        foreach (var property in skillManager.SkillProperties)
        {
            Text obj = Instantiate(comamndTextPrefab, skillCommandParent.transform);
            obj.text = property.SkillStr;
            skillSelectObjects.Add(obj.gameObject);
        }

        // �I��g�̏����ʒu��ݒ�ilayoutgroup�̌v�Z��ɂ���������1�t���[���x���j
        MonoBehaviourExtention.Delay(this, 1, () =>
        {
            skillSelectFrame.transform.position = skillSelectObjects[currentSkillIndex].transform.position;
            skillCommandParent.SetActive(false);
            skillSelectFrame.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// ���앨�C���[�W�쐬
    /// </summary>
    private void CreateItem()
    {
        if (currentStageProperty == null)
        {
            return;
        }

        // Image�I�u�W�F�N�g�쐬
        // StageProperty���琻�앨�摜�擾
        foreach (var property in currentStageProperty.ItemCellProperties)
        {
            // ���앨�}�X
            GameObject cellObj = Instantiate(itemSliceImagePrefab, itemPanel.transform);
            ItemSliceImageCell cell = cellObj.GetComponent<ItemSliceImageCell>();
            cell.itemImage.sprite = property.ItemSliceSprite;

            // �i���Q�[�W
            GameObject guageObj = Instantiate(guagePrefab, guagePanel.transform);
            CraftGuage guage = guageObj.GetComponent<CraftGuage>();

            // �i���Q�[�W�̗��z�l�␬���G���A�̒l�ݒ������
            guage.SetSliderWidth(property.LimitValuef);
            guage.SetSliderMaxValue(property.LimitValuef);
            guage.SetSliderValue(0);
            guage.SetSuccessAreaWidth(property._SuccessArea.Width);
            guage.SetSliderColor(Define.craftGuageNormalColor);

            craftCells.Add(new CraftCell(cell, guage));
        }

        // �Q�[�W����(�ʒu�A����)
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
                {   // �}�X�̍���
                    x -= guage.GetComponent<RectTransform>().sizeDelta.x;
                    guage.SetDirection(Slider.Direction.RightToLeft);
                    guage.SetIdealPointAnchorRight();
                    guage.SetIdealPointX(-property.IdealValue);
                }
                else
                {   // �}�X�̉E��
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
    /// ����̗̓e�L�X�g�쐬
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
    /// ����̗̓e�L�X�g�쐬
    /// </summary>
    /// <param name="hp"></param>
    /// <returns></returns>
    private string MakeHeatLevelText(int heatLevel)
    {
        var result = new StringBuilder();
        result.Append(heatLevel);
        result.Append("��");

        return result.ToString();
    }

    /// <summary>
    /// �����Ə���̗̓e�L�X�g�̍X�V
    /// </summary>
    /// <returns></returns>
    private void UpdateDiscStrAndNeedHpStr(CommandProperty property)
    {
        commandDiscriptionText.text = property.CommandDiscriptionStr;
        commandNeedHpText.text = MakeNeedHpText(property.NeedHp);
    }

    /// <summary>
    /// �����i�߂�
    /// </summary>
    private void Craft(int cellIndex, float value)
    {
        CreateHitEffect(cellIndex, value);
        StartCoroutine(ChangeGuageValue(cellIndex, value));
    }

    /// <summary>
    /// �i���G�t�F�N�g�쐬
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
    /// �i���Q�[�W��L�΂�
    /// </summary>
    /// <param name="cellIndex">�w��̃Z��</param>
    /// <param name="value">�L�΂����l�ivalue���������l�ɂȂ�j</param>
    /// <returns></returns>
    private IEnumerator ChangeGuageValue(int cellIndex, float value)
    {
        var property = currentStageProperty.ItemCellProperties[cellIndex];
        var guage = craftCells[cellIndex].guage;
        // ���ɏ���܂ł����Ă����牽�����Ȃ�
        if (property.LimitValuef < guage.SliderValue)
        {
            yield break;
        }

        float time = changeTimeGuageValue;
        float startTime = Time.timeSinceLevelLoad;

        var startGuageValue = guage.SliderValue;
        //Debug.Log("SliderValue :" + (guage.SliderValue + value));
        while (true)
        {
            var diff = Time.timeSinceLevelLoad - startTime;
            var rate = diff / time;
            guage.SliderValue = startGuageValue + (value * rate);

            // �Q�[�W�ύX�I��
            if (diff > time)
            {
                guage.SliderValue = startGuageValue + value;

                // �F�ς�
                switch (guage.GetGuageStatus(property))
                {
                    case CraftGuage.GUAGE_STATUS.IDEAL: guage.SetSliderColor(Define.craftGuageIdealColor); break;
                    case CraftGuage.GUAGE_STATUS.SUCCESS: guage.SetSliderColor(Define.craftGuageSuccessColor); break;
                    case CraftGuage.GUAGE_STATUS.NORMAL: guage.SetSliderColor(Define.craftGuageNormalColor); break;
                }

                break;
            }
            //yield break;
        }
    }

    //===============================================================================================================
    //===����Ԓ�`��================================================================================================
    //===============================================================================================================

    /// <summary>
    /// �ҋ@���
    /// </summary>
    private class IdleState : State<StageController>
    {
        public IdleState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            Debug.Log("�ҋ@");
        }

        /// <summary>
        /// ��ԍX�V
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
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// �R�}���h����Ɉړ�
        /// </summary>
        public void CommandFocusUp()
        {
            owner.currentCommandIndex = Mathf.Clamp(owner.currentCommandIndex - 1, 0, owner.commandProperties.Count - 1);
            owner.commandSelectFrame.transform.position = owner.commandSelectObjects[owner.currentCommandIndex].transform.position;
            owner.UpdateDiscStrAndNeedHpStr(owner.commandProperties[owner.currentCommandIndex]);
        }

        /// <summary>
        /// �R�}���h�����Ɉړ�
        /// </summary>
        public void CommandFocusDown()
        {
            owner.currentCommandIndex = Mathf.Clamp(owner.currentCommandIndex + 1, 0, owner.commandProperties.Count - 1);
            owner.commandSelectFrame.transform.position = owner.commandSelectObjects[owner.currentCommandIndex].transform.position;
            owner.UpdateDiscStrAndNeedHpStr(owner.commandProperties[owner.currentCommandIndex]);
        }

    }

    /// <summary>
    /// ���������
    /// </summary>
    private class HitState : State<StageController>
    {
        // �I�𒆂̃}�X
        private int currentFocusCellIndex = 0;

        public HitState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            Debug.Log("������");
            currentFocusCellIndex = 0;
            owner.cellSelectFrame.gameObject.SetActive(true);
            owner.cellSelectFrame.transform.position = owner.craftCells[currentFocusCellIndex].cell.transform.position;
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // ���E�E�E�Eskillmanager����u�������v�X�L���擾
                owner.TransState(STAGE_STATE.IDEL);
                //owner.Craft(currentFocusCellIndex,);
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
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            owner.cellSelectFrame.gameObject.SetActive(false);
        }

        /// <summary>
        /// �I���Z������Ɉړ�
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
        /// �I���Z�������Ɉړ�
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
        /// �I���Z�����E�Ɉړ�
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
        /// �I���Z�������Ɉړ�
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
    /// �Ƃ������
    /// </summary>
    private class SkillState : State<StageController>
    {
        enum SKILL_STATE
        {
            COMMAND,    // ���Z�I��
            CELL        // �}�X�I��
        }


        private StateMachine<SkillState, SKILL_STATE> stateMachine = new StateMachine<SkillState, SKILL_STATE>();

        public SkillState(StageController owner) : base(owner)
        {
            stateMachine.AddState(SKILL_STATE.COMMAND, new CommandState(this));
            stateMachine.AddState(SKILL_STATE.CELL, new CellState(this));
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            Debug.Log("�Ƃ���");
            owner.currentSkillIndex = 0;
            owner.skillCommandParent.SetActive(true);
            owner.skillSelectFrame.gameObject.SetActive(true);
            owner.skillSelectFrame.transform.position = owner.skillSelectObjects[0].transform.position;
            owner.footer.SetFooterText(owner.skillManager.GetSkillDiscription(0));
            stateMachine.ChangeState(SKILL_STATE.COMMAND);
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            stateMachine.Update();
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            owner.skillCommandParent.SetActive(false);
            owner.skillSelectFrame.gameObject.SetActive(false);
            owner.footer.ClearFooterText();
            owner.cellSelectFrame.gameObject.SetActive(false);
        }

        /// <summary>
        /// ���Z�R�}���h�I��
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
                Debug.Log("�X�L���R�}���h�I��");
            }

            public override void Execute()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (stageController.skillManager.GetProperty(stageController.currentSkillIndex).IsCellSelected)
                    {   // �}�X�I���Ɉړ�
                        owner.stateMachine.ChangeState(SKILL_STATE.CELL);
                        // ���E�E�E�E�E
                    }
                    else
                    {   // �}�X�I�������ɃX�L�����s
                        // �����E�E�E�E�E(�X�L�����s����)
                        stageController.TransState(STAGE_STATE.IDEL);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    CommandFocusUp();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    CommandFocusDown();
                }
                else if (Define.InputBackButton())
                {
                    stageController.TransState(STAGE_STATE.IDEL);
                }
            }
            public override void Exit()
            {
            }

            /// <summary>
            /// �R�}���h����Ɉړ�
            /// </summary>
            public void CommandFocusUp()
            {
                stageController.currentSkillIndex = Mathf.Clamp(stageController.currentSkillIndex - 1, 0, stageController.skillManager.SkillCount - 1);
                stageController.skillSelectFrame.transform.position = stageController.skillSelectObjects[stageController.currentSkillIndex].transform.position;
                stageController.footer.SetFooterText(stageController.skillManager.GetSkillDiscription(stageController.currentSkillIndex));
            }

            /// <summary>
            /// �R�}���h�����Ɉړ�
            /// </summary>
            public void CommandFocusDown()
            {
                stageController.currentSkillIndex = Mathf.Clamp(stageController.currentSkillIndex + 1, 0, stageController.skillManager.SkillCount - 1);
                stageController.skillSelectFrame.transform.position = stageController.skillSelectObjects[stageController.currentSkillIndex].transform.position;
                stageController.footer.SetFooterText(stageController.skillManager.GetSkillDiscription(stageController.currentSkillIndex));
            }

        }

        /// <summary>
        /// �}�X�I��
        /// </summary>
        private class CellState : State<SkillState>
        {
            private StageController stageController;

            // �I�𒆂̃}�X
            private int currentFocusCellIndex = 0;

            public CellState(SkillState owner) : base(owner)
            {
                stageController = owner.owner;
            }

            public override void Enter()
            {
                Debug.Log("�}�X�I��");
                currentFocusCellIndex = 0;
                stageController.cellSelectFrame.gameObject.SetActive(true);
                stageController.cellSelectFrame.transform.position = stageController.craftCells[currentFocusCellIndex].cell.transform.position;
            }
            public override void Execute()
            {
                if (Define.InputEnterButton())
                {
                    // ���E�E�E�E�E�X�L�����s����
                    stageController.TransState(STAGE_STATE.IDEL);
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
                if (Define.InputBackButton())
                {
                    owner.stateMachine.ChangeState(SKILL_STATE.COMMAND);
                }
            }
            public override void Exit()
            {
                stageController.cellSelectFrame.gameObject.SetActive(false);
            }

            /// <summary>
            /// �I���Z������Ɉړ�
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
            /// �I���Z�������Ɉړ�
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
            /// �I���Z�����E�Ɉړ�
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
            /// �I���Z�������Ɉړ�
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
    }

    /// <summary>
    /// ���킵��������
    /// </summary>
    private class CheckState : State<StageController>
    {
        public CheckState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            Debug.Log("���킵������");

            int detailValue = CalcDetailValue();
            ShowDetail(detailValue);
        }

        /// <summary>
        /// ��ԍX�V
        /// </summary>
        public override void Execute()
        {
            if (Define.InputEnterButton() || Define.InputBackButton())
            {
                owner.TransState(STAGE_STATE.IDEL);
            }
        }

        /// <summary>
        /// ��ԏI����
        /// </summary>
        public override void Exit()
        {
            HideDetail();
        }

        /// <summary>
        /// �ڍו\��
        /// </summary>
        private void ShowDetail(int detailValue)
        {
            var craftCells = owner.craftCells;
            foreach (var cell in craftCells)
            {
                cell.guage.ShowValue();
            }

            // �o�����\��
            switch (CheckDetail(detailValue))
            {
                case DETAIL_STATUS.BAD: owner.footer.SetFooterText(owner.footerProperty.DetailTextBad); break;
                case DETAIL_STATUS.NORMAL: owner.footer.SetFooterText(owner.footerProperty.DetailTextNormal); break;
                case DETAIL_STATUS.GOOD: owner.footer.SetFooterText(owner.footerProperty.DetailTextGood); break;
                case DETAIL_STATUS.GREAT: owner.footer.SetFooterText(owner.footerProperty.DetailTextGreat); break;
            }
        }

        /// <summary>
        /// �ڍה�\��
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
        /// �]���l�v�Z
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
        /// �]���l�`�F�b�N
        /// </summary>
        /// <param name="value">�e�Q�[�W�̍��Z�]���l</param>
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
    /// ����������
    /// </summary>
    private class FinishState : State<StageController>
    {
        public FinishState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            Debug.Log("��������");
            owner.TransState(STAGE_STATE.IDEL);
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
    /// �X�e�[�W�I�����
    /// </summary>
    private class StageEndState : State<StageController>
    {
        public StageEndState(StageController owner) : base(owner)
        {
        }

        /// <summary>
        /// ��ԊJ�n��
        /// </summary>
        public override void Enter()
        {
            Debug.Log("�I���");
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
}
