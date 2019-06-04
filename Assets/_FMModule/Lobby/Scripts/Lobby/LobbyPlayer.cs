using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        public UIInput NameInput;
        public UIButton ReadyButton;
        public GameObject WaitingPlayerLabel;
        public UIButton RemovePlayerButton;

        public GameObject localIcone;
        public GameObject remoteIcone;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";

        //void Awake()
        //{
        //    DontDestroyOnLoad(transform.gameObject);
        //}
        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.Instance != null) 
                LobbyManager.Instance.OnPlayersNumberModified(1);

            LobbyPlayerList.Instance.AddPlayer(this);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
        }

        public override void OnStartAuthority()
        {
           base.OnStartAuthority();
           SetupLocalPlayer();
        }

        void SetupOtherPlayer()
        {
            NameInput.enabled = false;
            RemovePlayerButton.enabled = NetworkServer.active;

            ReadyButton.transform.GetChild(0).GetComponent<UILabel>().text = "...";
            ReadyButton.enabled = false;
            ReadyButton.state = UIButtonColor.State.Disabled;

          //  OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            LobbyManager.Instance.MyLobbyPlayer = this;
            NameInput.enabled = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);
            GetComponent<UISprite>().spriteName = "xzt";

            CheckRemoveButton();

            ReadyButton.transform.GetChild(0).GetComponent<UILabel>().text = "准备";
            ReadyButton.enabled = true;

            //给玩家一个默认的名字
            if (playerName == "")
                CmdNameChanged(GameManager.studentName);

            NameInput.enabled = true;
            NameInput.onSubmit.Clear();
            global::EventDelegate.Add(NameInput.onSubmit, OnNameChanged);

            ReadyButton.onClick.Clear();
            global::EventDelegate.Add(ReadyButton.onClick, OnReadyClicked);


            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            //if (LobbyManager.s_Singleton != null)
            //    LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            RemovePlayerButton.enabled = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {

                UILabel textComponent = ReadyButton.transform.GetChild(0).GetComponent<UILabel>();
                textComponent.text = "已准备";
                ReadyButton.enabled = false;
                NameInput.enabled = false;
            }
            else
            {
                UILabel textComponent = ReadyButton.transform.GetChild(0).GetComponent<UILabel>();
                textComponent.text = isLocalPlayer ? "准备" : "未准备";
                textComponent.color = Color.white;
                ReadyButton.enabled = isLocalPlayer;
                NameInput.enabled = isLocalPlayer;
            }
        }


        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            NameInput.value = playerName;
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged()
        {
            CmdNameChanged(NameInput.value);
        }

        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
                LobbyManager.Instance.KickPlayer(connectionToClient);
                
        }

        public void ToggleJoinButton(bool enabled)
        {
            ReadyButton.gameObject.SetActive(enabled);
            WaitingPlayerLabel.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            //倒计时
            LobbyManager.Instance.CountdownPanel.Label.text = countdown.ToString();
            LobbyManager.Instance.CountdownPanel.gameObject.SetActive(countdown != 0);
        }
        [ClientRpc]
        public void RpcOpenSelectHeroWin()
        {
            LobbyManager.Instance.SelectHeroWin.gameObject.SetActive(true);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command
      
        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            LobbyPlayerList.Instance.RemovePlayer(this);
            if (LobbyManager.Instance != null) LobbyManager.Instance.OnPlayersNumberModified(-1);
        }
    }
}
