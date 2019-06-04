using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 报关 通关放行
/// </summary>
public class Clearance : TaskBase 
{
    /// <summary>
    /// 当前模块的角色
    /// </summary>
    private GameObject player;
    /// <summary>
    /// 任务开始时角色位置
    /// </summary>
    public Transform target;

    public GameObject computer;
    MouseoverOutline mOutline;


    void Awake()
    {
        TaskManager.Instance.RegisterTask(this);
    }

    void Start()
    {

        if (GameManager.IsNet)
        {
            if (GameManager._curTaskType == curTaskType)
            {
                GameManager.NetPlayerStartPos = target;
                NetGameManager.Instance.MultipleTasks.NetTaskAction += OnStart;
            }
        }
    }

    /// <summary>
    /// 自动调用  
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();

        if (!GameManager.IsNet)
        {
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报关_通关放行);
            player.transform.position = target.position;

            CameraCtrl.Instance.SetCameraTarget(player.transform);
        }

        StartClearance();
    }

    /// <summary>
    /// 自己调用  任务状态成功 
    /// </summary>
    /// <param name="finishState"></param>
    public void ThisOnFinish(TaskState finishState)
    {
        base.OnFinish(finishState);

        //如果是网络版，则通知下一个人
        if (GameManager.IsNet)
        {
            NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.NullTask);
        }
    }

    EventTrigger tventT;
    /// <summary>
    /// 添加点击相应事件
    /// </summary>
    EventTrigger AddEventTrigger(GameObject click, Action<BaseEventData> ac)
    {
        EventTrigger et = click.GetComponent<EventTrigger>();
        if (et == null)
        {
            et = click.AddComponent<EventTrigger>();
        }

        et.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerDown;
        enter.callback = new EventTrigger.TriggerEvent();
        UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(ac);
        enter.callback.AddListener(callback);

        MouseoverOutline mo;

        if (GetMouseoverOutline(click, out mo))
        {
            EventTrigger.Entry enter1 = new EventTrigger.Entry();
            enter1.eventID = EventTriggerType.PointerEnter;
            enter1.callback = new EventTrigger.TriggerEvent();
            UnityAction<BaseEventData> callback1 = new UnityAction<BaseEventData>((BaseEventData bed) => { mo.OnMEnter(); });
            enter1.callback.AddListener(callback1);

            et.triggers.Add(enter1);

            EventTrigger.Entry enter2 = new EventTrigger.Entry();
            enter2.eventID = EventTriggerType.PointerExit;
            enter2.callback = new EventTrigger.TriggerEvent();
            UnityAction<BaseEventData> callback2 = new UnityAction<BaseEventData>((BaseEventData bed) => { mo.OnMExit(); });
            enter2.callback.AddListener(callback2);

            et.triggers.Add(enter2);
        }


        et.triggers.Add(enter);

        return et;
    }

    /// <summary>
    /// 获取鼠标描边脚本
    /// </summary>
    bool GetMouseoverOutline(GameObject go, out MouseoverOutline mo)
    {
        mo = go.GetComponent<MouseoverOutline>();
        if (mo != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 开始通关放行
    /// </summary>
    void StartClearance()
    {
        PromptManager.Instance.Show("你收到了新的放行审核数据，请进行审核放行。", NotarizeType.Center, () =>{
            //发送学习海关放行知识点消息
            SocketManager.SendMsg(new NetModel(174));
            //注册海关放行知识学习完成监听
            SocketManager.RegisterMsgHandle(175, Clickcomputer);
        });
    }

    void Clickcomputer()
    {
        PromptManager.Instance.Show("点击电脑，打开海关平台进行操作。", NotarizeType.Center, () =>
        {
            ////点击描边提示
            //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.003f);
            //点击描边提示
            mOutline = computer.GetComponent<MouseoverOutline>();
            mOutline.ShowOutLine();

            PointToObject.Instance.Show(computer.transform.FindChild("Object002").position);
            tventT = AddEventTrigger(computer, OnClick);
        });
    }

    void OnClick(BaseEventData bed)
    {
        if (tventT)
        {
            ////取消点击描边提示
            //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.000f);
            ////取消鼠标放上提示
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            //取消点击描边提示
            if (mOutline)
            {
                mOutline.HideOutLine();
                mOutline.OnMExit();
            }

            tventT.triggers.Clear();
        }

        PointToObject.Instance.Hide();

        //发送打开海关平台消息消息
        SocketManager.SendMsg(new NetModel(176));
        //注册海关平台关闭完成监听
        SocketManager.RegisterMsgHandle(177, EndClearance);
    }

    void EndClearance()
    {
        PromptManager.Instance.Show("企业可以根据海关放行信息，办理货物放行、提取货物。", NotarizeType.Center, () =>
        {
            ThisOnFinish(TaskState.任务成功);
        });
    }
}
