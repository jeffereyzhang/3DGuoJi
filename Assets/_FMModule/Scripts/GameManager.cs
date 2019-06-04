using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class  GameManager : MonoBehaviour
{
    //网络玩家初始化的位置
    public static Transform NetPlayerStartPos ;

    private static Player myPlayer;

    public static Player MyPlayer
    {
        get { return myPlayer; }
        set
        {
            myPlayer = value;
            myPlayer.transform.position = NetPlayerStartPos.position;
        }
    }
    public static bool IsNet = false;

    public static string PlayerNumber;//学生学号，即登陆账号，由winform端传递过来
    //其它玩家信息
    private static Dictionary<string, SelectInfo> OtherPlayInfoDic = new Dictionary<string, SelectInfo>();

    public static void AddOtherPlayerInfo(string key, SelectInfo info)
    {
        OtherPlayInfoDic.Add(key, info);
    }
    public static void ClearOtherPlayerInfo()
    {
        OtherPlayInfoDic.Clear();
    }

    public static SelectInfo GetOtherPlayerInfo(string roleName)
    {
        SelectInfo info;
        OtherPlayInfoDic.TryGetValue(roleName, out info);
        return info;
    }
}
