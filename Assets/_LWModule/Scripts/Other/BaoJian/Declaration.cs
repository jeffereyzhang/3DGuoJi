using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Prototype.NetworkLobby;

/// <summary>
/// 报检 电子申报 
/// </summary>
public class Declaration : TaskBase
{
    /// <summary>
    /// 当前模块的角色
    /// </summary>
    private GameObject player;
    /// <summary>
    /// 任务开始时角色位置
    /// </summary>
    public Transform target;

    /// <summary>
    /// 前台处的单据预设
    /// </summary>
    public GameObject documents;

    /// <summary>
    /// 工位上的电脑
    /// </summary>
    public GameObject computer;
    MouseoverOutline mOutline;

    /// <summary>
    /// 角色要去的目标地点
    /// </summary>
    public List<Transform> taskPos = new List<Transform>();


    void Awake()
    {
        TaskManager.Instance.RegisterTask(this);
    }

    void Start ()
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

    /// <summary>
    /// 自动调用  
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        StartDeclaration();
    }

    /// <summary>
    /// 自己调用  任务状态成功 
    /// </summary>
    /// <param name="finishState"></param>
    protected override void OnFinish(TaskState finishState)
    {
        //UIFunctionButton functionButton = UIContainer.Instance.GetUI<UIFunctionButton>();
        //functionButton.ClearKnapsack();

        base.OnFinish(finishState);

        //如果是网络版，则通知下一个人
        if (GameManager.IsNet)
        {
            NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报检_审核报检材料);
        }
    }

    /// <summary>
    /// 任务开始 
    /// </summary>
    void StartDeclaration()
    {
        if (!GameManager.IsNet)
        {
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报检_电子申报);
            player.transform.position = target.position;
            Debuge.LogError("player = " + player.name);
            CameraCtrl.Instance.SetCameraTarget(player.transform);
        }

        PromptManager.Instance.Show("前台处收到了新的委托申请材料，请前往前台领取。", NotarizeType.Center);
        //在前台显示箭头指引
        RemindPos.Instance.Show(taskPos[0].position, ArriveReception);
    }

    /// <summary>
    /// 到达前台
    /// </summary>
    void ArriveReception(GameObject go)
    {
        if (GameManager.IsNet)
        {
            player = GameManager.MyPlayer.gameObject;
        }

        //这里加判断是否就是当前的角色
        if (go == player)
        {
            PromptManager.Instance.HidePN();

            PromptManager.Instance.Show("点击拾取前台上的委托申请材料。", NotarizeType.Center, PackUp);
        }
    }

    void PackUp()
    {
        //点击描边提示
        mOutline = documents.GetComponent<MouseoverOutline>();
        mOutline.ShowOutLine();

        Vector3 pos = documents.transform.position;
        pos.y = pos.y - 0.15f;
        PointToObject.Instance.Show(pos);

        clickComputerEvent = null;
        clickComputerEvent = AddEventTrigger(documents, OnClickDocument);

        RemindPos.Instance.Hide();
    }

    MouseoverOutline mo;

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

        et.triggers.Add(enter);

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

        return et;
    }

    /// <summary>
    /// 获取鼠标描边脚本
    /// </summary>
    bool GetMouseoverOutline(GameObject go, out MouseoverOutline mo)
    {
        mo = null;
        mo = go.GetComponent<MouseoverOutline>();
        if (mo != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 点击单据  然后提示返回工位
    /// </summary>
    /// <param name="bed"></param>
    void OnClickDocument(BaseEventData bed)
    {
        //UIFunctionButton  functionButton = UIContainer.Instance.GetUI<UIFunctionButton>();
        //functionButton.AddDocuments("委托申请材料", SendOpenExamine);

        PromptManager.Instance.Show("已拾取委托申请材料，返回工位后进行进行审查！。", NotarizeType.Center);
        RemindPos.Instance.Show(taskPos[1].position, ArriveStation);

        PointToObject.Instance.Hide();

        if (clickComputerEvent)
        {

            //clickComputerEvent.triggers.Clear();

            if (mOutline)
            {
                mOutline.HideOutLine();
                mOutline.OnMExit();
            }

            ////点击描边提示
            //documents.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.003f);
            ////取消鼠标放上提示
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);


            //点击完成之后因此 快件
            documents.SetActive(false);
        }
    }

    /// <summary>
    /// 发送打开委托材料页面的消息
    /// </summary>
    void SendOpenExamine()
    {
        SocketManager.SendMsg(new NetModel(128));
    }

    /// <summary>
    /// 到达工位  注册审查报检材料返回的事件
    /// </summary>
    /// <param name="go"></param>
    void ArriveStation(GameObject go)
    {
        if (GameManager.IsNet)
        {
            player = GameManager.MyPlayer.gameObject;
        }
        if (go == player)
        {
            PromptManager.Instance.Show("将对委托申请材料进行审查。之后还可以在案例背景中进行查看！", NotarizeType.Center,() => {

                SocketManager.SendMsg(new NetModel(128));
                SocketManager.RegisterMsgHandle(129, ExamineFinish);
               // ExamineFinish();
            });

            RemindPos.Instance.Hide();

        }
    }

    /// <summary>
    /// 审查完毕 发送消息给form打开选中回执方
    /// </summary>
    void ExamineFinish()
    {
     //   接受委托回执消息
        SocketManager.SendMsg(new NetModel(136));
        SocketManager.RegisterMsgHandle(137, ReceiptFinish);
       // ReceiptFinish();
    }

    EventTrigger clickComputerEvent;

    /// <summary>
    /// 回执完成  开始电子报关
    /// </summary>
    void ReceiptFinish()
    {
        PromptManager.Instance.Show("点击办公桌上的电脑，打开全国检验检疫无纸化系统！", NotarizeType.Center, AddClickComputer);

    }

    void AddClickComputer()
    {
        //点击描边提示
        mOutline = computer.GetComponent<MouseoverOutline>();
        mOutline.ShowOutLine();

        //  clickComputerEvent.triggers.Clear();

        if (clickComputerEvent != null)
        {
            clickComputerEvent = null;
        }
        clickComputerEvent = AddEventTrigger(computer, OnClickComputer);

        PointToObject.Instance.Show(computer.transform.FindChild("Object002").position);
    }

    /// <summary>
    /// 点击电脑 发送打开无纸化系统
    /// </summary>
    /// <param name="bed"></param>
    void OnClickComputer(BaseEventData bed)
    {
        if (clickComputerEvent)
        {
            //clickComputerEvent.triggers.Clear();

            //取消描边
            if (mOutline)
            {
                mOutline.HideOutLine();
                mOutline.OnMExit();
            }

            BoxCollider box = computer.GetComponent<BoxCollider>();
            if (box != null)
            {
                box.enabled = false;
            }

            ////点击描边提示
            //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.003f);
            ////取消鼠标放上提示
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        PointToObject.Instance.Hide();

        SocketManager.SendMsg(new NetModel(124));
        SocketManager.RegisterMsgHandle(125, EndDeclaration);
    }

    void EndDeclaration()
    {
        PromptManager.Instance.Show("电子申报已完成，等待检验检疫工作人员审核单据。", NotarizeType.Center,() => { OnFinish(TaskState.任务成功); });
    }

}
