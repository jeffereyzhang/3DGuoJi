using UnityEngine;
using System.Collections;
using ProtoBuf;

[ProtoContract]
public class NetModel
{
    //添加特性，表示该字段可以被序列化，1可以理解为下标
    [ProtoMember(1)]
    public int ID;
    // 完成socket任务的ID
    [ProtoMember(2)]
    public int CommitID;
    [ProtoMember(3)]
    public string Info;

    [ProtoMember(4)]
    public ProtoObject MessageContent;

    public NetModel(int _id)
    {
        this.ID = _id;
    }

    public NetModel()
    {

    }
}



[ProtoContract]
public class BaseStruct : ProtoObject
{
    [ProtoMember(6)]
    public string xiaoxiti;
    public BaseStruct(string _name)
    {
        xiaoxiti = _name;
    }
}
