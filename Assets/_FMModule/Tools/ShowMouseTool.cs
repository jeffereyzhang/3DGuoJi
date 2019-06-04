using UnityEngine;
using System.Collections.Generic;


public delegate void MyAction();
/// <summary>
/// 鼠标滑过显示热点图标
/// </summary>
public class ShowMouseTool : MonoBehaviour
{
    public bool isShine = true;
    public MyAction onMouseDownAction;
    public GameObject HighLightObj;

    public List<Renderer>otherRender = new List<Renderer>();

    //public //音效变量可在此暴露给检视面板
    // Use this for initialization
    public void Start()
    {
        if (HighLightObj == null)
            HighLightObj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnMouseEnter()
    {
        //if (UICamera.hoveredObject != null)
        //{
        //    Debuge.Log("UICamera.hoveredObject.name = " + UICamera.hoveredObject.name);
        //    return;
        //}
        if (DrawPickUpHand.Instance != null)
        {
            if (isShine)
            {
                DrawPickUpHand.Instance.DrawHand(HighLightObj.GetComponent<Renderer>());
                for (int i = 0; i < otherRender.Count; i++)
                {
                    DrawPickUpHand.Instance.DrawHand(otherRender[i]);
                }
            }
            else
            {
               DrawPickUpHand.Instance.DrawHand();
            }
        }
    }


    void OnMouseExit()
    {
        if (DrawPickUpHand.Instance != null)
            if (isShine)
            {
                DrawPickUpHand.Instance.DrawMouseNormal(HighLightObj.GetComponent<Renderer>());
                for (int i = 0; i < otherRender.Count; i++)
                {
                    DrawPickUpHand.Instance.DrawMouseNormal(otherRender[i]);
                }
            }
            else
            {
                DrawPickUpHand.Instance.DrawMouseNormal();
            }
    }

    void OnMouseDown()//如使用统一音效，可在此添加
    {
        DrawPickUpHand.Instance.DrawMouseNormal(HighLightObj.GetComponent<Renderer>());
        for (int i = 0; i < otherRender.Count; i++)
        {
            DrawPickUpHand.Instance.DrawMouseNormal(otherRender[i]);
        }
        if (onMouseDownAction != null)
        {
            onMouseDownAction();
        }

    }
    private void OnHover(bool isHover)
    {
        if (isHover)
        {
            OnMouseEnter();
        }
        else
        {
            OnMouseExit();
        }
    }

    private void OnClick()
    {
        OnMouseDown();
    }

}
