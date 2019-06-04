using UnityEngine;
using System.Collections;
using System;

public class PromptManager : Singleton<PromptManager>
{
    static Action click;
    public GameObject item;
    public GameObject selectItem;
    public GameObject notarizeItem;
    public GameObject waitItem;


    private TweenAlpha itemTa;
    private TweenPosition itemTp;
    private bool initTipsFlag = false;
    private string HeadSpace = "\u3000\u3000";

    void Awake()
    {
    }

    void Start()
    {

    }

    void InitStartData()
    {
        InitItem();
        item.SetActive(false);

        InitNotarize();

        itemTa = item.GetComponent<TweenAlpha>();
        itemTp = item.GetComponent<TweenPosition>();
    }

    void Update()
    {

    }

    public void Show(string st, PromptType type, Action Click = null)
    {
        InitStartData();//封装原start中初始化的数据
        StopCoroutine("WaitTipsHide");
        RestItemTween();

        item.SetActive(false);
        selectItem.SetActive(false);
        notarizeItem.SetActive(false);
        waitItem.SetActive(false);

        if (Click != null)
        {
            click = Click;
        }

        switch (type)
        {
            case PromptType.Tip:
                waitTime = 1;
                OnShowTips(st);
                break;

            case PromptType.Select:
                UpDataSelectItem(st);
                break;

            case PromptType.Notarize:
                UpDataNotarizeItem(st);
                break;

            case PromptType.Wait:
                UILabel l = waitItem.transform.FindChild("Label").GetComponent<UILabel>();
                l.text = HeadSpace + st;
                waitItem.SetActive(true);
                break;

            default:
                return;
        }
    }

    public void Show(string st, TipsType tips, Action Click = null)
    {
        InitStartData();
        StopCoroutine("WaitTipsHide");
        RestItemTween();

        waitTime = 1;
        OnShowTips(st, tips);


        if (Click != null)
        {
            click = Click;
        }

    }

    public void Show(string st, int time = 1, TipsType tips = TipsType.Center, Action Click = null)
    {
        InitStartData();
        StopCoroutine("WaitTipsHide");
        RestItemTween();

        waitTime = time;
        OnShowTips(st, tips);

        if (Click != null)
        {
            click = Click;
        }
    }

    public delegate void PNCallback();

    public void Show(string st, NotarizeType tepe, PNCallback callback = null)
    {
        InitStartData();
        item.SetActive(false);
        selectItem.SetActive(false);
        notarizeItem.SetActive(false);
        waitItem.SetActive(false);

        StopCoroutine("WaitTipsHide");
        RestItemTween();

        UpDataNotarizeItem(st, tepe, callback);
    }

    void OnShowTips(string st, TipsType tips = TipsType.Center)
    {
        item.SetActive(false);
        selectItem.SetActive(false);
        notarizeItem.SetActive(false);
        waitItem.SetActive(false);

        UpDataItem(st, tips);
    }




    #region 自动消失框

    public GameObject rItem;
    public GameObject cItem;
    public GameObject lItem;

    GameObject showItem;

    void InitItem()
    {
        rItem.SetActive(false);
        cItem.SetActive(false);
        lItem.SetActive(false);
    }

    void UpDataItem(string st, TipsType type = TipsType.Center)
    {
        item.SetActive(true);

        SetItemPos(type);
        showItem.SetActive(true);
        UILabel ul = showItem.transform.FindChild("Table/Label").GetComponent<UILabel>();
        ul.text =HeadSpace+ st;

        UILabel time = showItem.transform.FindChild("Table/Time").GetComponent<UILabel>();
        timelable = time;

        StartCoroutine("WaitTipsHide");
    }

    void SetItemPos(TipsType type)
    {
        if (showItem != null)
        {
            showItem.SetActive(false);
        }

        switch (type)
        {
            case TipsType.Center:
                showItem = cItem;

                cItem.transform.parent = item.transform.FindChild("Center").transform;
                cItem.transform.localPosition = Vector3.zero;
                cItem.transform.localScale = Vector3.one;
                break;

            case TipsType.LeftBottom:
                showItem = lItem;


                lItem.transform.parent = item.transform.FindChild("LeftBottom").transform;
                lItem.transform.localPosition = Vector3.zero;
                lItem.transform.localScale = Vector3.one;
                break;

            case TipsType.LeftCenter:
                showItem = lItem;

                lItem.transform.parent = item.transform.FindChild("LeftCenter").transform;
                lItem.transform.localPosition = Vector3.zero;
                lItem.transform.localScale = Vector3.one;
                break;

            case TipsType.RightCenter:
                showItem = rItem;

                rItem.transform.parent = item.transform.FindChild("RightCenter").transform;
                rItem.transform.localPosition = Vector3.zero;
                rItem.transform.localScale = Vector3.one;
                break;

            default:
                break;
        }
    }

