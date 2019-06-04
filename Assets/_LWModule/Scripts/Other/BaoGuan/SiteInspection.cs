using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 现场查验
/// </summary>
public class SiteInspection : TaskBase
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

    public GameObject site;

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
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报关_现场查验);
            player.transform.position = target.position;

            CameraCtrl.Instance.SetCameraTarget(player.transform);
        }

        site.SetActive(false);

        StartSite();
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
            NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报关_通关放行);
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
    /// 获取鼠标描边效果
    /// </summary>
    bool GetMouseoverOutline(GameObject go ,out MouseoverOutline mo)
    {
        mo = go.GetComponent<MouseoverOutline>();
        if (mo != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 开始现场查验
    /// </summary>
    void StartSite()
    {
        EndStudy();
    }

    void EndStudy()
    {
        PromptManager.Instance.Show("到了预约查验时间这一天，报关员陪同海关查验人员到港口查验区进行查验。", NotarizeType.Center, InspectionProcess);
    }

    void InspectionProcess()
    {
        //发送学习海关查验和检疫查验的区别消息
        SocketManager.SendMsg(new NetModel(172));
        //注册海关查验和检疫查验的区别学习完成监听
        SocketManager.RegisterMsgHandle(173, ProcessEnd);

    }

    void ProcessEnd()
    {
        PromptManager.Instance.Show("请根据提示前往目的地!", NotarizeType.Center);
        //显示箭头指引
        RemindPos.Instance.Show(taskPos[0].position, ArrivePosition);
    }

    /// <summary>
    /// 需要点击的设备
    /// </summary>
    public GameObject machine;
    MouseoverOutline mOutline;

    void ArrivePosition(GameObject go)
    {
        if (GameManager.IsNet)
        {
            player = GameManager.MyPlayer.gameObject;
        }

        if (go == player)  //如果是当前角色
        {
            RemindPos.Instance.Hide();
           // PromptManager.Instance.HidePN();
            PromptManager.Instance.Show("点击货物查看货物的唛头信息是否正确。", NotarizeType.Center, () =>{
                tventT = AddEventTrigger(machine, CheckingMarks);
                //点击描边提示
                mOutline = machine.GetComponent<MouseoverOutline>();
                mOutline.ShowOutLine();
                //machine.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.005f);
            });
        }
    }

    /// <summary>
    /// 检查唛头开始
    /// </summary>
    /// <param name="bed"></param>
    void CheckingMarks(BaseEventData bed)
    {
        if (tventT)
        {
            ////取消点击描边提示
            //machine.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.000f);
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

        UIContainer.Instance.GetUI<UIChecking>().Show(4);
    }

    public GameObject leftDoor;
    public GameObject rightDoor;

    /// <summary>
    /// 关闭箱门
    /// </summary>
    public void CloseTheDoor()
    {
        leftDoor.GetComponent<BoxCollider>().enabled = false;
        rightDoor.GetComponent<BoxCollider>().enabled = false;

        StartCoroutine(ObRrotat(leftDoor.transform, -160, 3, HasOpen));
        StartCoroutine(ObRrotat(rightDoor.transform, 160, 3));
    }

    /// <summary>
    /// 打开之后提示让拖动铅封图标到箱门
    /// </summary>
    void HasOpen()
    {
        leftDoor.GetComponent<BoxCollider>().enabled = true;
        rightDoor.GetComponent<BoxCollider>().enabled = true;
        PromptManager.Instance.Show("选中铅封图标并拖动至箱门上，锁上铅封。", NotarizeType.Center, () =>
        {
            UIContainer.Instance.GetUI<UIDragDocument>().Show();

            //点击描边提示
            mOutline = rightDoor.GetComponent<MouseoverOutline>();
            if (mOutline)
            {
                mOutline.ShowOutLine();
            }


            //rightDoor.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.005f);
        });
    }

    /// <summary>
    /// 提示流程完成
    /// </summary>
    public void EndSite()
    {
        //取消点击描边提示
        if (mOutline)
        {
            mOutline.HideOutLine();
            mOutline.OnMExit();
        }

        //rightDoor.GetComponent<MeshRenderer>().materials[1].SetFloat("_Outline", 0.000f);
        site.SetActive(true) ;
        PromptManager.Instance.Show("铅封已锁上，请返回工作单位，将检查结果上传至平台，供申报单位和海关查看。", NotarizeType.Center, () =>
        {
            ThisOnFinish(TaskState.任务成功);
        });
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
