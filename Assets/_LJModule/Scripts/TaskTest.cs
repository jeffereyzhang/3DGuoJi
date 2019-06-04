using UnityEngine;
using System.Collections;

public class TaskTest : TaskBase
{

    // Use this for initialization
    void Start()
    {
        TaskManager.Instance.RegisterTask(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            FinishItemOfTask(0);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        Debuge.Log("任务开始了");
    }

    protected override void OnFinish(TaskState finishState)
    {
        base.OnFinish(finishState);
        Debuge.Log("任务结束了");
    }

    public override void FinishItemOfTask(int itemIndex)
    {
        base.FinishItemOfTask(itemIndex);
        Debuge.Log("完成了任务中的一项");
    }
}
