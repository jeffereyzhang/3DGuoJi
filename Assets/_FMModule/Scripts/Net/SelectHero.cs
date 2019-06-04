using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class SelectHero : MonoBehaviour
{
    private LobbyPlayer player;
    public bool isSetup = false;
    public short MsgId;

    public LobbyManager net;

    public UIButton[] HeroBtn;

    public GameObject Mask;

	// Use this for initialization
    void OnEnable() 
    {
        SetupClient();
        Init();
        GameManager.ClearOtherPlayerInfo();
    }

    public GameObject BaoJianGrid;
    public GameObject BaoGuanGrid;
    private void Init()
    {
        if (GameManager._curModuleType == ModuleType.进口报关流程)
        {
            BaoGuanGrid.SetActive(true);
            BaoJianGrid.SetActive(false);
        }
        else if (GameManager._curModuleType == ModuleType.进口报检流程)
        {
            BaoGuanGrid.SetActive(false);
            BaoJianGrid.SetActive(true);
        }
        Mask.SetActive(false);
        for (int i = 0; i < HeroBtn.Length; i++)
        {
            HeroBtn[i].onClick.Clear();
            UIEventListener.Get(HeroBtn[i].gameObject).onClick = OnSelectHero;
            HeroBtn[i].GetComponent<BoxCollider>().enabled = true;
            HeroBtn[i].transform.FindChild("HasSelectFlag").gameObject.SetActive(false);
        }
    }

    public void SetupClient()
    {
        MsgId = MsgType.Highest + 3;

        net = LobbyManager.Instance;
        player = net.MyLobbyPlayer;
        isSetup = true;

        if (player.isServer)
        {
            NetworkServer.RegisterHandler(MsgId, OnReciveMessage);
        }
        else
        {
            player.connectionToServer.RegisterHandler(MsgId, OnReciveMessage);
        }
    }
    // Update is called once per frame
	void Update () {
	
	}

    private void OnSelectHero(GameObject btn)
    {
        string roleName = "";
        SelectInfo info = new SelectInfo();
        switch (btn.name)
        {
                //报检
            case "YeWuYuan":
                roleName = "业务员";
                info.HasSelected = 0;
                GameManager._curTaskType = TaskType.进口报检_委托报检;
                Debuge.Log("123123123123123123123");
                break;
            case "BaoJianYuan":
                roleName = "报检员";
                info.HasSelected = 1;
                GameManager._curTaskType = TaskType.进口报检_电子申报;
                break;
            case "JianWuYuan":
                roleName = "检务员";
                info.HasSelected = 2;
                GameManager._curTaskType = TaskType.进口报检_审核报检材料;;
                break;
            case "JianYanYuan":
                roleName = "检验员";
                info.HasSelected = 3;
                GameManager._curTaskType = TaskType.进口报检_实施检验检疫;;
                break;
            case "TongGuanYuan":
                roleName = "通关员";
                info.HasSelected = 4;
                GameManager._curTaskType = TaskType.进口报检_出证放行;
                break;
                //报关
            case "BGYeWuYuan":
                roleName = "业务员";
                info.HasSelected = 5;
                GameManager._curTaskType = TaskType.进口报关_委托报关; ;
                break;
            case "BGBaoGuanYuan":
                roleName = "报关员";
                info.HasSelected = 6;
                GameManager._curTaskType = TaskType.进口报关_电子申报;
                break;
            case "BGShenDanYuan":
                roleName = "审单员";
                info.HasSelected = 7;
                GameManager._curTaskType = TaskType.进口报关_审核单证; ;
                break;
            case "BGChaYanYuan":
                roleName = "查验员";
                info.HasSelected = 8;
                GameManager._curTaskType = TaskType.进口报关_现场查验; ;
                break;
            case "BGTongGuanYuan":
                roleName = "通关员";
                info.HasSelected = 9;
                GameManager._curTaskType = TaskType.进口报关_通关放行;
                break;
        }
        //告诉winform端自己选了什么角色
        NetModel nm = new NetModel(108);
        nm.MessageContent = new ProtoObject(GameManager._curTaskType);
        
        //SocketManager.SendMsg(nm);
        //告诉其他人自己已经选了这个角色
        Mask.SetActive(true);

        info.RoleName = roleName;
        info.Id = LobbyManager.Instance.MyNetId;
        info.Number = GameManager.PlayerNumber;
        info.Name = player.playerName;
      
        player.connectionToServer.Send(MsgId, info);
    }



    public  void OnReciveMessage(NetworkMessage netMsg)
    {
        SelectInfo msg = netMsg.ReadMessage<SelectInfo>();

        if (player.isClient)
        {
            HeroBtn[msg.HasSelected].GetComponent<BoxCollider>().enabled = false;
            GameObject go = HeroBtn[msg.HasSelected].transform.FindChild("HasSelectFlag").gameObject;
            go.SetActive(true);
            go.GetComponent<UILabel>().text = msg.Name;
            go.GetComponent<TweenLetters>().PlayForward();

            //将所有的角色信息保存到一个字典里边，后续用
            GameManager.AddOtherPlayerInfo(msg.RoleName, msg);
        }
        //服务器转播消息
        if (player.isServer)
        {
            if (!LobbyManager.Instance.SelectHeroList.ContainsKey(msg.Id))
            LobbyManager.Instance.SelectHeroList.Add(msg.Id,msg);

            NetworkServer.SendToAll(MsgId, msg);

            if (LobbyManager.Instance.SelectHeroList.Count == LobbyPlayerList.Instance._players.Count)
            {
                StartCoroutine(LobbyManager.Instance.ServerCountdownCoroutine());
            }
        }
    }
}

public class SelectInfo : MessageBase
{
    public int HasSelected;
    public int Id;//网络连接ID
    public string Name;//姓名
    public string Number;//学号
    public string RoleName;//所选的角色名
    public SelectInfo()
    {

    }
}
