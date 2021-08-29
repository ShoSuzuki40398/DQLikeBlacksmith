using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    // メインUI切替
    private UISwicther swicther;

    // 選択しているステージ
    private int currentSelectIndex = 0;
    private int stageNum = 3;

    [SerializeField]
    private SelectController selectController;

    // Start is called before the first frame update
    void Start()
    {
        selectController = GetComponent<SelectController>();
        swicther = GetComponent<UISwicther>();

        swicther.switchUI(UISwicther.UI_NUMBER.OUTGAME);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //swicther.switchUI(UISwicther.UI_NUMBER.TITLE);
            currentSelectIndex = Mathf.Clamp(currentSelectIndex-1,0,stageNum-1);
            selectController.FocusFrame(currentSelectIndex);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //swicther.switchUI(UISwicther.UI_NUMBER.INAGAME);
            currentSelectIndex = Mathf.Clamp(currentSelectIndex + 1, 0, stageNum-1);
            selectController.FocusFrame(currentSelectIndex);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //swicther.switchUI(UISwicther.UI_NUMBER.OUTGAME);
        }
    }
}
