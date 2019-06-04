using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Timers;



public class TaskManager : Singleton<TaskManager>
{
    public Dictionary<TaskType, TaskBase> taskDic = new Dictionary<TaskType, TaskBase>();//包含所有任务的字典集合

    public Dictionary<TaskType, TaskBase> finishedTaskDic = new Dictionary<TaskType, TaskBase>();//已完成任务的字典集合

    public bool isTaskDoing;//是否有任务正在进行

    private int totalScore = 0;

    private int taskToalTime;
    private bool gameOverFlag;
    private TaskBase currentTask = null;
    public TaskBase lastTask = null;

    System.Diagnostics.Stopwatch stopwatch;

    void Start()
    {
        StartMyTimer();

        //InitTask(GameManager._curTaskType);//根据GameManager的任务类型初始化任务，这个类型会在winform中点击按钮后socket通信过来改变
        Debug.LogError("GameManager._curTaskType = " + GameManager._curTaskType);
        Debug.LogError("GameManager._curModuleType = " + GameManager._curModuleType);
        InitTaskContent();

    }

    public void InitTaskContent()
    {
        if (!GameManager.IsNet)//单人模式则开始当前点击的任务
        {
            StartTask(GameManager._curTaskType);
            Debug.Log("GameManager._curTaskType = " + GameManager._curTaskType);
        }
        else//如果是多人模式找到模块的第一个任务并开始
        {
            if (GameManager._curModuleType == ModuleType.进口报检流程)
            {
                if (GameManager._curTaskType == TaskType.进口报检_委托报检)
                {
                    StartTask(TaskType.进口报检_委托报检);
                }
            }
            else if (GameManager._curModuleType == ModuleType.进口报关流程)
            {
                if (GameManager._curTaskType == TaskType.进口报关_委托报关)
                {
                    StartTask(TaskType.进口报关_委托报关);
                }
            }
        }
    }


    /// <summary>
    /// 开始计时器
    /// </summary>
    private void StartMyTimer()
    {
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
    }
    /// <summary>
    /// 结束计时器
    /// </summary>
    private void StopMyTimer()
    {
        stopwatch.Stop();
        System.TimeSpan timespan = stopwatch.Elapsed;
        taskToalTime = (int)timespan.TotalSeconds;  //  总秒数
    }
    /// <summary>
    /// 获取总分
    /// </summary>
    /// <returns></returns>
    public int GetTotalScore()
    {
        return totalScore;
    }
    /// <summary>
    /// 停掉计时器并获取总用时
    /// </summary>
    /// <returns></returns>
    public int GetTotalTime()
    {
        StopMyTimer();
        return taskToalTime;
    }

    /// <summary>
    /// 获取任务完成个数
    /// </summary>
    /// <returns></returns>
    public int GetFinishTaskCount()
    {
        return finishedTaskDic.Count;
    }


    public TaskBase GetCurrentTask()
    {

        return currentTask;
    }

    public TaskBase GetTaskBase(TaskType tt)
    {
        if (taskDic.ContainsKey(tt))
        {
            return taskDic[tt];
        }
        else
        {
            Debuge.LogError("此任务未注册 ：" + tt);
            return null;
        }
    }


    /// <summary>
    /// 注册任务
    /// </summary>
    /// <param name="tb">任务类</param>
    public void RegisterTask(TaskBase tb)
    {
        if (tb.curTaskType == TaskType.NullTask)
        {
            Debuge.LogError("未设置任务类型！");
            return;
        }
        if (!taskDic.ContainsKey(tb.curTaskType))
        {
            taskDic.Add(tb.curTaskType, tb);
            Debuge.LogError("注册成功：" + tb.curTaskType);
        }
        else
        {
            Debuge.LogError("任务类型：" + tb.curTaskType + "已经注册！");
        }
    }
    /// <summary>
    /// 开始任务
    /// </summary>
    /// <param name="_curTaskType"></param>
    public TaskBase StartTask(TaskType _curTaskType)
    {
        if (!taskDic.ContainsKey(_curTaskType))
        {
            Debuge.LogError("任务：" + _curTaskType + " 未注册至任务系统！无法开始，需提前注册！！！");
            return null;
        }
        if (taskDic[_curTaskType].curTaskState != TaskState.未开始)
            return null;
        if (isTaskDoing)
        {
            Debuge.Log("有任务正在进行！任务类型：" + CheckHasTaskDoing().ToString());
            return null;
        }
        taskDic[_curTaskType].OnStart();
        Debuge.Log("@@@ = " + taskDic[_curTaskType].curTaskType);

        isTaskDoing = true;
        taskDic[_curTaskType].curTaskState = TaskState.进行中;
        currentTask = taskDic[_curTaskType];
        //StartCoroutine(Helper.DelayToInvokeDo(() =>
        //{

        //}, 1.5f));
        return currentTask;
    }

