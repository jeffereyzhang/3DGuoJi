using UnityEngine.Networking;

public class ChatSystem : NetEventBase {

    public UITextList TextList;
    public UIInput MyChatInput;

    public UILabel LastTextInfo;
    public UIButton SendBtn;
    private int  connectionID = -1;
	// Use this for initialization
    public  void  Start ()
	{
        if (!GameManager.IsNet)
        {
            gameObject.SetActive(false);
            return;
        }
        base.OnStart();
	    global::EventDelegate.Add(MyChatInput.onSubmit, OnChatSubmit);
        global::EventDelegate.Add(SendBtn.onClick, OnChatSubmit);
	}
    //和某人私聊
    public void ChatToSb(int conId,string name)
    {
        connectionID = conId;
        MyChatInput.value = "@" + name;
    }

    public void OnChatSubmit()
    {
        string text = NGUIText.StripSymbols(MyChatInput.value);
        if (text.Length > 40f)
        {
            return;
        }
        if (!string.IsNullOrEmpty(text))
        {
            MyChatInput.isSelected = false;
            ChatInfo info = new ChatInfo(GameManager.MyPlayer.Name, text);
            client.Send(MsgId, info); // 客户端向服务器发送消息
            MyChatInput.value = "";
        }
    }

    public  void SendTaskFlow(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            MyChatInput.isSelected = false;
            ChatInfo info = new ChatInfo("[99ff00]系统消息[-]", text);
            client.Send(MsgId, info); // 客户端向服务器发送消息
        }
    }


    public override void SetupClient()
    {
        MsgId = MsgType.Highest + 2;
        base.SetupClient();
    }

    public override void OnReciveMessage(NetworkMessage netMsg)
    {
        ChatInfo msg = netMsg.ReadMessage<ChatInfo>();

        //服务器转播消息
        if (isServer)
        {
            if (connectionID == -1)
            {
                NetworkServer.SendToAll(MsgId, msg);
            }
            else
            {
                NetworkServer.SendToClient(connectionID, MsgId, msg);
                connectionID = -1;
            }
        }
        //客户端接受消息
        if (client.isConnected)
        {
            string t = msg.Sender + ":" + msg.Text;
            TextList.Add(t);
            LastTextInfo.text = t;
        }
    }

    private void OnDestroy()
    {
        TextList.Clear();
    }
}
public class ChatInfo : MessageBase
{
    public string Sender;
    public string Text;
    public bool PassFroServer = false;

    public ChatInfo()
    {
    }
    public ChatInfo(string sender, string content)
    {
        Sender = sender;
        Text = content;
    }
}
