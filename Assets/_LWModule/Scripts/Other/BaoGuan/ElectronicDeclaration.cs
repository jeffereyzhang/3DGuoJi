using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 报关电子申报
/// </summary>
public class ElectronicDeclaration : TaskBase
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
                //player = Prototype.NetworkLobby.LobbyManager.Instance.MyLobbyPlayer;
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
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报关_电子申报);
            player.transform.position = target.position;

            CameraCtrl.Instance.SetCameraTarget(player.transform);
        }
        StartElectronic();
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
            NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报关_审核单证);
        }
    }

    void StartElectronic()
    {
        PromptManager.Instance.Show(@"报关员收到中国中福实业总公司寄来的相关单证，审核通过后，现在请点击电脑打开中国电子口岸确认接受委托。",
                                    NotarizeType.Center, () => {
                                        //点击描边提示
                                        mOutline = computer.GetComponent<MouseoverOutline>();
                                        mOutline.ShowOutLine();
                                        ////点击描边提示
                                        //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.003f);

                                        PointToObject.Instance.Show(computer.transform.FindChild("Object002").position);
                                        //注册点击电脑
                                        tventT = AddEventTrigger(computer, PleaseClickComputer);
                                    });

    }

    /// <summary>
    /// 点击电脑打开中国电子口岸
    /// </summary>
    /// <param name="bed"></param>
    void  PleaseClickComputer(BaseEventData bed)
    {
        if (tventT)
        {
            //取消点击描边提示
            if (mOutline)
            {
                mOutline.HideOutLine();
                mOutline.OnMExit();
            }

            ////取消点击描边提示
            //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.000f);
            ////取消鼠标放上提示
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            tventT.triggers.Clear();
        }

        PointToObject.Instance.Hide();

        //发送打开中国电子口岸消息
        SocketManager.SendMsg(new NetModel(160));

        //注册口岸关闭监听消息
        SocketManager.RegisterMsgHandle(161, EnterSystem);
    }

    void EnterSystem()
    {
        //注册口岸关闭监听消息
        SocketManager.RemoveMsgHandle(161);
        PromptManager.Instance.Show("已经接受报关委托，接下来点击电脑进行电子申报业务。",
                                    NotarizeType.Center, () => {

                                        //点击描边提示
                                        mOutline = computer.GetComponent<MouseoverOutline>();
                                        mOutline.ShowOutLine();
                                        ////点击描边提示
                                        //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.003f);

                                        PointToObject.Instance.Show(computer.transform.FindChild("Object002").position);
                                        //注册点击电脑
                                        tventT = AddEventTrigger(computer, ClickComputerAgain);
                                    });
    }

    /// <summary>
    /// 点击电脑 打开中国电子口岸通关系统
    /// </summary>
    void ClickComputerAgain(BaseEventData bed)
    {
        if (tventT)
        {
            //取消点击描边提示
            if (mOutline)
            {
                mOutline.HideOutLine();
                mOutline.OnMExit();
            }

            ////取消点击描边提示
            //computer.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.000f);
            ////取消鼠标放上提示
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            tventT.triggers.Clear();
        }

        PointToObject.Instance.Hide();

        //注册点击电脑打开中国电子口岸通关系统消息
        SocketManager.SendMsg(new NetModel(162));
        //注册中国电子口岸通关系统监听
        SocketManager.RegisterMsgHandle(163, OverElectronic);
    }

    void OverElectronic()
    {
        PromptManager.Instance.Show("电子申报完成后，等待着海关处理该票报关单。",
                            NotarizeType.Center, () => {
                                ThisOnFinish(TaskState.任务成功);
                            });

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
