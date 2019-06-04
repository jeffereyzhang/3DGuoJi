using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
public class SocketManager : MonoBehaviour
{
    public static MyTcpClient _tcpClient;
    public static List<NetModel> reciveMsgList = new List<NetModel>();

    public static Dictionary<int, Action> clientMsgHandleDic = new Dictionary<int, System.Action>();
    public static Dictionary<int, MsgDelegate> MessageHandleDic = new Dictionary<int, MsgDelegate>();


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 注册消息ID对应的事件
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="handleAction"></param>
    public static void RegisterMsgHandle(int messageID, Action handleAction)
    {
        if (!clientMsgHandleDic.ContainsKey(messageID))
        {
            clientMsgHandleDic.Add(messageID, handleAction);
        }
        else
        {
            Debuge.Log("此ID的消息已经注册！id = " + messageID);
        }
    }

    /// <summary>
    /// 注册消息ID对应的事件
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="handleAction"></param>
    public static void RegisterMsgHandle(int messageID, MsgDelegate handleAction)
    {
        if (!MessageHandleDic.ContainsKey(messageID))
        {
            MessageHandleDic.Add(messageID, handleAction);
        }
        else
        {
            Debuge.Log("此ID的消息已经注册！id = " + messageID);
        }
    }


    /// <summary>
    /// 移除消息ID对应的事件
    /// </summary>
    /// <param name="messageID"></param>
    /// <param name="handleAction"></param>
    public static void RemoveMsgHandle(int messageID)
    {
        if (MessageHandleDic.ContainsKey(messageID))
        {
            MessageHandleDic.Remove(messageID);
        }
        if (clientMsgHandleDic.ContainsKey(messageID))
        {
            clientMsgHandleDic.Remove(messageID);
        }
    }


    /// <summary>
    /// 接收到服务端的消息
    /// </summary>
    /// <param name="msgModel"></param>
    public static void MsgDistributeNetModel(NetModel msgModel)
    {
        if (msgModel.ID == 0)
            return;
        reciveMsgList.Add(msgModel);
        Debug.LogError("接收到winform的消息，ID = " + msgModel.ID);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msgModel"></param>
    public static void SendMsg(NetModel msgModel)
    {
        if (_tcpClient == null)
        {
            _tcpClient = GameObject.Find("SocketManager").GetComponent<MyTcpClient>();
        }
        _tcpClient.SendMessage(ProtoBufUtils.SerializeAutoGZip(msgModel));
    }

    void Update()
    {
        if (reciveMsgList.Count > 0)
        {
            if (clientMsgHandleDic.ContainsKey(reciveMsgList[0].ID))
            {
                clientMsgHandleDic[reciveMsgList[0].ID]();//执行事件ID对应的事件方法
            }

            if (MessageHandleDic.ContainsKey(reciveMsgList[0].ID))
            {
                MessageHandleDic[reciveMsgList[0].ID](reciveMsgList[0]);//执行事件ID对应的事件方法,并返，转为了object
            }

            reciveMsgList.RemoveAt(0);//分发完成后移除掉第一个已经分发的消息
        }
    }

    void OnApplicationQuit()
    {
        
        SocketManager.SendMsg(new NetModel(105));
        if (_tcpClient == null)
        {
            _tcpClient = GameObject.Find("SocketManager").GetComponent<MyTcpClient>();
        }
        _tcpClient.StopSocket();
    }




}

