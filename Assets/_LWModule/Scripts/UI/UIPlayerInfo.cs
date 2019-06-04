using UnityEngine;
using System.Collections;

public class UIPlayerInfo : UIBase
{
    public UISprite icon; //角色头像
    public UILabel playerName; //角色名字
    public UILabel scene;  //办公区域名字
    public UILabel score; //分数数值
    public UISlider slider; //分数条

    //private string[] iconName;
    //private string[] playerName;

    private const int maxScore = 100;

    void Awake()
    {
        icon.spriteName = "";
        playerName.text = "";
        scene.text = "";
        score.text = "";
        slider.value = 0;
    }

    //void Start () 
    //{


    //    //iconName = new string[] { "Button", "Button A", "Button B", "Button X", "Button Y", "Emoticon - Smirk", "Emoticon - Annoyed" };
    //    //playerName = new string[] { "第1个任务角色", "第2个任务角色", "第3个任务角色", "第4个任务角色", "第5个任务角色", "第6个任务角色", "第7个任务角色" };

    //    //string _roleName, string _address, int _score, string _headIcon
    //  //  UpDataUI(new roleStrut("是个人", "天空之城", 100, ""));
    //}

    public void UpDataUI(roleStrut rs)
    {

      //  Debuge.LogError("UpDataUI");

        UpDataNameAndIcon(rs.headIcon, rs.roleName);
        UpDataScene(rs.address);
        UpDataScore(rs.score);
    }

    public void UpDataNameAndIcon(string headIcon, string roleName)
    {
        icon.spriteName = headIcon;
        playerName.text = roleName;
    }
    public void UpDataScene(string sceneName)
    {
        scene.text = sceneName;
    }
    public void UpDataScore(float scoreNum)
    {
        slider.value = scoreNum / maxScore;
        score.text = scoreNum + "/" + maxScore;
    }
}
