using System;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MultipleTaskFlow : NetEventBase
{
    public Action NetTaskAction;
    public ChatSystem MyCahChatSystem;

    // Use this for initialization
    public  void Start()
    {
        if (!GameManager.IsNet)
        {
            gameObject.SetActive(false);
            return;
        }
        MyCahChatSystem = FindObjectOfType<ChatSystem>();
        base.OnStart();
    }



    public override void SetupClient()
    {
        MsgId = MsgType.Highest + 4;
        base.SetupClient();
    }

    public override void OnReciveMessage(NetworkMessage netMsg)
    {
        FlowInfo msg = netMsg.ReadMessage<FlowInfo>();

        //服务器转播消息
        if (isServer)
        {
            NetworkServer.SendToAll(MsgId, msg);
        }
        //客户端接受消息
        if (client.isConnected)
        {
            Debug.Log(GameManager._curTaskType + "." + msg.TaskFlow);
            if (GameManager._curTaskType == msg.TaskFlow)
            {
                //启动任务
                if (NetTaskAction != null)
                {
                    
                    NetTaskAction();
                    NetTaskAction = null;
                }
            }
            else if (msg.TaskFlow != TaskType.NullTask)
            {
                Debug.Log(msg.TaskFlow.ToString());
                //PromptManager.Instance.Show(((Enum)(msg.TaskFlow-1))+"任务完成,开始"+ msg.TaskFlow+"任务。",NotarizeType.Center);
               // [99ff00]系统消息[-]
                MyCahChatSystem.SendTaskFlow("[99ff00]" + ((Enum)(msg.TaskFlow - 1)) + "[-]任务完成,开始[99ff00]" + msg.TaskFlow + "[-]任务。");
            }
        }
        //总的结束界面出现
        if (msg.TaskFlow == TaskType.NullTask)
        {
            //显示结束界面
            UIContainer.Instance.GetUI<UITaskFinish>().SetTaskFinishContent(GameManager._curTaskType,100, () =>
            {
                //SocketManager.SendMsg(new NetModel(103));
                LobbyManager.Instance.GoBackButton();
            });
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    if (GameManager._curTaskType == TaskType.进口报检_出证放行 || GameManager._curTaskType == TaskType.进口报关_通关放行)
        //    {
        //        SendInfoToNextTask(TaskType.NullTask);
        //    }
        //    else
        //    {
        //        SendInfoToNextTask(GameManager._curTaskType + 1);
        //    }
        //}
    }

    
    /// <summary>
    /// 自己任务完成，给下一个任务发消息
    /// </summary>
    public void SendInfoToNextTask(TaskType taksFlow)
    {
        FlowInfo info = new FlowInfo();
        info.TaskFlow = taksFlow;
        client.Send(MsgId, info);
    }
    public class FlowInfo : MessageBase
    {
        public TaskType TaskFlow;

        public FlowInfo()
        {

        }
    }
}

