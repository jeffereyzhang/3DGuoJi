using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 报关 审核单证
/// </summary>
public class AuditDocuments : TaskBase
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
    //    OnStart();
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 100, 50), "退出重新进"))
    //    {
    //        Application.LoadLevel(Application.loadedLevelName);
    //    }
    //}

    /// <summary>
    /// 自动调用  
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();

        if (!GameManager.IsNet)
        {
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报关_审核单证);
            player.transform.position = target.position;

            CameraCtrl.Instance.SetCameraTarget(player.transform);
        }

        StartAudit();
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
            NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报关_现场查验);
        }
    }

    void StartAudit()
    {
        PromptManager.Instance.Show("你收到了新的报关单数据，该票货物需要人工审单,首先学习一下审单方式及注意事项。", NotarizeType.Center, OpenAuditWay);
    }

    /// <summary>
    /// 打开审单方式知识点
    /// </summary>
    void OpenAuditWay()
    {
        //发送学习审单知识点消息
        SocketManager.SendMsg(new NetModel(164));
        //注册审单知识点学习完成监听
        SocketManager.RegisterMsgHandle(165, Audit);

      //  Audit();
    }

    ///// <summary>
    ///// 学习结束
    ///// </summary>
    //void EndStudy()
    //{
    //    PromptManager.Instance.Show("你已经完成了审核方式的学习，审单开始前还需要学习电子口岸报关单审核过程中的注意事项。", NotarizeType.Center, StudyPrecautions);
    //}

    /// <summary>
    /// 打开审核
    /// </summary>
    void Audit()
    {

        Debuge.LogError("Audit");
        PromptManager.Instance.Show("请点击电脑，开始审单！",
                                    NotarizeType.Center,
                                    () =>
                                    {
                                        ////点击描边提示
                                        //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.003f);

                                        if (computer == null)
                                        {
                                            Debuge.LogError("电脑不见了"+gameObject.name);
                                        }
                                        //点击描边提示
                                        mOutline = computer.GetComponent<MouseoverOutline>();
                                        mOutline.ShowOutLine();

                                        PointToObject.Instance.Show(computer.transform.FindChild("Object002").position);
                                        tventT = AddEventTrigger(computer, EndPrecautions);
                                    });

    }

    ///// <summary>
    ///// 学习注意事项知识点
    ///// </summary>
    //void StudyPrecautions()
    //{
    //    //发送学习注意事项知识点消息
    //    SocketManager.SendMsg(new NetModel(152));
    //    //注册学习注意事项知识点完成监听
    //    SocketManager.RegisterMsgHandle(153, Audit);
    //}

    /// <summary>
    /// 注意事项知识点学习完成
    /// </summary>
    void EndPrecautions(BaseEventData bed)
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

        //发送审单消息
        SocketManager.SendMsg(new NetModel(166));
        //注册审单完成监听
        SocketManager.RegisterMsgHandle(167, SendResult);
     //   SendResult();
        PointToObject.Instance.Hide();
    }

    /// <summary>
    /// 发送审单结果知识点
    /// </summary>
    void SendResult()
    {
        PromptManager.Instance.Show("预审核通过，审单中心向业务现场海关发送有关指令和数据，同时向报关人发出“到现场海关办理货物验放手续”的回执或通知。",
                            NotarizeType.Center,
                            () =>
                            {
                                //发送审单结果知识点消息
                                SocketManager.SendMsg(new NetModel(170));
                                //注册学习审单结果知识点完成监听
                                SocketManager.RegisterMsgHandle(171, EndResult);
                             //   EndResult();
                            });
    }

    /// <summary>
    /// 审单结果知识点结束
    /// </summary>
    void EndResult()
    {
        ThisOnFinish(TaskState.任务成功);
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
}
