using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClickComputer : MonoBehaviour
{
    private BoxCollider box;

    private UITaskIntroduction uiti;

    void Start()
    {
        box = gameObject.GetComponent<BoxCollider>();
        uiti = UIContainer.Instance.GetUI<UITaskIntroduction>();
      //  GameManager._curTaskType = TaskType.核销退税;
        GetTask();
    }

    void GetTask()
    {
        switch (GameManager._curTaskType)
        {
            case TaskType.租船订舱:
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("单证员", "杭州婉丽进出口有限公司", 0, "bgry"));
                ChangeScene(0);
                uiti.Show("缮制单据", "杭州婉丽进出口有限公司办公室", "杭州婉丽进出口有限公司委托德威联系船公司安排订舱并装运货物。", ShanZhi);
                break;

            case TaskType.投保:
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("单证员", "杭州婉丽进出口有限公司", 0, "bgry"));
                ChangeScene(0);
                uiti.Show("缮制投保单据", "杭州婉丽进出口有限公司办公室", "杭州婉丽进出口有限公司准备投保资料。", TouBaoDan);
                break;

            case TaskType.提空装箱:
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("业务员", "上海德威国际集装箱货运公司", 0, "bgry"));
                ChangeScene(0);
                uiti.Show("缮制设备交接单", "上海德威国际集装箱货运公司", "德威公司开具的集装箱设备交接单。", JiaoJieDan);
                break;

            case TaskType.签发提单:
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("制单员", "中远上海分公司办公室", 0, "bgry"));
                ChangeScene(0);
                uiti.Show("缮制提单", "中远上海分公司办公室", "中远上海分公司为杭州婉丽进出口有限公司提供提单。", TiDan);
                break;

            case TaskType.交单结汇:
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("单证员", "杭州婉丽进出口有限公司办公室", 0, "bgry"));
                ChangeScene(0);
                uiti.Show("缮制汇票", "杭州婉丽进出口有限公司办公室", "杭州婉丽进出口公司准备交单结汇业务资料。", HuiPiao);
                break;

            case TaskType.核销退税:
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("单证员", "杭州婉丽进出口有限公司办公室", 0, "bgry"));
                ChangeScene(0);
                uiti.Show("填制核销单", "杭州婉丽进出口有限公司办公室", "杭州婉丽进出口公司准备出口收汇的核销退税资料。", HeXiaoDan);
                break;
            default:
                return;
        }
    }

    void Update()
    {

    }

    #region 
    
    public GameObject player;

    public enum TaskScene
    {
        办公室 = 1,
        堆场 = 2,
        银行 = 3
    }


    [System.Serializable]
    public class TaskPos
    {
        /// <summary>
        /// 只是方便绑定  没啥用
        /// </summary>
        public TaskScene scene;
        public Transform pos;
    }

    public List<TaskPos> taskPosList = new List<TaskPos>();


    void ChangeScene(int index)
    {
        for (int i = 0; i < 3; i++)
        {
            if (index == i)
            {
                taskPosList[i].pos.parent.gameObject.SetActive(true);
                player.transform.position = taskPosList[i].pos.position;
            }
            else
            {
                taskPosList[i].pos.parent.gameObject.SetActive(false);
            }
        }
    }

    #endregion




    #region 点击电脑

    /// <summary>
    /// 鼠标变色信息
    /// </summary>
    private MouseoverOutline mo;
    /// <summary>
    /// 点击响应信息
    /// </summary>
    private EventTrigger et;

    public void AddClickComputer(Action<BaseEventData> ac)
    {
        if (et != null)
        {
            et.triggers.Clear();
            et = null;
        }
        box.enabled = true;

        Vector3 pos = transform.position;
        pos.y += 0.2f;
        PointToObject.Instance.Show(pos);
        et = AddEventTrigger(gameObject, (BaseEventData bd) =>
        {
            OnClick();
            ac(bd);
        });
     //   et = AddEventTrigger(gameObject, ac);
    }

    void OnClick()
    {
        PointToObject.Instance.Hide();
        Debug.LogError("======== OnClick =============");
        box.enabled = false;
        mo.HideOutLine();
        mo.OnMExit();
        //  et.triggers.Clear();
        //   mo = null;
    }

    /// <summary>
    /// 添加点击相应事件
    /// </summary>
    EventTrigger AddEventTrigger(GameObject click, Action<BaseEventData> ac)
    {
        EventTrigger et = click.GetComponent<EventTrigger>();
        if (et == null)
        {
            et = click.AddComponent<EventTrigger>();
        }

        et.triggers = new List<EventTrigger.Entry>();

        EventTrigger.Entry enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerDown;
        enter.callback = new EventTrigger.TriggerEvent();
        UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(ac);
        enter.callback.AddListener(callback);

        et.triggers.Add(enter);

        if (GetMouseoverOutline(click, out mo))
        {
            EventTrigger.Entry enter1 = new EventTrigger.Entry();
            enter1.eventID = EventTriggerType.PointerEnter;
            enter1.callback = new EventTrigger.TriggerEvent();
            UnityAction<BaseEventData> callback1 = new UnityAction<BaseEventData>((BaseEventData bed) => { mo.OnMEnter(); });
            enter1.callback.AddListener(callback1);

            et.triggers.Add(enter1);

            EventTrigger.Entry enter2 = new EventTrigger.Entry();
            enter2.eventID = EventTriggerType.PointerExit;
            enter2.callback = new EventTrigger.TriggerEvent();
            UnityAction<BaseEventData> callback2 = new UnityAction<BaseEventData>((BaseEventData bed) => { mo.OnMExit(); });
            enter2.callback.AddListener(callback2);

            et.triggers.Add(enter2);
        }

        return et;
    }

    /// <summary>
    /// 获取鼠标描边脚本
    /// </summary>
    bool GetMouseoverOutline(GameObject go, out MouseoverOutline mou)
    {
        mou = null;
        mou = go.GetComponent<MouseoverOutline>();
        if (mou != null)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region  租船订舱

    /// <summary>
    /// 缮制单据
    /// </summary>
    void ShanZhi()
    {
        PromptManager.Instance.Show(TaskDesc.租船订舱_开始缮制单据, NotarizeType.Center, () =>
        {
            AddClickComputer(FaPiaoZhuangXiangDan);
        });
    }

    void FaPiaoZhuangXiangDan(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.租船订舱_缮制单据));
        SocketManager.RegisterMsgHandle(MessageId.租船订舱_缮制单据完成, ShanZhiWan);
        //      ShanZhiWan();
    }

    void ShanZhiWan()
    {
        PromptManager.Instance.Show(TaskDesc.租船订舱_缮制单据完成, NotarizeType.Center, DingCang);
    }

    /// <summary>
    /// 装货单
    /// </summary>
    void DingCang()
    {
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("业务员", "上海德威国际集装箱货运公司", 0, "bgry"));
        ChangeScene(0);
        uiti.Show("订舱", "上海德威国际集装箱货运公司", "上海德威国际集装箱货运公司审核委托并进行配舱。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.租船订舱_开始订舱, NotarizeType.Center, () =>
            {
                AddClickComputer(ZhuangHuoDan);
            });
        });

        //PromptManager.Instance.Show(TaskDesc.租船订舱_开始订舱, NotarizeType.Center, () =>
        //{
        //    AddClickComputer(ZhuangHuoDan);
        //});
    }

    void ZhuangHuoDan(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.租船订舱_订舱));
        SocketManager.RegisterMsgHandle(MessageId.租船订舱_订舱完成, ZhuangHuoDanWan);
        //  ZhuangHuoDanWan();
    }

    void ZhuangHuoDanWan()
    {
        PromptManager.Instance.Show(TaskDesc.租船订舱_订舱完成, NotarizeType.Center, ZhuangChuan);
    }

    /// <summary>
    /// 收货单
    /// </summary>
    void ZhuangChuan()
    {
        //更换角色信息
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("船长", "码头", 0, "bgry"));
        ChangeScene(1);
        uiti.Show("装船", "码头", "配舱完成后，在码头对货物进行装船。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.租船订舱_开始装船, NotarizeType.Center, () =>
            {
                ShowShouHhuoDan();
             //   AddClickComputer(ShowShouHhuoDan);
            });
        });
        //PromptManager.Instance.Show(TaskDesc.租船订舱_开始装船, NotarizeType.Center, () =>
        //{
        //    AddClickComputer(ShowShouHhuoDan);
        //});
    }

    void ShowShouHhuoDan()
    {
        SocketManager.SendMsg(new NetModel(MessageId.租船订舱_装船));
        SocketManager.RegisterMsgHandle(MessageId.租船订舱_装船完成, ZuChuanDingCangWan);
        //      ZuChuanDingCangWan();
    }

    void ZuChuanDingCangWan()
    {
        PromptManager.Instance.Show(TaskDesc.租船订舱_装船完成, NotarizeType.Center, () =>
        {
            ExitScene();
          //  SocketManager.SendMsg(new NetModel(MessageId.租船订舱完成));
        });
    }

    #endregion

    #region 投保

    /// <summary>
    /// 投保单
    /// </summary>
    void TouBaoDan()
    {
        //更换角色信息

        PromptManager.Instance.Show(TaskDesc.投保_开始投保单, NotarizeType.Center, () =>
        {
            AddClickComputer(ShowTouBaoDan);
        });
    }

    void ShowTouBaoDan(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.投保_投保单));
        SocketManager.RegisterMsgHandle(MessageId.投保_投保单完成, TouBaoDanWan);
    }

    void TouBaoDanWan()
    {
        PromptManager.Instance.Show(TaskDesc.投保_投保单完成, NotarizeType.Center, BaoXianDan);
    }

    /// <summary>
    /// 保险单
    /// </summary>
    void BaoXianDan()
    {
        //更换角色信息
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("制单员", "保险公司", 0, "bgry"));
        ChangeScene(0);
        uiti.Show("保险办理", "保险公司", "单证员到保险公司提交投保单和商业发票，办理投保。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.投保_开始保险单, NotarizeType.Center, () =>
            {
                AddClickComputer(ShowBaoXianDan);

            });
        });
        //PromptManager.Instance.Show(TaskDesc.投保_开始保险单, NotarizeType.Center, () =>
        //{
        //    AddClickComputer(ShowBaoXianDan);

        //});
    }

    void ShowBaoXianDan(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.投保_保险单));
        SocketManager.RegisterMsgHandle(MessageId.投保_保险单完成, TouBaoWan);
    }

    void TouBaoWan()
    {
        PromptManager.Instance.Show(TaskDesc.投保_保险单完成, NotarizeType.Center, () =>
        {
            ExitScene();
          //  SocketManager.SendMsg(new NetModel(MessageId.投保完成));
        });
    }

    #endregion

    #region 提空装箱

    /// <summary>
    /// 交接单
    /// </summary>
    void JiaoJieDan()
    {

        PromptManager.Instance.Show(TaskDesc.提空装箱_开始制作交接单, NotarizeType.Center, () =>
        {
            AddClickComputer(ShowJiaoJieDan);
        });
    }

    void ShowJiaoJieDan(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.提空装箱_交接单));
        SocketManager.RegisterMsgHandle(MessageId.提空装箱_交接单完成, JiaoJieDanWan);
    }

    void JiaoJieDanWan()
    {
        PromptManager.Instance.Show(TaskDesc.提空装箱_制作交接单完成, NotarizeType.Center, TiKong);
    }

    void TiKong()
    {
        //更换角色信息
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("司机", "堆场", 0, "bgry"));
        ChangeScene(1);
        uiti.Show("提取空箱", "堆场", "杭州婉丽进出口有限公司司机在堆场提取空箱。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.提空装箱_开始展示交接单, NotarizeType.Center, () =>
            {
                ShowJJD();
                //  AddClickComputer(ShowJJD);
            });
        });
        //PromptManager.Instance.Show(TaskDesc.提空装箱_开始展示交接单, NotarizeType.Center, () =>
        //{
        //    AddClickComputer(ShowJJD);
        //});
    }

    void ShowJJD()
    {
        SocketManager.SendMsg(new NetModel(MessageId.提空装箱_提取));
        SocketManager.RegisterMsgHandle(MessageId.提空装箱_提取完成, TiKongWan);
    }

    void TiKongWan()
    {
        PromptManager.Instance.Show(TaskDesc.提空装箱_展示交接单完成, NotarizeType.Center, () =>
        {
            ExitScene();
          //  SocketManager.SendMsg(new NetModel(MessageId.提空装箱完成));
        });
    }

    #endregion

    #region 签发提单

    void TiDan()
    {
        PromptManager.Instance.Show(TaskDesc.签发提单_开始签发提单, NotarizeType.Center, () =>
        {
            AddClickComputer(ShowTiDan);

        });
    }

    void ShowTiDan(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.签发提单_提单));
        SocketManager.RegisterMsgHandle(MessageId.签发提单_提单完成, TiDanWan);
    }

    void TiDanWan()
    {
        PromptManager.Instance.Show(TaskDesc.签发提单_签发提单完成, NotarizeType.Center, () =>
        {
            ExitScene();
           // SocketManager.SendMsg(new NetModel(MessageId.签发提单完成));
        });
    }

    #endregion

    #region 交单结汇

    /// <summary>
    /// 汇票
    /// </summary>
    void HuiPiao()
    {

        PromptManager.Instance.Show(TaskDesc.交单结汇_开始汇票, NotarizeType.Center, () =>
        {
            ShowHuiPiao();
         //   AddClickComputer(ShowHuiPiao);
        });
    }

    void ShowHuiPiao()
    {
        SocketManager.SendMsg(new NetModel(MessageId.交单结汇_汇票));
        SocketManager.RegisterMsgHandle(MessageId.交单结汇_汇票完成, HuiPiaoWan);
    }

    void HuiPiaoWan()
    {
        PromptManager.Instance.Show(TaskDesc.交单结汇_汇票完成, NotarizeType.Center, ShouHui);
    }

    void ShouHui()
    {
        //更换角色信息
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("银行工作人员", "中国银行杭州支行办公大厅", 0, "bgry"));
        ChangeScene(2);
        uiti.Show("办理押汇", "中国银行杭州支行办公大厅", "杭州婉丽进出口有限公司到银行办理押汇。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.交单结汇_开始押汇, NotarizeType.Center, () =>
            {
                ShowYaHui();
              //  AddClickComputer(ShowYaHui);
            });
        });
        //PromptManager.Instance.Show(TaskDesc.交单结汇_开始押汇, NotarizeType.Center, () =>
        //{
        //    AddClickComputer(ShowYaHui);
        //});
    }

    void ShowYaHui()
    {
        SocketManager.SendMsg(new NetModel(MessageId.交单结汇_收汇));
        SocketManager.RegisterMsgHandle(MessageId.交单结汇_收汇完成, JiaHuiWan);
    }

    void JiaHuiWan()
    {
        PromptManager.Instance.Show(TaskDesc.交单结汇_押汇完成, NotarizeType.Center, () =>
        {
            ExitScene();
          //  SocketManager.SendMsg(new NetModel(MessageId.交单结汇完成));
        });
    }

    #endregion

    #region 核销退税

    /// <summary>
    /// 核销单
    /// </summary>
    void HeXiaoDan()
    {

        PromptManager.Instance.Show(TaskDesc.核销退税_开始核销单, NotarizeType.Center, () =>
        {
            AddClickComputer(ShowHeXiao);
        });
    }

    void ShowHeXiao(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.核销退税_核销单));
        SocketManager.RegisterMsgHandle(MessageId.核销退税_核销单完成, HuiHeXiao);
        //HuiHeXiao();
    }

    void HuiHeXiao()
    {
        Debug.LogError("HuiHeXiao");
        PromptManager.Instance.Show(TaskDesc.核销退税_核销单完成, NotarizeType.Center, DengJiBiao);
    }

    /// <summary>
    /// 登记表
    /// </summary>
    void DengJiBiao()
    {
        //更换角色信息
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("外汇局工作人员", "国家外汇局", 0, "bgry"));
        ChangeScene(0);
        uiti.Show("核销", "国家外汇局", "杭州婉丽进出口有限公司去银行办理核销退税。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.核销退税_开始登记表, NotarizeType.Center, () =>
            {
                AddClickComputer(ShowDengJi);
            });
        });
        //PromptManager.Instance.Show(TaskDesc.核销退税_开始登记表, NotarizeType.Center, () =>
        //{
        //    AddClickComputer(ShowDengJi);
        //});
    }

    void ShowDengJi(BaseEventData bed)
    {
        SocketManager.SendMsg(new NetModel(MessageId.核销退税_登记表));
        SocketManager.RegisterMsgHandle(MessageId.核销退税_登记表完成, TuiShuiWan);
        //TuiShuiWan();
    }

    void TuiShuiWan()
    {
        PromptManager.Instance.Show(TaskDesc.核销退税_登记表完成, NotarizeType.Center, () =>
        {
            KaiShiTuiShui();
        });
    }


    void KaiShiTuiShui()
    {
        //更换角色信息
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(new roleStrut("单证员", "国税局", 0, "bgry"));
        ChangeScene(0);
        uiti.Show("退税", "国税局", "杭州婉丽进出口有限公司到国税局办理退税手续。", () =>
        {
            PromptManager.Instance.Show(TaskDesc.核销退税_开始退税, NotarizeType.Center, () =>
            {
                XianShiZhiShiDian();
            });
        });
    }

    void XianShiZhiShiDian()
    {
        SocketManager.SendMsg(new NetModel(MessageId.核销退税_打开知识点));
        SocketManager.RegisterMsgHandle(MessageId.核销退知识点关闭, JieSuTuiShui);
    }

    void JieSuTuiShui()
    {
        PromptManager.Instance.Show(TaskDesc.核销退税_退税完成, NotarizeType.Center, () =>
        {
            ExitScene();
        });
    }

    #endregion

    public void ExitScene()
    {
        GameSenceManager.Instance.InitRegister();

        SocketManager.SendMsg(new NetModel(103));
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadSence");
    }
}
