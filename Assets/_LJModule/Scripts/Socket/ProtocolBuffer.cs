using UnityEngine;
using System;
using ProtoBuf;
using System.IO;
using System.Collections;

using System.Collections.Generic;

public class ProtocolBuffer : MonoBehaviour
{
    //private List<NetModel> NetModelList = new List<NetModel>();

    void Start()
    {
        Test();
    }

    void Update()
    {

    }



    void Test()
    {
        NetModel nm = new NetModel() { ID = 250, CommitID = 123, Info = "Unity2222" };
        byte[] winformData = ProtoBufUtils.SerializeAutoGZip(nm, false);
        for (int i = 0; i < winformData.Length; i++)
        {
            Debug.LogError("# winformData[" + i + "]=" + winformData[i] + "\n");
        }

        string recive = System.Text.Encoding.ASCII.GetString(winformData, 0, winformData.Length);
        Debug.LogError("recive = " + recive);
        //序列化对象
        NetModel result = (NetModel)ProtoBufUtils.DeserializeAutoGZip(winformData, typeof(NetModel));
        Debug.LogWarning("result.ID  = " + result.ID + " .result.Commit =  " + result.CommitID + " .result.Message =  " + result.Info);
    }

    // 将消息序列化为二进制的方法
    // < param name="model">要序列化的对象< /param>
    private byte[] Serialize(NetModel model)
    {
        try
        {
            //涉及格式转换，需要用到流，将二进制序列化到流中
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法
                ProtoBuf.Serializer.Serialize<NetModel>(ms, model);
                //定义二级制数组，保存序列化后的结果
                byte[] result = new byte[ms.Length];
                //将流的位置设为0，起始点
                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("序列化失败: " + ex.ToString());
            return null;
        }
    }

    // 将收到的消息反序列化成对象
    // < returns>The serialize.< /returns>
    // < param name="msg">收到的消息.</param>
    private NetModel DeSerialize(byte[] msg)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //将消息写入流中
                ms.Write(msg, 0, msg.Length);
                //将流的位置归0
                ms.Position = 0;
                //使用工具反序列化对象
                NetModel result = ProtoBuf.Serializer.Deserialize<NetModel>(ms);
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("反序列化失败: " + ex.ToString());
            return null;
        }
    }
}
