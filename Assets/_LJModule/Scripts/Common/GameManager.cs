using UnityEngine;
using System.Collections;
using System;

public delegate void VoidDelegate();

public delegate void MsgDelegate(object _obj);


public enum TaskType
{
    NullTask = 0,

    进口报检_委托报检 = 1,
    进口报检_电子申报 = 2,
    进口报检_审核报检材料 = 3,
    进口报检_实施检验检疫 = 4,
    进口报检_出证放行 = 5,

    进口报关_委托报关 = 6,
    进口报关_电子申报 = 7,
    进口报关_审核单证 = 8,
    进口报关_现场查验 = 9,
    进口报关_通关放行 = 10,

    租船订舱 = 11,
    投保 = 12,
    提空装箱 = 13,
    签发提单 = 14,
    交单结汇 = 15,
    核销退税 = 16,

}

public enum ModuleType
{
    NullModule = 0,
    购销合同流程 = 1,
    信用证申请 = 2,
    产地证申请 = 3,
    加工贸易合同备案 = 4,
    进口报关流程 = 5,
    出口报关流程 = 6,
    填制海运提单 = 7,
    填制商业发票 = 8,
    进口报检流程 = 9,
    出口报检流程 = 10,
}

/// <summary>
/// 管理系统类型
/// </summary>
public enum ManagerSystemType
{
    进口报检_委托报检系统,
    进口报检_电子申报系统,
    进口报检_,
}

/// <summary>
/// 单据类型
/// </summary>
public enum InvoiceType
{
    委托报检单,
    单据2,
    单据3,
}

public enum TaskState
{
    未开始 = 0,
    进行中 = 1,
    放弃任务 = 2,
    任务成功 = 3,
}

public struct MessageStruct
{
    public int messageID;
    public Action doAction;
}

public struct roleStrut
{
    public string roleName;
    public string address;
    public int score;
    public string headIcon;

    public roleStrut(string _roleName, string _address, int _score, string _headIcon)
    {
        this.address = _address;
        this.headIcon = _headIcon;
        this.roleName = _roleName;
        this.score = _score;
    }
}


public partial class GameManager
{
    public static VoidDelegate systemTipsDelegate;
    public static MsgDelegate messsgeDelegate;
    public static TaskType _curTaskType;
    public static ModuleType _curModuleType = ModuleType.进口报检流程;
    public static string studentName = "科比";


    public static roleStrut GetTaskRoleName(TaskType tt)
    {
        roleStrut curRole = new roleStrut();
        switch (tt)
        {
            case TaskType.NullTask:
                break;

            case TaskType.进口报检_委托报检:
                curRole = new roleStrut("业务员", "中福实业总公司", 80, "qt");
                break;

            case TaskType.进口报检_电子申报:
                curRole = new roleStrut("报检员", "汉德通关服务有限公司", 80, "ywy");
                break;

            case TaskType.进口报检_审核报检材料:
                curRole = new roleStrut("检务员", "检验检疫大厅", 80, "bjryn");
                break;

            case TaskType.进口报检_实施检验检疫:
                curRole = new roleStrut("检验人员", "港口查验地点", 80, "cyry");
                break;

            case TaskType.进口报检_出证放行:
                curRole = new roleStrut("通关人员", "检验检疫大厅", 80, "bjrynn");
                break;


            case TaskType.进口报关_委托报关:
                curRole = new roleStrut("业务员", "中福实业总公司", 80, "qt");
                break;

            case TaskType.进口报关_电子申报:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;

            case TaskType.进口报关_审核单证:
                curRole = new roleStrut("审单员", "报关大厅", 80, "hgryn");
                break;

            case TaskType.进口报关_现场查验:
                curRole = new roleStrut("海关查验员", "港口查验区", 80, "hgry");
                break;

            case TaskType.进口报关_通关放行:
                curRole = new roleStrut("海关人员", "报关大厅", 80, "hgryn");
                break;


            case TaskType.租船订舱:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;

            case TaskType.投保:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;

            case TaskType.提空装箱:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;
            case TaskType.签发提单:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;

            case TaskType.交单结汇:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;

            case TaskType.核销退税:
                curRole = new roleStrut("报关员", "汉德通关服务有限公司", 80, "bgry");
                break;

            default:
                break;
        }
        return curRole;
    }
}