    /// <summary>
    /// 重置任务状态
    /// </summary>
    /// <param name="_curTaskType"></param>
    public void ResetTaskState(TaskType _curTaskType)
    {

        if (!taskDic.ContainsKey(_curTaskType))
        {
            Debuge.LogError("任务：" + _curTaskType + " 未注册至任务系统！无法开始，需提前注册！！！");
            return;
        }

        taskDic[_curTaskType].curTaskState = TaskState.未开始;
    }

    /// <summary>
    /// 任务完成,当TaskBase中步骤完成后调用此方法
    /// </summary>
    /// <param name="_curTaskType"></param>
    public void TaskFinish(TaskType _curTaskType)
    {
        if (!taskDic.ContainsKey(_curTaskType))
        {
            Debuge.LogError("任务：" + _curTaskType + " 未注册至任务系统！请注册！！！");
            return;
        }
        
        Debuge.Log("任务：" + _curTaskType + "");
        currentTask = null;

        isTaskDoing = false;

        #region
        
        #endregion
    }


    public void TaskFinish(TaskType _curTaskType, TaskState _curTaskState)
    {
        if (!taskDic.ContainsKey(_curTaskType))
        {
            Debuge.LogError("任务：" + _curTaskType + " 未注册至任务系统！请注册！！！");
            return;
        }
        isTaskDoing = false;
        if (_curTaskState != TaskState.放弃任务)
        {
            Debuge.Log("SwitchWinState......");
        }
        GameManager._curTaskType = TaskType.NullTask;
        taskDic.Clear();
        SocketManager.MessageHandleDic.Clear();
        SocketManager.clientMsgHandleDic.Clear();


        //单机模式或者多人联机模式事件
        SocketManager.RegisterMsgHandle(101, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(102, GameSenceManager.Instance.SenceManageMsgHandle);

        //点击左侧列表，传值id与tasktype
        SocketManager.RegisterMsgHandle(111, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(112, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(113, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(114, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(115, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(116, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(117, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(118, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(119, GameSenceManager.Instance.SenceManageMsgHandle);
        SocketManager.RegisterMsgHandle(120, GameSenceManager.Instance.SenceManageMsgHandle);

        SocketManager.RegisterMsgHandle(188, GameSenceManager.Instance.SenceManageMsgHandle);

        SocketManager.RegisterMsgHandle(106, GameSenceManager.Instance.PlayerInfoMsgHandle);
    }

    public void AbortTask()
    {

    }

    public bool GetGameOverFlag()
    {
        return gameOverFlag;
        //禁用所有任务触发
    }


    /// <summary>
    /// 检测当前是否有任务在执行
    /// </summary>
    /// <returns></returns>
    public TaskType CheckHasTaskDoing()
    {
        foreach (TaskType key in taskDic.Keys)
        {
            if (taskDic[key].curTaskState == TaskState.进行中)
            {
                return key;//返回正在执行任务的键
            }
        }
        return TaskType.NullTask;//当前没有任务在执行
    }
    /// <summary>
    /// 将已完成任务添加进已完成列表
    /// </summary>
    /// <param name="tb"></param>
    /// <returns></returns>
    public bool AddToFinishTaskDic(TaskBase tb)
    {
        if (!taskDic.ContainsValue(tb))
        {
            Debuge.LogError("此任务未注册...");
            return false;
        }

        //if (finishedTaskDic.ContainsValue(tb))
        //{
        //    Debuge.LogError("此任务已包含在已完成任务集合...");
        //    return false;
        //}
        //finishedTaskDic.Add(tb.curTaskType, tb);

        //控制加分与是否成功完成任务部分
        if (tb.curTaskState == TaskState.任务成功)
        {
            totalScore += tb.score; //加分
            Debuge.Log("totalScore = " + totalScore);
        }
        else if (tb.curTaskState == TaskState.放弃任务)//星星变暗
        {
            Debuge.Log("任务失败 totalScore = " + totalScore);
            totalScore -= tb.score; //扣分
            //设置完成状态
        }

        return true;
    }

    //void OnApplicationQuit()
    //{

    //}



}
