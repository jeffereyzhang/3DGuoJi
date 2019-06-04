using UnityEngine;
using System.Collections;

public class CommissionInspection : TaskBase
{
    private GameObject bussinessPlayer;
    public Transform standToComputerPos;
    public Transform pointToComputer;
    public GameObject Comuputer;
    public GameObject Container;
    private bool canClickCp;

    void Awake()
    {
        TaskManager.Instance.RegisterTask(this);
    }
    void Start()
    {


        if (GameManager.IsNet)//网络版
        {
            if (GameManager._curTaskType == curTaskType)
            {
                NetGameManager.Instance.MultipleTasks.NetTaskAction += OnStart;
                GameManager.NetPlayerStartPos = missionStartPos;
            }
        }
        else//单机版
        {
            //OnStart();
        }

    }

    public override void OnStart()
    {
        base.OnStart();
        PromptManager.Instance.Show("中国中福实业总公司将报检业务涉及的[ff0000]所有事宜[-]均委托给宁波汉德通关服务有限公司，现需要填写一份[ff0000]有效期到2017年9月30日[-]的代理报检委托书。", NotarizeType.Center);
        RemindPos.Instance.Show(standToComputerPos.position, OnClickArrow);
        Debuge.Log("RemindPos.Instance.Show......");

        if (!GameManager.IsNet)
        {
            bussinessPlayer = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报检_委托报检);
            bussinessPlayer.transform.position = missionStartPos.position;

            CameraCtrl.Instance.SetCameraTarget(bussinessPlayer.transform);
        }

        SocketManager.RegisterMsgHandle(123, CommissionInspectionBookFinish);
        Container.SetActive(true);
    }

    private void CommissionInspectionBookFinish()
    {
        PromptManager.Instance.Show("业务员需要整理该批货物报检所需的各类单据通过快递发送给宁波汉德通关服务有限公司，静待其审核并接受委托。", NotarizeType.Center,
           () =>
           {
               //如果是网络版，则通知下一个人
               if (GameManager.IsNet)
               {
                   NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报检_电子申报);
               }
               else
               {
                   Container.SetActive(false);
                   base.OnFinish(TaskState.任务成功);
               }
           });
    }

    void Update()
    {

    }

    public void OnClickArrow(GameObject obj)
    {
        //if (obj != null)
        //{
        PointToObject.Instance.Show(pointToComputer.position);
        Comuputer.GetComponent<MouseoverOutline>().ShowOutLine();
        RemindPos.Instance.Hide();
        canClickCp = true;
        //}
    }

    public void OnOpenWinformUI()
    {
        if (!canClickCp)
            return;
        PointToObject.Instance.Hide();
        SocketManager.SendMsg(new NetModel(122));
        canClickCp = false;
    }

}
