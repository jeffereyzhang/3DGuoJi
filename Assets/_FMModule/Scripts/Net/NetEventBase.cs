using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetEventBase : NetworkBehaviour
{
    public NetworkClient client;
    public bool isSetup = false;
    public short MsgId;

    public LobbyManager net;

    // Use this for initialization
    public virtual void OnStart()
    {
        if (!isSetup)
        {
            SetupClient();
        }
    }
    public virtual void SetupClient()
    {
        net = LobbyManager.Instance;
        client = net.client;
        isSetup = true;
        if (isServer)
        {
            NetworkServer.RegisterHandler(MsgId, OnReciveMessage);
        }
        else
        {
            client.RegisterHandler(MsgId, OnReciveMessage);
        }
    }
    public virtual void OnReciveMessage(NetworkMessage netMsg)
    {

    }

}