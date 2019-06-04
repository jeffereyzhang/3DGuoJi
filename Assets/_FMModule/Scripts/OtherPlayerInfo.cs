using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class OtherPlayerInfo : MonoBehaviour
{
    [SerializeField]
    private string RoleName;//角色名称
    private ChatSystem chatSystem;
    private SelectInfo info;
    public OtherPlayerInfoWin InfoWin;

	// Use this for initialization
	void Start ()
	{

        chatSystem = FindObjectOfType<ChatSystem>();
        info = GameManager.GetOtherPlayerInfo(RoleName);
	}
    
    //鼠标点击事件,指定消息发给谁
    private void OnClick()
    {
        chatSystem.ChatToSb(info.Id,info.Name);
        InfoWin.OnHide();
    }
    //鼠标悬浮事件
    private void OnHover(bool isHover)
    {
        if (isHover)
        {
            InfoWin.OnShow(info.Name, info.Number, RoleName);
        }
        else
        {
            InfoWin.OnHide();
        }
    }
}
