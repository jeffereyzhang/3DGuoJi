using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 拖动铅封操作  
/// 使用NGUI点击和 unity EventTrigger实现
/// </summary>
public class UIDragDocument : UIBase
{
    public Camera uiCame;
    public UIButton iconSeal;

    private bool isEnter = false;
    public GameObject target;

	void Start ()
    {
       UIEventListener ue = UIEventListener.Get(iconSeal.gameObject);
       ue.onClick = OnHoverButton;

       gameObject.SetActive(false);
	}
	
	void Update () 
    {
        if (isEnter)
        {
            gameObject.transform.position = ScreenToUI(Input.mousePosition);
        }
	}

    public void Show()
    {
        isEnter = false;
        target = null;
        gameObject.SetActive(true);
    }

    void OnHoverButton(GameObject sender)
    {
        if (!isEnter)
        {
            isEnter = true;

            if(target == null)
            {
                SiteInspection ei = (SiteInspection)TaskManager.Instance.GetTaskBase(TaskType.进口报关_现场查验);
                target = ei.rightDoor;
            }

            tventT = AddEventTrigger(target, OnClickLeftDoor);
        }
        else
        {
            isEnter = false;
            if (tventT != null)
            {
                tventT.triggers.Clear();
            }
        }
    }

    void OnClickLeftDoor(BaseEventData bed)
    {
        if (tventT != null)
        {
            tventT.triggers.Clear();
        }

        SiteInspection ei = (SiteInspection)TaskManager.Instance.GetTaskBase(TaskType.进口报关_现场查验);
        ei.EndSite();

        gameObject.SetActive(false);
    }


    EventTrigger tventT;
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

        return et;
    }

    /// <summary>
    /// 把屏幕坐标转换成UI坐标
    /// </summary>
    public Vector3 ScreenToUI(Vector3 point)
    {
        if (uiCame == null)
        {
            return Vector3.zero;
        }

        Vector3 spos = uiCame.ScreenToWorldPoint(point);

        return spos;
    }
}
