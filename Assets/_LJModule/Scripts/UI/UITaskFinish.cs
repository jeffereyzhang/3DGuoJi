using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System;

public class UITaskFinish : UIBase
{
    public UILabel taskNameLabel;
    public UILabel roleLabel;
    public UILabel scoreLabel;
    public GameObject bg;

    private Action callBack;

    public UIButton sureBtn;

    public UIButton backBtn;

    void Start()
    {
        EventDelegate.Add(sureBtn.onClick, OnSureBtnClick);
        EventDelegate.Add(backBtn.onClick, OnBackBtnClick);

        if (GameManager.IsNet)
        {
            backBtn.gameObject.SetActive(false);//如果是网络模式，隐藏返回按钮
        }
    }

    private void OnBackBtnClick()
    {
        if (!GameManager.IsNet)
        {
            TaskManager.Instance.TaskFinish(GameManager._curTaskType, TaskState.放弃任务);
            //任务结束时同时通知winform界面切换到背景，开始后再将3D界面切换至前面
            SocketManager.SendMsg(new NetModel(103));
            SceneManager.LoadScene("LoadSence");
        }
        else
        {
            PromptManager.Instance.Show("多人模式下禁止用户自主退出实训！", NotarizeType.Center);
        }
    }

    private void OnSureBtnClick()
    {
        if (callBack != null && GameManager.IsNet)//网络版
        {
            //回传成绩
            NetModel nm = new NetModel(190);
            nm.MessageContent = new ProtoObject(UnityEngine.Random.Range(60, 99));
            SocketManager.SendMsg(nm);

            callBack();
        }
        else//单机版
        {

            //任务结束时同时通知winform界面切换到背景，开始后再将3D界面切换至前面
            //GradeManager.Instance.ReceiveScore(90);

            //回传成绩
            NetModel nm = new NetModel(190);
            nm.MessageContent = new ProtoObject(UnityEngine.Random.Range(10,19));
            SocketManager.SendMsg(nm);

            NetModel nm2 = new NetModel(103);
            nm.MessageContent = new ProtoObject("成绩测试用");
            SocketManager.SendMsg(nm2);

            SceneManager.LoadScene("LoadSence");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTaskFinishContent(TaskType _curTaskType, int score, Action _callBack = null)
    {
        bg.SetActive(true);
        taskNameLabel.text = _curTaskType.ToString();
        roleLabel.text = GameManager.GetTaskRoleName(_curTaskType).roleName;
        scoreLabel.text = score + "";
        if (_callBack != null)
        {
            callBack = _callBack;
        }
    }
}
