using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby
{
    public class LobbyServerEntry : MonoBehaviour 
    {
        public UILabel ServerInfoText;
        public UILabel SlotInfo;
        public UIButton JoinButton;
        public GameObject IsStartLable;
        public UILabel GameTypeLabel;

        public void Populate(NetworkBroadcastResult match)
		{
            string[] infos = BytesToString(match.broadcastData).Split(':');
            ServerInfoText.text = infos[0];
            SlotInfo.text = infos[1] + "/" + infos[2];
            IsStartLable.SetActive(Boolean.Parse(infos[3]));

            ModuleType moduleType = (ModuleType)Enum.Parse(typeof(ModuleType), infos[4]);//房间类型
            SetGameType(moduleType);
            JoinButton.onClick.Clear();
            EventDelegate.Add(JoinButton.onClick, () =>
            {
                if (!IsStartLable.activeSelf)
                {
                    if (GameManager._curModuleType != moduleType)
                    {
                        LobbyManager.Instance.InfoPanel.Display("房间类型和您选的模块不符！", "确定");
                        return;
                    }

                    if (int.Parse(infos[1]) < int.Parse(infos[2]))
                    {
                        LobbyManager.Instance.OnClickJoin(match.serverAddress);
                        LobbyManager.Instance.SetServerInfo(infos[0],"");
                    }
                    else
                    {
                        LobbyManager.Instance.InfoPanel.Display("房间已满", "确定");
                    }  
                }
                else
                {
                    LobbyManager.Instance.InfoPanel.Display("该房间游戏已经开始，请加入其它房间", "确定"); 
                }
            });
        }
        string BytesToString(byte [] btyes)
        {
            char[] chars = new char[btyes.Length / sizeof(char)];
            System.Buffer.BlockCopy(btyes, 0, chars, 0, btyes.Length);
            string str = new string(chars);
            return str;
        }

        private void SetGameType(ModuleType type)
        {
            if (type == ModuleType.进口报检流程)
            {
                GameTypeLabel.text = "进口报检";
            }
            else if(type == ModuleType.出口报关流程)
            {
                GameTypeLabel.text = "进口报关";
            }
        }
    }

}