using System;
using UnityEngine;
using System.Collections.Generic;

public class UIFunctionButton : UIBase
{
    /// <summary>
    /// 点击案例背景
    /// </summary>
    public void OnClickCase()
    {
        //发送打开案例背景的消息
        SocketManager.SendMsg(new NetModel(104));
    }

    /// <summary>
    /// 点击单据
    /// </summary>
    public void OnClickKnowledge()
    {
        Debuge.LogError(" ============ ClickKnowledge ");
    }

    /// <summary>
    /// 点击背包
    /// </summary>
    public void OnClickKnapsack()
    {
        if (DocumentsItem == null || !haveDocuments)
        {
            Debuge.LogError(" ============ 没有绑定单据按钮预设或者没有添加单据 ");
            return;
        }
        Debuge.LogError(" ============ ClickKnapsack ");
        //
        grid.gameObject.SetActive(!grid.gameObject.activeSelf);
    }



    void Start()
    {
        grid.gameObject.SetActive(false);

        AddHover();
    }

    //int tid = 0;
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 50, 30), "添加"))
    //    {
    //        AddDocuments("单据" + tid, () => { Debuge.LogError( "点击了：单据"+ tid); });
    //        tid += 1;
    //    }
    //    if (GUI.Button(new Rect(70, 10, 50, 30), "清空"))
    //    {
    //        ClearKnapsack();
    //    }
    //}

    #region  关于单据

    public UIButton DocumentsItem;  //单个单据预设
    public UIGrid grid;  //单据都放到这个下面

    private Dictionary<int, Action> actionDic = new Dictionary<int, Action>();

    /// <summary>
    /// 标记现在背包里是否有单据
    /// </summary>
    private bool haveDocuments = false;

    private List<DocumentsUI> documentsList = new List<DocumentsUI>();

    /// <summary>
    /// 点击单据  
    /// </summary>
    public void OnClickDocuments(GameObject sendder)
    {
        int id = (int)UIEventListener.Get(sendder).parameter;
        Debuge.LogError(" ============ ClickDocuments id = " + id);

        if (actionDic.ContainsKey(id))
        {
            //Debuge.LogError(" ============ ContainsKey id = " + id);
            if (actionDic[id] != null)
            {
                actionDic[id]();
            }
        }
    }

    /// <summary>
    /// 清空单据
    /// </summary>
    public void ClearKnapsack()
    {
        int count = documentsList.Count;
        for (int i = 0; i < count; i++)
        {
            if (documentsList[i].used)
            {
                documentsList[i].used = false;
                documentsList[i].button.gameObject.SetActive(false);

                Debuge.LogError("i = " + i);
            }
        }

        if (grid.gameObject.activeSelf)
        {
            grid.gameObject.SetActive(false);
        }

        actionDic.Clear();

        haveDocuments = false;
    }

    /// <summary>
    /// 添加单据
    /// </summary>
    /// <param name="name">单据名字</param>
    /// <param name="id">单据编号</param>
    /// <returns></returns>
    public bool AddDocuments(string name, Action ac)
    {
        if (grid == null || DocumentsItem == null)
        {
            return false;
        }

        haveDocuments = true;

        int id = actionDic.Count;
        actionDic.Add(id, ac);

        int count = documentsList.Count;
        for (int i = 0; i < count; i++)
        {
            if (!documentsList[i].used)
            {
                InitDocuments(documentsList[i], name, id);
                return true;
            }
        }

        DocumentsUI du = GetDocuments();
        documentsList.Add(du);
        InitDocuments(du, name, id);

        return true;
    }

    /// <summary>
    /// 给一个单据注册事件
    /// </summary>
    /// <param name="d"></param>
    /// <param name="name"></param>
    /// <param name="id"></param>
    void InitDocuments(DocumentsUI d, string name, int id)
    {
        UIEventListener ue = UIEventListener.Get(d.button.gameObject);
        ue.onClick = OnClickDocuments;
        ue.parameter = id;

        d.lable.text = name;
        d.button.gameObject.SetActive(true);
        d.used = true;

        grid.repositionNow = true;
    }

    /// <summary>
    /// 实例化一个单据按钮
    /// </summary>
    /// <returns></returns>
    DocumentsUI GetDocuments()
    {
        DocumentsUI du = new DocumentsUI();

        GameObject go = GameObject.Instantiate(DocumentsItem.gameObject);

        du.button = go.GetComponent<UIButton>();
        du.lable = go.transform.FindChild("Lable").GetComponent<UILabel>();
        du.used = false;

        go.transform.parent = grid.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        return du;
    }

    public class DocumentsUI
    {
        public UIButton button;
        public UILabel lable;
        public bool used = false;
    }

    #endregion

    #region  按钮放上效果处理

    public GameObject caseButton;
    public GameObject knowledgeButton;
    public GameObject knapsackButton;

    public GameObject caseHover;
    public GameObject knowledgeHover;
    public GameObject knapsackHover;

    void AddHover()
    {
        caseHover.SetActive(false);
        knowledgeHover.SetActive(false);
        knapsackHover.SetActive(false);

        UIEventListener.Get(caseButton).onHover = (GameObject sender,bool flag) =>
        {
            if(flag)
            {
                caseHover.SetActive(true);
            }
            else
            {
                caseHover.SetActive(false);
            }
        };

        UIEventListener.Get(knowledgeButton).onHover = (GameObject sender, bool flag) =>
        {
            if (flag)
            {
                knowledgeHover.SetActive(true);
            }
            else
            {
                knowledgeHover.SetActive(false);
            }
        };

        UIEventListener.Get(knapsackButton).onHover = (GameObject sender, bool flag) =>
        {
            if (flag)
            {
                knapsackHover.SetActive(true);
            }
            else
            {
                knapsackHover.SetActive(false);
            }
        }; 
    }

    #endregion

}
