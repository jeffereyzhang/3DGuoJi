using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;


public class LJtest : MonoBehaviour
{

    // Use this for initialization
    public UILabel msgLabel;
    public static LJtest instance;
    public UIButton sendmsgbtn;
    void Start()
    {
        instance = this;
    }

    private void OnMssgClick()
    {
        NetModel nm = new NetModel(108);
        nm.MessageContent = new ProtoObject(TaskType.进口报关_通关放行);
        SocketManager.SendMsg(nm);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("GameSence_BaoJian");//退回至主界面
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NetModel nm = new NetModel(108);
            nm.MessageContent = new ProtoObject(TaskType.进口报关_通关放行);
            SocketManager.SendMsg(nm);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SocketManager.RegisterMsgHandle(101, () =>
            {
                Debuge.LogError("unity端自定义的101消息方法体！");
            });

            SocketManager.RegisterMsgHandle(132, On102recieve);


        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //TaskManager.Instance.StartTask(TaskType.进口报检_委托报检);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("LoadSence");//退回至主界面
            
        }

        //if()
    }

    private void  On102recieve(object obj)
    {
        Debug.Log("((NetModel)obj).MessageContent.Value = " + ((NetModel)obj).MessageContent.Value);
    }
}
