using UnityEngine;
using System.Collections;

public class UIChecking : UIBase
{
    /// <summary>
    /// 展示需要检查的图片
    /// </summary>
    public UISprite checkSprite;

    /// <summary>
    /// 关于图片的一段描述
    /// </summary>
    public UILabel lable;

    /// <summary>
    /// 正确按钮上显示文字
    /// </summary>
    public UILabel okLable;
    /// <summary>
    /// 错误按钮上显示文字
    /// </summary>
    public UILabel noLable;

    public enum CType
    {
        Null = 0,  //不知道给传进来了一个啥
        Container = 1,  //检查集装箱
        Seal = 2, //检查铅封
        Marks = 3,  //报检唛头信息
        Marksg = 4  //报关唛头信息
    }

    private CType checkType = CType.Null;

    void Start ()
    {
        gameObject.SetActive(false);
	}
	
	void Update ()
    {
	
	}

    /// <summary>
    /// 显示检查的UI界面
    /// </summary>
    /// <param name="type"></param>
    public void Show(int type)
    {
        if (type == (int)CType.Container)
        {
            checkType = CType.Container;
            InitContainerUI();
        }
        else if (type == (int)CType.Seal)
        {
            checkType = CType.Seal;
            InitSealUI();
        }
        else if (type == (int)CType.Marks)
        {
            checkType = CType.Marks;
            InitMarksUI();
        }
        else if (type == (int)CType.Marksg)
        {
            checkType = CType.Marksg;
            InitMarksgUI();
        }
        else
        {
            checkType = CType.Null;
            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 初始化检查集装箱的UI
    /// </summary>
    void InitContainerUI()
    {
        checkSprite.spriteName = "jzx_bq";

        lable.text = "核对集装箱规格与检验文件是否相符！";

        okLable.text = "相符";
        noLable.text = "不相符";
    }

    /// <summary>
    /// 初始化检查铅封的UI
    /// </summary>
    void InitSealUI()
    {
        checkSprite.spriteName = "zs1";

        lable.text = "检查集装箱及其铅封是否完整！";

        okLable.text = "完整";
        noLable.text = "不完整";
    }

    void InitMarksUI()
    {
        checkSprite.spriteName = "wz_bq";

        lable.text = "检查货物外包装的唛头、标记、编号及数量与检验文件是否相符！";

        okLable.text = "相符";
        noLable.text = "不相符";
    }

    void InitMarksgUI()
    {
        checkSprite.spriteName = "wz_bq";

        lable.text = "经查验唛头信息准确，货物无问题！";

        okLable.text = "确定";
  //      noLable.text = "不相符";
        noLable.gameObject.transform.parent.gameObject.SetActive(false);


        Vector3 pos = okLable.gameObject.transform.parent.localPosition;
        pos.x = 0;
        okLable.gameObject.transform.parent.localPosition = pos;
    }

    /// <summary>
    /// 点击对的处理
    /// </summary>
    public void OnClickOk()
    {
        gameObject.SetActive(false);

        if (checkType == CType.Container)
        {
            Show(2);
        }
        else if (checkType == CType.Seal)
        {
            ExecuteInspection ei = (ExecuteInspection)TaskManager.Instance.GetTaskBase(TaskType.进口报检_实施检验检疫);
            ei.OpenDoor();
        }
        else if (checkType == CType.Marks)
        {
            ExecuteInspection ei = (ExecuteInspection)TaskManager.Instance.GetTaskBase(TaskType.进口报检_实施检验检疫);
            ei.CheckingMarksEnd();
        }
        else if (checkType == CType.Marksg)
        {
            SiteInspection ei = (SiteInspection)TaskManager.Instance.GetTaskBase(TaskType.进口报关_现场查验);
            ei.CloseTheDoor();
        }
    }

    /// <summary>
    /// 点击错误的处理
    /// </summary>
    public void OnClickNo()
    {
        if (checkType == CType.Container)
        {
            lable.text = "请认真核对集装箱规格与检验文件是否相符！";
        }
        else if (checkType == CType.Seal)
        {
            lable.text = "请认真检查集装箱及其铅封是否完整！";
        }
        else if (checkType == CType.Marks)
        {
            lable.text = "请认真检查货物外包装的唛头、标记、编号及数量与检验文件是否相符！";
        }
    }
}
