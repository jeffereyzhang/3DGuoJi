using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.NetworkSystem;


namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager 
    {
        static short MsgKicked = MsgType.Highest + 1;
        static short MsgSetId = MsgType.Highest + 10;

        static public LobbyManager Instance;

        public float prematchCountdown = 5.0f;//倒计时时间
        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel TopPanel;

        public Transform MainMenuPanel;
        public Transform LobbyPanel;

        public LobbyInfoPanel InfoPanel;
        public LobbyCountdownPanel CountdownPanel;

        protected Transform currentPanel;

        public UIButton BackButton;

        public UILabel StatusInfo;
        //public UILabel HostInfo;

        public int PlayerNumber = 0;
        protected bool DisconnectServer = false;

        protected LobbyHook LobbyHooks;
        public NetworkDiscovery Discovery;
        private string roomName;
 
        public List<GameObject> PlayerPrefabList = new List<GameObject>();

        public LobbyPlayer MyLobbyPlayer;
        public int MyNetId;//服务器端给客户端设置的独一无二的网络id
        public Dictionary<int, SelectInfo> SelectHeroList = new Dictionary<int, SelectInfo>(); 
        public GameObject SelectHeroWin;
        void Start()
        {
            if (GameManager._curModuleType == ModuleType.进口报关流程)
            {
                playScene = "GameSence_BaoGuan";

            }
            else if (GameManager._curModuleType == ModuleType.进口报检流程)
            {
                playScene = "GameSence_BaoJian";
            }
            Instance = this;
            LobbyHooks = GetComponent<LobbyHook>();
            ChangeTo(MainMenuPanel);

            StarDiscoveryAsClient();
            SetServerInfo("", "");
          
        }

        private void Update()
        {
            //如果是存游戏中退出，则需要退出到主界面
            if (TopPanel.isInGame && SceneManager.GetSceneAt(0).name == "LobbyScene")
            {
                TopPanel.isInGame = false;
                BackMainScene();
            }
        }

        private void StarDiscoveryAsClient()
        {
            Discovery.Initialize();
            if (Discovery.running)
                Discovery.StopBroadcast();

            Discovery.Initialize();
            Discovery.StartAsClient();

        }

        //返回启动场景
        private void BackMainScene()
        {
            SocketManager.SendMsg(new NetModel(103));
            Destroy(gameObject);
            Application.LoadLevel("LoadSence");
        }

        //场景变换的时候调用
        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            base.OnLobbyClientSceneChanged(conn);
            ChangeTo(null);

            TopPanel.isInGame = true;
            TopPanel.ToggleVisibility(false);
            Discovery.StopBroadcast();
        }

        public void ChangeTo(Transform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != MainMenuPanel)
            {
               BackButton.gameObject.SetActive(true);
            }
            else
            {
                BackButton.gameObject.SetActive(false);
                SelectHeroWin.SetActive(false);
                SetServerInfo("", "");
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            InfoPanel.Display("努力链接中....", "取消", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            StatusInfo.text = status;
            //HostInfo.text = host;
        }
        

        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            if (backDelegate != null)
            {
                 backDelegate();
                //backDelegate = null;
            }
            
			//TopPanel.isInGame = false;
        }
        //进入主机
        public void OnClickJoin(string ip)
        {
            Instance.ChangeTo(LobbyPanel);

            networkAddress = ip;
            StartClient();

            backDelegate = StopClientClbk;
            DisplayIsConnecting();
            //SetServerInfo("Connecting...", networkAddress);
        }

        public void OnSendServerInfo(string roomName,bool isStartServer = false,bool isStartGame = false)
        {
            Debug.Log("向局域网发布信息");
            //向局域网发布信息
            if (Discovery.isServer || isStartServer)
            {
                this.roomName = roomName;
                SetServerInfo(roomName, networkAddress);
                string str = roomName + ":" + PlayerNumber + ":" + maxPlayers + ":" + isStartGame + ":"+GameManager._curModuleType;

                if (gameObject.activeSelf)
                {
                    if (!myLock)
                    {
                        StartCoroutine(RepeateServer(str));  
                    }
                }
                else
                {
                    StopAllCoroutines();
                }
            }
        }

        private bool myLock = false;
        //重启server,用以广播消息
        private IEnumerator RepeateServer(string str)
        {
            myLock = true;

            if (Discovery.running)
            {
                Discovery.StopBroadcast();
            }
            while (NetworkTransport.IsBroadcastDiscoveryRunning())
            {
                yield return null;
            }
            Discovery.broadcastData = str;

            Discovery.StartAsServer();
            myLock = false;
        }

        // ----------------- Server management

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(MainMenuPanel);
        }

        public void StopHostClbk()
        {
            StopHost();
            ChangeTo(MainMenuPanel);
            StarDiscoveryAsClient();
        }

        public void StopClientClbk()
        {
            StopClient();
            ChangeTo(MainMenuPanel);
            if(!Discovery.running)
            StarDiscoveryAsClient();
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }

        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            InfoPanel.Display("被踢！", "关闭", null);
            netMsg.conn.Disconnect();
        }

        //===================
        public override void OnStartHost()
        {
            ChangeTo(LobbyPanel);
            backDelegate = StopHostClbk;
            SelectHeroList.Clear();
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            PlayerNumber += count;
            
            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            OnSendServerInfo(roomName,false,TopPanel.isInGame);
        }

        // ----------------- Server callbacks ------------------
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);

            NetworkServer.SendToClient(conn.connectionId, MsgSetId, new IdMsg(conn.connectionId));
      //      Debug.LogError(conn.connectionId);


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

       public class IdMsg : MessageBase
        {
           public IdMsg()
           {
           }
            public IdMsg(int id)
            {
                this.id = id;
            }

            public int id;
        }
        //收到设置的id
        private void OnReciveId(NetworkMessage netMsg)
        {
            IdMsg idmsg = netMsg.ReadMessage<IdMsg>();
            MyNetId = idmsg.id;
            Debug.LogError(MyNetId + "MyNetId");
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("OnLobbyServerPlayerRemoved");
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            Debug.Log("OnLobbyServerPlayerRemoved");

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (LobbyHooks)
                LobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
			bool allready = true;
			for(int i = 0; i < lobbySlots.Length; ++i)
			{
				if(lobbySlots[i] != null)
					allready &= lobbySlots[i].readyToBegin;
			}

            if (allready)
            {
                OnSendServerInfo(roomName, false, true);
                //SlecHeroWin.SetActive(true);
                //StartCoroutine(ServerCountdownCoroutine()); 
                for (int i = 0; i < lobbySlots.Length; ++i)
                {
                    if (lobbySlots[i] != null)
                    {
                        (lobbySlots[i] as LobbyPlayer).RpcOpenSelectHeroWin();
                    }
                }
            }
        }
        //倒计时
        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {
                    //to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {
                            //there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }
            ServerChangeScene(playScene);
            
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            //this.Conn = conn;
            InfoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);
            conn.RegisterHandler(MsgSetId, OnReciveId);

            if (!NetworkServer.active)
            {
                //only to do on pure client (not self hosting client)
                ChangeTo(LobbyPanel);
                backDelegate = StopClientClbk;
                //SetServerInfo("", networkAddress);
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(MainMenuPanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(MainMenuPanel);
            InfoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }

        public override GameObject OnLobbyServerCreateGamePlayer
            (NetworkConnection conn, short playerControllerId)
        {
            int index = SelectHeroList[conn.connectionId].HasSelected;
            GameObject _temp = Instantiate(spawnPrefabs[index]);

            NetworkServer.AddPlayerForConnection(conn, _temp, playerControllerId);
            return _temp;
        }
    }
}
