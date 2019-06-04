using UnityEngine;
using System.Collections;
using System;

using UnityEngine.SceneManagement;


[System.Serializable]
public class TaskProgress
{
    public string progresssName = "";
    public string descript = "";
    public bool isFinished = false;
}

/// <summary>
/// 任务基类，包含主线任务与任务进度控制
/// </summary>
public class TaskBase : MonoBehaviour
{
    public TaskType curTaskType = TaskType.NullTask;
    public TaskState curTaskState = TaskState.未开始;
    public TaskProgress[] taskProgressList;//任务进度
    public string taskTotalDescript;//总的任务描述
    public int score;
    public int taskRequireTime = 1200;//默认5分钟
    public int errorCountLimit = 5;//错误上限
    private int tempErrorCount;

    public int userTime;//用户使用时间
    public int starCount;//用户操作星级

    public Transform missionStartPos;//任务初始点


    void Update()
    {
        
    }

    public void BaseStartTask()
    {
        OnStart();

    }

    public virtual void OnStart()
    {
        if (curTaskState == TaskState.未开始)
        {
            Debuge.LogError("TaskBase: OnStart ：执行了 =  " + curTaskType);
            curTaskState = TaskState.进行中;
            GameManager._curTaskType = this.curTaskType;
            //UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(GameManager.GetTaskRoleName(curTaskType));
        }
    }
    /// <summary>
    /// 步骤取值范围0-n
    /// </summary>
    /// <param name="stepIndex"></param>
    public virtual void StepTips(int stepIndex)
    {
        //示例
        switch (stepIndex)
        {
            case 1:
                //按钮闪
                Debuge.Log("步骤提示1");
                break;
            case 2:
                //集装箱闪
                Debuge.Log("步骤提示2");
                break;
        }
    }

    public virtual void SetProgressTipsContent(int tipsIndex, string content)
    {
        taskProgressList[tipsIndex].descript = content;
    }


    /// <summary>
    /// 完成任务中的一小步
    /// </summary>
    /// <param name="itemIndex"></param>
    public virtual void FinishItemOfTask(int itemIndex)
    {
        if (curTaskState != TaskState.进行中)
        {
            Debuge.LogError("此任务未开始");
            return;
        }
        if (itemIndex < 0 || itemIndex > taskProgressList.Length - 1)
        {
            Debuge.LogError("任务步骤下标有误......");
            return;
        }
        if (taskProgressList[itemIndex].isFinished)
        {
            Debuge.LogError("此步骤已完成，不需要重复调用......");
            return;
        }
        else
        {
            taskProgressList[itemIndex].isFinished = true;

            Debuge.Log("步骤" + itemIndex + "完成！");
        }
        if (itemIndex == taskProgressList.Length - 1)
        {
            for (int i = 0; i < taskProgressList.Length; i++)
            {
                if (!taskProgressList[i].isFinished)
                {
                    return;
                }
            }
            //任务结束
            OnFinish(TaskState.任务成功);
            Debuge.Log("任务结束....");
        }
    }


    protected virtual void OnFinish(TaskState finishState)
    {
        Debuge.Log("OnFinish....");
        if (curTaskState == TaskState.进行中)
        {
            Debuge.Log("TaskBase: OnStop ： " + curTaskType);
            TaskManager.Instance.lastTask = this;
            //通知系统任务结束
            TaskManager.Instance.TaskFinish(curTaskType, finishState);
            curTaskState = finishState;

            TaskEvaluate();
            //将此任务添加至已完成字典集合
            if (TaskManager.Instance.AddToFinishTaskDic(this))
            {

            }
            else
            {
                Debuge.LogError("添加至已完成列表异常！");
            }
            //StartCoroutine(Helper.DelayToInvokeDo(() =>
            //{
            //}, 1.5f));
            if (GameManager.IsNet)//多人模式
            {
            }
            else//单人模式
            {
                //任务结束时同时通知winform界面切换到背景，开始后再将3D界面切换至前面

                UIContainer.Instance.GetUI<UITaskFinish>().SetTaskFinishContent(curTaskType, 80);
            }
        }
    }

    public void AbortTask()
    {
        OnFinish(TaskState.放弃任务);
    }

    public virtual void DamageTrigger()
    {
        tempErrorCount++;
        if (tempErrorCount > errorCountLimit)
        {
            Debuge.Log("错误次数达到此任务上限...");
        }
        Debuge.Log("tempErrorCount = " + tempErrorCount);
    }

    private int gradeLevel;
    private void TaskEvaluate()
    {
        if (tempErrorCount < errorCountLimit)
        {
            gradeLevel += 1;
        }
        if (curTaskState == TaskState.任务成功)
        {
            gradeLevel += 1;
        }
        //if (UITimeCountDown.Instance.GetTaskUsedTime() > 1)
        //{
        //    gradeLevel += 1;
        //}
        if (curTaskState == TaskState.放弃任务)
        {
            gradeLevel = 0;
        }

        this.starCount = gradeLevel;
        //this.userTime = taskRequireTime - UITimeCountDown.Instance.GetTaskUsedTime();

    }
}
