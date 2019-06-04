using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityStandardAssets.Characters.ThirdPerson;

public class ShenHeBaoJian : TaskBase
{

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
	void Start ()
	{
        if (GameManager.IsNet)
        {
            if (GameManager._curTaskType == curTaskType)
            {
                GameManager.NetPlayerStartPos = StartPos;

                NetGameManager.Instance.MultipleTasks.NetTaskAction += OnStart;
                //test
               // Debuge.Log("ShenHeBaoJian");
               // OnStart();
            }
        }
	}

    public override void OnStart()
    {
        base.OnStart();
        PromptManager.Instance.Show("申报单位将所需资料通过电子平台传递到检验检疫机构，检务员审核这批货物的报检材料。", NotarizeType.Center);
        RemindPos.Instance.Show(RPos.position, OnClickArrow);

        if(!GameManager.IsNet)
        {
            player = StandAlonePlayerManager.Instance.GetPlayer(TaskType.进口报检_审核报检材料);
            player.transform.position = StartPos.position;
        }
        SocketManager.RegisterMsgHandle(127, Finish);
        Container.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

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
        if(!canClickCp)
            return;
        PointToObject.Instance.Hide();
        canClickCp = false;
        SocketManager.SendMsg(new NetModel(126));
    }

    public void Finish()
    {
        PromptManager.Instance.Show("检务机构审核通过后，确认查验方式为“现场检验”，这些结果返回到报检机构，报检机构根据该结果安排人员进行检验检疫相应事宜。", NotarizeType.Center,
            () =>
            {
                //如果是网络版，则通知下一个人
                if (GameManager.IsNet)
                {
                    NetGameManager.Instance.MultipleTasks.SendInfoToNextTask(TaskType.进口报检_实施检验检疫);
                }
                Container.SetActive(false);
                base.OnFinish(TaskState.任务成功);
            });
    }
}
