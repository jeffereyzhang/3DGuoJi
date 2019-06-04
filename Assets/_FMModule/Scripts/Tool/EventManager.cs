using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{

    private Dictionary<EventEnum, UnityEvent<EventArgs>> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("场景里没有EventManager脚本");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EventEnum, UnityEvent<EventArgs>>();
        }
    }
    /// <summary>
    /// 开始监听
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StartListening(EventEnum eventType, UnityAction<EventArgs> listener)
    {
        UnityEvent<EventArgs> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new MyEvent<EventArgs>();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventType, thisEvent);
        }
    }
    /// <summary>
    /// 停止监听
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StopListening(EventEnum eventType, UnityAction<EventArgs> listener)
    {
        if (eventManager == null) return;

        UnityEvent<EventArgs> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="arg"></param>
    public static void TriggerEvent(EventEnum eventType, EventArgs arg)
    {
        UnityEvent<EventArgs> thisEvent = null;
        Debuge.Log(eventType);
        if (instance.eventDictionary.TryGetValue(eventType, out thisEvent))
        {
            thisEvent.Invoke(arg);
        }
    }

    //public void OnDestroy()
    //{
    //    eventDictionary.Clear();
    //}
}

public class MyEvent<T0>:UnityEvent<T0>
{

}

public struct EventArgs
{
    public UIEffectID UITeXiaoID;
    public RtsCameraTarget MyRtsCameraTarget;
    public string AreaInfo;

    public EventArgs(RtsCameraTarget rtsCameraTarget)
    {
        MyRtsCameraTarget = rtsCameraTarget;
        UITeXiaoID = UIEffectID.NULL;
        AreaInfo = null;
    }

    public EventArgs(UIEffectID teXiaoID)
    {
        UITeXiaoID = teXiaoID;
        MyRtsCameraTarget = RtsCameraTarget.集装箱拖车;
        AreaInfo = null;
    }

    public EventArgs(string areaInfo)
    {
        AreaInfo = areaInfo;
        UITeXiaoID = UIEffectID.NULL;
        MyRtsCameraTarget = RtsCameraTarget.集装箱拖车;
    }
}


