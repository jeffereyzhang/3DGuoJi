using UnityEngine;
using System.Collections;


public class UISystemTipsManager : UIBase
{
    public UILabel taskCardContent;
    public TweenPosition taskCardTP;
    public GameObject sureBtn;


    void Awake()
    {
    }

    void Start()
    {
        UIEventListener.Get(sureBtn).onClick += SureBtnClick;
    }

    public void SetTipsContent(string content)
    {
        taskCardContent.text = content;
        ShowWin();

    }

    private void SureBtnClick(GameObject go)
    {
        HideWin();
    }

    public void ShowWin()
    {
        taskCardTP.gameObject.SetActive(true);
        taskCardTP.PlayForward();
        EventDelegate.Remove(taskCardTP.onFinished, DestroyWin);
    }

    private void HideWin()
    {
        if (GameManager.systemTipsDelegate != null)
            GameManager.systemTipsDelegate();
        taskCardTP.PlayReverse();
        EventDelegate.Add(taskCardTP.onFinished, DestroyWin);
        GameManager.systemTipsDelegate = (() =>
        {
            Debug.Log("系统提示委托清空");
        });


    }

    private void DestroyWin()
    {
        taskCardTP.gameObject.SetActive(false);
    }
}
