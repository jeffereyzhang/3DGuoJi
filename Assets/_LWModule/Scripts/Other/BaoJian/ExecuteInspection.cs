using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 报检 检验检疫
/// </summary>
public class ExecuteInspection : TaskBase
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
    /// 角色要去的目标地点
    /// </summary>
    public List<Transform> taskPos = new List<Transform>();

    public GameObject leftDoor;
    public GameObject rightDoor;

    public GameObject machine;

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
 //       OnStart();
    }

    void Update()
    {

    }

    /// <summary>
    /// 自动调用  
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        StartExecuteInspection();

    }

    /// <summary>
    /// 自己调用  任务状态成功 
    /// </summary>
    /// <param name="finishState"></param>
    public void ThisOnFinish(TaskState finishState)
    {
        //UIFunctionButton functionButton = UIContainer.Instance.GetUI<UIFunctionButton>();
        //functionButton.ClearKnapsack();

        base.OnFinish(finishState);

        //如果是网络版，则通知下一个人
        if (GameManager.IsNet)
        {
            NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报检_出证放行);
        }
    }

    /// <summary>
    /// 开始检验检疫任务
    /// </summary>
    void StartExecuteInspection()
    {
        if (GameManager.IsNet)
        {
            player = GameManager.MyPlayer.gameObject;
        }
        
        if (!GameManager.IsNet)
        {
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报检_实施检验检疫);
            player.transform.position = target.position;

            CameraCtrl.Instance.SetCameraTarget(player.transform);
        }

        PromptManager.Instance.Show("报检材料审核通过，报检员已经缴费并预约现在去宁波港堆场的查验区，在堆工作人员的陪同下实施检验检疫，现在前往目的地。", NotarizeType.Center);
        //在前台显示箭头指引
        RemindPos.Instance.Show(taskPos[0].position, ArrivePosition);
    }

    void ArrivePosition(GameObject go)
    {
        if (go == player)
        {
            RemindPos.Instance.Hide();
            PromptManager.Instance.HidePN();

            ////发送打开知识点的消息
            //SocketManager.SendMsg(new NetModel(130));
            ////注册学习完成监听
            //SocketManager.RegisterMsgHandle(131, AddStudy);
            AddStudy();

        }
    }

    void AddStudy()
    {
        UIContainer.Instance.GetUI<UIChooseDocument>().Show();
    }

    /// <summary>
    /// 选择审核单据完成  给单据栏中添加多种单据
    /// </summary>
    public void OnChooseFinish()
    {
        PromptManager.Instance.Show("已接收需要审核的单据，可以在案例背景中查看，接下来将进行集装箱检查。", NotarizeType.Center, CheckingContainer);
    }

    /// <summary>
    /// 检查集装箱规格
    /// </summary>
    void CheckingContainer()
    {
        UIContainer.Instance.GetUI<UIChecking>().Show(1);
    }

    public void OpenDoor()
    {
        PromptManager.Instance.Show("确认集装箱检查信息后，将进行开箱检查。", NotarizeType.Center);

        StartCoroutine(ObRrotat(leftDoor.transform, 160, 3, BeOpen));
        StartCoroutine(ObRrotat(rightDoor.transform, -160, 3));

        leftDoor.GetComponent<BoxCollider>().enabled = false;
        rightDoor.GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// 集装箱门已经打开
    /// </summary>
    void BeOpen()
    {
        PromptManager.Instance.Show("集装箱门已经打开了,请点击唛头查看设备信息。", NotarizeType.Center, () =>
        {

            //点击描边提示
            mOutline = machine.GetComponent<MouseoverOutline>();
            mOutline.ShowOutLine();
            tventT = AddEventTrigger(machine, CheckingMarks);
        });
    }

    EventTrigger tventT;
    MouseoverOutline mOutline;

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
    /// 检查唛头开始
    /// </summary>
    /// <param name="bed"></param>
    void CheckingMarks(BaseEventData bed)
    {
        UIContainer.Instance.GetUI<UIChecking>().Show(3);

        if (tventT)
        {
            //取消点击描边提示
            if (mOutline)
            {
                mOutline.HideOutLine();
                mOutline.OnMExit();
            }

            //machine.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.000f);
            //取消鼠标放上提示
            //Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            tventT.triggers.Clear();
        }
    }

    public void CheckingMarksEnd()
    {
        StartCoroutine(ObRrotat(leftDoor.transform, -160, 3, () =>
        {
            leftDoor.GetComponent<BoxCollider>().enabled = true;
            rightDoor.GetComponent<BoxCollider>().enabled = true;

            PromptManager.Instance.Show("对检验合格的进口成套设备，施检部门在接到收货人编写的交接验收报告，并审核该报告的内容真实有效后出具《入境货物检验检疫证明》。", NotarizeType.Center, ShowKnowledge);
        }));
        StartCoroutine(ObRrotat(rightDoor.transform, 160, 3));

    }

    void ShowKnowledge()
    {
        //发送打开知识点的消息
        SocketManager.SendMsg(new NetModel(132));
        //注册学习完成监听
        SocketManager.RegisterMsgHandle(133, KnowledgeClose);

        //KnowledgeClose();
    }

    void KnowledgeClose()
    {
        PromptManager.Instance.Show("友情提示：货物进口后，检验人员仍需要对设备进行性能检验。", NotarizeType.Center, () => { ThisOnFinish(TaskState.任务成功); });
    }

    #region  s秒的时间  从当前角度转到 指定角度

    //首先转换出一秒转多少 然后是一帧转多少 
    IEnumerator ObRrotat(Transform t, float euler, float time, Action ac = null)
    {
        //一秒转过的度数
        float speed = euler / time;

        yield return StartCoroutine(StartRotat(t, speed, time));
        if (ac != null)
        {
            ac();
        }
    }

    IEnumerator StartRotat(Transform t, float speed, float time)
    {
        while (time - Time.deltaTime > 0)
        {
            time -= Time.deltaTime;

            t.rotation = Quaternion.Euler(t.rotation.eulerAngles.x, t.rotation.eulerAngles.y + speed * Time.deltaTime, t.rotation.eulerAngles.z);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    #endregion
}
