using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSenceManager : Singleton<GameSenceManager>
{

    void Start()
    {
        InitRegister();
    }

    public  void InitRegister()
    {
        SocketManager.MessageHandleDic.Clear();
        SocketManager.clientMsgHandleDic.Clear();


        //单机模式或者多人联机模式事件
        SocketManager.RegisterMsgHandle(101, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(102, GameSenceManager.Instance.SenceManageMsgHandle);

        //点击左侧列表，传值id与tasktype
        SocketManager.RegisterMsgHandle(111, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(112, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(113, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(114, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(115, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(116, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(117, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(118, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(119, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(120, GameSenceManager.Instance.SenceManageMsgHandle);

        SocketManager.RegisterMsgHandle(188, GameSenceManager.Instance.SenceManageMsgHandle);

        SocketManager.RegisterMsgHandle(106, GameSenceManager.Instance.PlayerInfoMsgHandle);
    }

    public void PlayerInfoMsgHandle(object _msgObject)
    {
        NetModel tempNetModel = (NetModel)_msgObject;
        GameManager.studentName = tempNetModel.MessageContent.Value.ToString();
    }
    /// <summary>
    /// 根据消息ID，找到对应的senceType。再根据taskType在sencetype中的场景中设置
    /// </summary>
    /// <param name="_msgObject"></param>
    /// <returns></returns>
    public void SenceManageMsgHandle(object _msgObject)
    {
        NetModel tempNetModel = (NetModel)_msgObject;
        switch (tempNetModel.ID)
        {
            case 101:// 单人模式
                GameManager.IsNet = false;
                break;

            case 102:// 多人联机模式
                GameManager.IsNet = true;
                break;

            case 111:// 购销合同流程
                GameManager._curModuleType = ModuleType.购销合同流程;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 112:// 信用证申请
                GameManager._curModuleType = ModuleType.信用证申请;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 113:// 产地证申请
                GameManager._curModuleType = ModuleType.产地证申请;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 114:// 加工贸易合同备案
                GameManager._curModuleType = ModuleType.加工贸易合同备案;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 115:// 进口报关流程
                GameManager._curModuleType = ModuleType.进口报关流程;
                if (tempNetModel.MessageContent != null)
                {
                    Debug.LogError("******接收到的任务类型 = " + (TaskType)(System.Convert.ToInt32(tempNetModel.MessageContent.Value)));
                    GameManager._curTaskType = (TaskType)(System.Convert.ToInt32(tempNetModel.MessageContent.Value));//指定管理类中的当前任务类型
                }

                if (!GameManager.IsNet)//单人模式
                {

                    SceneManager.LoadScene("GameSence_BaoGuan");//加载至报检场景
                }
                else//多人模式
                {
                    SceneManager.LoadScene("LobbyScene");//加载至报检角色选择场景
                }

                break;

            case 116:// 出口报关流程
                GameManager._curModuleType = ModuleType.出口报关流程;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 117:// 填制海运提单
                GameManager._curModuleType = ModuleType.填制海运提单;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 118:// 填制商业发票
                GameManager._curModuleType = ModuleType.填制商业发票;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 119://进口报检场景
                GameManager._curModuleType = ModuleType.进口报检流程;
                if (tempNetModel.MessageContent != null)
                {
                    //int nTaskType;
                    Debug.LogError("接收到的任务类型 = " + (TaskType)(System.Convert.ToInt32(tempNetModel.MessageContent.Value)));
                    GameManager._curTaskType = (TaskType)(System.Convert.ToInt32(tempNetModel.MessageContent.Value));//指定管理类中的当前任务类型
                }

                if (!GameManager.IsNet)//单人模式
                {

                    SceneManager.LoadScene("GameSence_BaoJian");//加载至报检场景
                }
                else//多人模式
                {
                    Debug.LogError("进入多人在线模式");
                    SceneManager.LoadScene("LobbyScene");//加载至报检角色选择场景
                }
                break;

            case 120://
                GameManager._curModuleType = ModuleType.出口报检流程;
                if (tempNetModel.MessageContent != null)
                {
                    GameManager._curTaskType = (TaskType)tempNetModel.MessageContent.Value;//指定管理类中的当前任务类型
                }
                break;

            case 188://
                if (tempNetModel.MessageContent != null)
                {
                    Debug.LogError("接收到的任务类型 = " + (TaskType)(System.Convert.ToInt32(tempNetModel.MessageContent.Value)));
                    GameManager._curTaskType = (TaskType)(System.Convert.ToInt32(tempNetModel.MessageContent.Value));//指定管理类中的当前任务类型
                }
                SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                //if (!GameManager.IsNet)//单人模式
                //{
                //    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                //}
                break;

            case 219://
                GameManager._curTaskType = TaskType.租船订舱;
                if (!GameManager.IsNet)//单人模式
                {
                    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                }
                break;

            case 220://
                GameManager._curTaskType = TaskType.投保;
                if (!GameManager.IsNet)//单人模式
                {
                    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                }
                break;

            case 221://
                GameManager._curTaskType = TaskType.提空装箱;
                if (!GameManager.IsNet)//单人模式
                {
                    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                }
                break;

            case 222://
                GameManager._curTaskType = TaskType.签发提单;
                if (!GameManager.IsNet)//单人模式
                {
                    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                }
                break;

            case 223://
                GameManager._curTaskType = TaskType.交单结汇;
                if (!GameManager.IsNet)//单人模式
                {
                    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                }
                break;

            case 224://
                GameManager._curTaskType = TaskType.核销退税;
                if (!GameManager.IsNet)//单人模式
                {
                    SceneManager.LoadScene("LinGangGameScene");//加载至报检场景
                }
                break;

        }
        //throw new System.NotImplementedException();
    }

    void Update()
    {

    }

    public void LoadGameSence(int senceIndex)
    {
        TaskType _curType = (TaskType)senceIndex;

        GameManager._curTaskType = _curType;//指定管理类中的当前任务类型

        switch (_curType)
        {
            case TaskType.进口报检_出证放行:
                UnityEngine.SceneManagement.SceneManager.LoadScene("");//加载到指定场景
                Debuge.Log("初始化场景：" + _curType);
                break;

            case TaskType.进口报检_电子申报:
                Debuge.Log("初始化场景：" + _curType);
                break;
        }
    }
}