    int waitTime; //提示停留时间
    UILabel timelable;

    IEnumerator WaitTipsHide()
    {
        int i = waitTime;
        timelable.text = i + "秒后消失";

        while (i >= 0)
        {
            yield return new WaitForSeconds(1f);
            i -= 1;
            timelable.text = i + "秒后消失";
        }

        itemTa.PlayForward();
        itemTp.PlayForward();
        initTipsFlag = true;

        StartCoroutine(Helper.DelayToInvokeDo(() =>
        {
            if (initTipsFlag)
            {
                RestItemTween();
                item.SetActive(false);

            }
        }, 1));
    }

    private void RestItemTween()
    {
        initTipsFlag = false;
        itemTa.enabled = false;
        itemTp.enabled = false;
        itemTa.ResetToBeginning();
        itemTp.ResetToBeginning();
    }

    #endregion

    #region 确认框

    GameObject nLitem;
    GameObject nCitem;

    void InitNotarize()
    {
        nLitem = notarizeItem.transform.FindChild("LItem").gameObject;
        nLitem.SetActive(false);
        nCitem = notarizeItem.transform.FindChild("CItem").gameObject;
        nCitem.SetActive(false);
    }

    void UpDataNotarizeItem(string st, NotarizeType tepe = NotarizeType.Center, PNCallback callback = null)
    {
        notarizeItem.SetActive(true);
        if (tepe == NotarizeType.Center)
        {
            if (nLitem.activeSelf)
            {
                nLitem.SetActive(false);
            }

            nCitem.SetActive(true);

            UILabel ul = nCitem.transform.FindChild("Table/Label").GetComponent<UILabel>();
            ul.text = HeadSpace+st;

            UITable tb = nCitem.transform.FindChild("Table").GetComponent<UITable>();
            tb.repositionNow = true;

            GameObject bt = nCitem.transform.FindChild("Table/Button/Yes").gameObject;
            UIEventListener.Get(bt).onClick = (GameObject sender) =>
            {


                nCitem.SetActive(false);
                notarizeItem.SetActive(false);

                if (click != null)
                {
                    click();
                    click = null;
                }
                if (callback != null)
                {
                    callback();
                }
            };
        }
        else
        {
            if (nCitem.activeSelf)
            {
                nCitem.SetActive(false);
            }

            nLitem.SetActive(true);

            UILabel ul = nLitem.transform.FindChild("Table/Label").GetComponent<UILabel>();
            ul.text = HeadSpace + st;
        }
    }

    public void HidePN()
    {
        if (nCitem.activeSelf)
        {
            nCitem.SetActive(false);
        }

        if (nLitem.activeSelf)
        {
            nLitem.SetActive(false);
        }

        if (notarizeItem.activeSelf)
        {
            notarizeItem.SetActive(false);
        }
    }

    #endregion

    #region 选择

    void UpDataSelectItem(string st)
    {
        selectItem.SetActive(true);
        UILabel ul = selectItem.transform.FindChild("Table/Label").GetComponent<UILabel>();
        ul.text = HeadSpace + st;

        GameObject bt = selectItem.transform.FindChild("Table/Button/No").gameObject;
        UIEventListener.Get(bt).onClick = (GameObject sender) =>
        {
            click = null;
            selectItem.SetActive(false);
        };

        GameObject gb = selectItem.transform.FindChild("Table/Button/Yes").gameObject;
        UIEventListener.Get(gb).onClick = OnClickButton;
    }

    void OnClickButton(GameObject sender)
    {
        selectItem.SetActive(false);
        if (click != null)
        {
            click();
            click = null;
        }
    }

    void test(GameObject go)
    {
        Debug.LogError("我就是试一下的  啦啦啦");
    }

    #endregion

    public void HideWait()
    {
        if (waitItem.activeSelf)
        {
            waitItem.SetActive(false);
        }
    }

}

public enum NotarizeType
{
    Center,
    LeftBottom
}

public enum PromptType
{
    /// <summary>
    /// 选择
    /// </summary>
    Select,

    /// <summary>
    /// 确认
    /// </summary>
    Notarize,

    /// <summary>
    /// 等待提示
    /// </summary>
    Wait,

    /// <summary>
    /// 提示
    /// </summary>
    Tip
}

public enum TipsType
{
    /// <summary>
    /// 显示在中间
    /// </summary>
    Center,
    /// <summary>
    /// 显示在左侧中间 
    /// </summary>
    LeftCenter,
    /// <summary>
    /// 显示在左侧下面
    /// </summary>
    LeftBottom,
    /// <summary>
    /// 显示在右侧中间
    /// </summary>
    RightCenter
}