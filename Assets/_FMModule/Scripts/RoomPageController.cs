using UnityEngine;
using System.Collections.Generic;

public class RoomPageController : MonoBehaviour
{

    public int ItemLength = 0;//总个数

    private int pageNum = 1;//总页数
    public int perPageNum = 9;//每页有几个
    private int CurrentPageNum = 1;//当前在第几页

    public List<GameObject>ItemList = new List<GameObject>();

    public UILabel PageLabel;
    public UIButton NextPageBtn;
    public UIButton PrePageBtn;
    public UILabel LengthLabel;

    private void Start()
    {
        ShowPageInfo();
        NextPageBtn.onClick.Add(new EventDelegate(PageAdd));
        PrePageBtn.onClick.Add(new EventDelegate(PageReduce));
    }

    private void ShowPageInfo()
    {
        LengthLabel.text ="共"+ ItemLength+"条记录";
        PageLabel.text = "第 " + CurrentPageNum + " 页/共 " + pageNum + " 页";
    }


    //当总的item变动的时候，更新
    public void UpdatePage()
    {
        ItemLength = ItemList.Count;
        pageNum = Mathf.CeilToInt(ItemLength / (float)perPageNum);

        SetItemState();
    }
    //加页
    public void PageAdd()
    {
       CurrentPageNum++;
       SetItemState();
    }
    //减页
    public void PageReduce()
    {
       CurrentPageNum--;
       SetItemState();
    }

    public void ClearPageInfo()
    {
        ItemList.Clear();
    }
    //只打开当前页的item
    private void SetItemState()
    {
        CurrentPageNum = Mathf.Clamp(CurrentPageNum, 1, pageNum);
        ShowPageInfo();
        {
            for (int i = 0; i < ItemList.Count ; i++)
            {
                if (i >= (CurrentPageNum - 1)*perPageNum  && i < CurrentPageNum*perPageNum)
                {
                    ItemList[i].SetActive(true); 
                }
                else
                {
                    ItemList[i].SetActive(false);
    
                }
            }
        }
        if (ItemList.Count > 0)
        ItemList[0].GetComponentInParent<UIGrid>().repositionNow = true;
    }
}
