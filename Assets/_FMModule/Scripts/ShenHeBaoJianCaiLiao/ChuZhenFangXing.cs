using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityStandardAssets.Characters.ThirdPerson;

public class ChuZhenFangXing : TaskBase {

    public Transform StartPos;
    public Transform RPos;
    private GameObject player;
    public GameObject Comuputer;
    public GameObject Container;
    private bool canClickCp;

	
	void Awake()
	{
		TaskManager.Instance.RegisterTask(this);
	}
    // Use this for initialization
    void Start()
    {

        if (GameManager.IsNet)
        {

            if (GameManager._curTaskType == curTaskType)
            {
              //  Debuge.Log(StartPos.position);

                GameManager.NetPlayerStartPos = StartPos;
                NetGameManager.Instance.MultipleTasks.NetTaskAction += OnStart;
                //OnStart();
            }
        }
       // OnStart();
    }

    public override void OnStart()
    {
        base.OnStart();
        PromptManager.Instance.Show("施检部门将《入境货物检验检疫证明》上传至平台上，通关人员可以查看该单据，在平台上对这批货物进行放行。", NotarizeType.Center,()=>
        {
            //打开知识点
            SocketManager.SendMsg(new NetModel(134));
        });
        if (!GameManager.IsNet)
        {
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报检_出证放行);
            player.transform.position = StartPos.position;
        }
        SocketManager.RegisterMsgHandle(135, LearingEnd);
        SocketManager.RegisterMsgHandle(127, Finish);
       // LearingEnd();
    }
    //学习完毕回调
    private void LearingEnd()
    {
        RemindPos.Instance.Show(RPos.position, OnClickArrow);
        Container.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickArrow(GameObject obj)
    {
        if (GameManager.IsNet)
        {
            player = GameManager.MyPlayer.gameObject;
        }
        if (obj != null)
        {
            if (obj == player)
            {
                PointToObject.Instance.Show(Comuputer.transform.FindChild("Object002").position);
                Comuputer.GetComponent<MouseoverOutline>().ShowOutLine();
                RemindPos.Instance.Hide();
                canClickCp = true;  
            }
        }
    }
    /// <summary>
    /// 打开winform表格填写界面
    /// </summary>
    public void OnOpenWinformUI()
    {
        if (!canClickCp)
            return;
        PointToObject.Instance.Hide();
        canClickCp = false;
        SocketManager.SendMsg(new NetModel(126));
    }

    public void Finish()
    {
        PromptManager.Instance.Show("通关人员在系统上放行后，报检机构既可以从该平台上看到报检单的处理结果，也可根据实际情况自行打印所需单据。", NotarizeType.Center,
            () =>
            {
                //如果是网络版，则通知下一个人
                if (GameManager.IsNet)
                {
                   NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.NullTask);
                }
                Container.SetActive(false);
                base.OnFinish(TaskState.任务成功);
            });
    }
}
