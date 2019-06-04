using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    public class LobbyMainMenu : MonoBehaviour
    {
        private LobbyManager lobbyManager;

        public Transform lobbyPanel;
        public UIInput RoomNameInput;


        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
           // lobbyManager.TopPanel.ToggleVisibility(true);
        }

        public void OnClickHost()
        {
            string str = string.IsNullOrEmpty(RoomNameInput.text) ? "决战到天亮" : RoomNameInput.text;
            if (str.Length > 15)
            {
                str = str.Substring(0, 15); 
            }
           lobbyManager.OnSendServerInfo(str, true);
           lobbyManager.StartHost();
           lobbyManager.InfoPanel.Display("创建中...", "取消", null);
        }
    }
}
