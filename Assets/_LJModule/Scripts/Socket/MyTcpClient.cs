using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.IO;




public class MyTcpClient : MonoBehaviour
{
    List<byte> cache = new List<byte>();
    //List<SocketModel> messages = new List<SocketModel>();
    List<NetModel> messages2 = new List<NetModel>();
    const int portNo = 64587;
    //const int portNo = 64599;//加密服务器用
    private TcpClient _client;
    byte[] data;
    string Error_Message;
    /// <summary>
    /// 连接到服务器
    /// </summary>
    void Start()
    {
        try
        {
            this._client = new TcpClient();
            //this._client.Connect("127.0.0.1", portNo);
            this._client.Connect("127.0.0.1", portNo);
            data = new byte[this._client.ReceiveBufferSize];
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
            Debug.LogError("客户端连接上了.....");
        }
        catch (Exception ex)
        {
            Debug.LogError("客户端连接socket存在异常：" + ex.Message);
        }
    }


    void Update()
    {

    }

    public void StopSocket()
    {
        if (this._client != null)
        {
            this._client.Close();
        }
    }


    public void SendMessage(byte[] message)
    {
        try
        {
            NetworkStream ns = this._client.GetStream();
            ns.Write(message, 0, message.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
            Error_Message = ex.Message;
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="message"></param>
    public new void SendMessage(string message)
    {
        try
        {
            NetworkStream ns = this._client.GetStream();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
            Error_Message = ex.Message;
        }
    }
    private void ReceiveMessage(IAsyncResult ar)
    {
        Debug.LogError("ReceiveMessage......");

        try
        {
            //清空errormessage
            Error_Message = "";
            int bytesRead;
            bytesRead = this._client.GetStream().EndRead(ar);
            if (bytesRead < 1)
            {
                return;
            }
            else
            {
                byte[] message = new byte[bytesRead];

                Buffer.BlockCopy(data, 0, message, 0, bytesRead);
                //尾递归 再次开启异步消息接收 消息到达后会直接写入 缓冲区 readbuff
                this._client.GetStream().BeginRead(data, 0, 1024, ReceiveMessage, data);

                Debug.Log("data bytesRead jieguo  = " + System.Text.Encoding.ASCII.GetString(message, 0, bytesRead));
                NetModel result = (NetModel)ProtoBufUtils.DeserializeAutoGZip(message, typeof(NetModel));
                Debug.Log("result = " + result.ID);
                SocketManager.MsgDistributeNetModel(result);//收到消息后转发
            }
        }
        catch (Exception ex)
        {
            Error_Message = ex.Message;
            Debug.LogWarning(Error_Message);
            this._client.Close();
        }

    }

    //缓存中有数据处理
    void onData()
    {
        Debug.LogWarning("onData");
        //长度解码
        byte[] result = decode(ref cache);

        //长度解码返回空 说明消息体不全，等待下条消息过来补全
        if (result == null)
        {
            Debug.Log("result == null......");
            return;
        }
        //SocketModel message = mdecode(result);

        NetModel message = mdecode2(result);

        if (message == null)
        {
            return;
        }
        Debug.LogWarning("onData2222");
        //进行消息的处理
        //messages.Add(message);
        messages2.Add(message);
        //尾递归 防止在消息处理过程中 有其他消息到达而没有经过处理
        onData();
    }
    public static byte[] decode(ref List<byte> cache)
    {
        if (cache.Count < 4)
            return null;
        MemoryStream ms = new MemoryStream(cache.ToArray());//创建内存流对象，并将缓存数据写入进去
        BinaryReader br = new BinaryReader(ms);//二进制读取流
        int length = br.ReadInt32();//从缓存中读取int型消息体长度
        //如果消息体长度 大于缓存中数据长度 说明消息没有读取完 等待下次消息到达后再次处理
        if (length > ms.Length - ms.Position)
        {
            Debug.Log("length > ms.Length - ms.Position = " + length + " .ms.Position = " + ms.Position + " .ms.Length = " + ms.Length);
            return null;
        }
        //读取正确长度的数据
        byte[] result = br.ReadBytes(length);
        //清空缓存
        cache.Clear();
        //将读取后的剩余数据写入缓存
        cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
        br.Close();
        ms.Close();
        return result;
    }

    public static SocketModel mdecode(byte[] value)
    {
        ByteArray ba = new ByteArray(value);
        SocketModel model = new SocketModel();
        byte type;
        int area;
        int command;
        //从数据中读取 三层协议  读取数据顺序必须和写入顺序保持一致
        ba.read(out type);
        ba.read(out area);
        ba.read(out command);
        model.type = type;
        model.area = area;
        model.command = command;
        //判断读取完协议后 是否还有数据需要读取 是则说明有消息体 进行消息体读取
        if (ba.Readnable)
        {
            byte[] message;
            //将剩余数据全部读取出来
            ba.read(out message, ba.Length - ba.Position);
            //反序列化剩余数据为消息体
            //model.message = SerializeUtil.decode(message);
            model.message = ProtoBufUtils.DeserializeAutoGZip(message, typeof(String));
            Debug.Log(" message.area = " + model.area + " message.command = " + model.command + " message.message = " + model.message);

        }
        ba.Close();
        return model;
    }

    public static NetModel mdecode2(byte[] value)
    {
        //ByteArray ba = new ByteArray(value);
        NetModel model = new NetModel();
        //byte type;
        //int area;
        //int command;
        ////从数据中读取 三层协议  读取数据顺序必须和写入顺序保持一致
        //ba.read(out type);
        //ba.read(out area);
        //ba.read(out command);
        ////model.type = type;
        ////model.area = area;
        ////model.command = command;
        ////判断读取完协议后 是否还有数据需要读取 是则说明有消息体 进行消息体读取
        //if (ba.Readnable)
        //{
        //    byte[] message;
        //    //将剩余数据全部读取出来
        //    ba.read(out message, ba.Length - ba.Position);
        //    //反序列化剩余数据为消息体
        //    //model.message = SerializeUtil.decode(message);
        //    Debug.Log(" message.area = " + model.area + " message.command = " + model.command + " message.message = " + model.message);

        //}
        //ba.Close();
        model = (NetModel)ProtoBufUtils.DeserializeAutoGZip(value, typeof(NetModel));
        Debug.Log("model.Message = " + model.Info);
        return model;
    }


    void OnDestroy()
    {
        if (this._client != null)
        {
            this._client.Close();
        }
    }
    /// <summary>
    /// 基础的接收消息方法
    /// </summary>
    /// <param name="ar"></param>
    public void ReceiveMessage2(IAsyncResult ar)
    {
        try
        {
            //清空errormessage
            Error_Message = "";
            int bytesRead;
            bytesRead = this._client.GetStream().EndRead(ar);
            if (bytesRead < 1)
            {
                return;
            }
            else
            {
                Debug.Log(System.Text.Encoding.ASCII.GetString(data, 0, bytesRead));
            }

            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage2, null);
        }
        catch (Exception ex)
        {
            Error_Message = ex.Message;
        }
    }




}