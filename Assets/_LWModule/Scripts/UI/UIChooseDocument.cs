using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIChooseDocument : UIBase
{
    /// <summary>
    /// 显示题目的lable
    /// </summary>
    public UILabel theme;

    /// <summary>
    /// 选项的根节点
    /// </summary>
    public GameObject grid;

    void Start ()
    {
        Init();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

    }

    Dictionary<int, ChildInfo> childDic = new Dictionary<int, ChildInfo>();

    void Init()
    {
        theme.text = "请选取需要审核的单据！";

        List<ItemValue> itemList = new List<ItemValue>() { new ItemValue("商业发票",true), new ItemValue("销售合同",true), new ItemValue("海运提单",true),
                                                           new ItemValue("代理报检委托书",true), new ItemValue("入境货物报检单",true), new ItemValue("入境货物报关单",false)};

        for (int i = 0; i < itemList.Count;i++)
        {
            int index = Random.Range(0, itemList.Count);

            if (i == index)
            {
                continue;
            }

            ItemValue item = itemList[i];
            itemList[i] = itemList[index];
            itemList[index] = item;
        }

        int count = grid.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform go = grid.transform.GetChild(i);
            go.FindChild("Label").GetComponent<UILabel>().text = itemList[i].name;

            ChildInfo cd = new ChildInfo();

            cd.toggle = go.GetComponent<UIToggle>();
            cd.istrue = itemList[i].value;

            childDic.Add(i, cd);
        }


    }

    /// <summary>
    /// 判断是否选择正确
    /// </summary>
    /// <returns></returns>
    bool Judge()
    {
        foreach (int i in childDic.Keys)
        {
            if (childDic[i].toggle.value != childDic[i].istrue)
            {
                return false;
            }
        }

        return true;
    }

    public void OnClickSubmit(GameObject sender)
    {
        //选择正确
        if (Judge())
        {
            Debuge.LogError("选择正确");
            ExecuteInspection ei = (ExecuteInspection)TaskManager.Instance.GetTaskBase(TaskType.进口报检_实施检验检疫);
            ei.OnChooseFinish();
            gameObject.SetActive(false);
        }
        else
        {
            Debuge.LogError("选择不正确");
            theme.text = "选择不正确，请再次认真选取需要审核的单据！";
        }
    }

    public class ChildInfo
    {
        public UIToggle toggle;
        public bool istrue;
    }


    public class ItemValue
    {
        public string name;
        public bool value;

        public ItemValue()
        {

        }

        public ItemValue(string _name, bool _value)
        {
            name = _name;
            value = _value;
        }
    }
}
