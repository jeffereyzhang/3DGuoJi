using System;
using UnityEngine;
using System.Collections;
using System.Net.Configuration;
using DG.Tweening;
using UnityStandardAssets.Cameras;

public class CameraCtrl : Singleton<CameraCtrl>
{
    public Transform cameraTransform;
    private FreeLookCam personCamera;
   // public Transform Player;
    //public void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.M))
    //    SetCameraTarget(Player);
    //}

    // Use this for initialization
    void Awake()
    {
        personCamera = cameraTransform.GetComponent<FreeLookCam>();
    }
    /// <summary>
    /// 设置相机跟随的目标
    /// </summary>
    /// <param name="target"></param>
    public void SetCameraTarget(Transform target)
    {
        personCamera.SetTarget(target);
    }

    /// <summary>
    ///设置相机的位置 
    /// </summary>
    public void SetPosition(Transform targetPos)
    {
        cameraTransform.position = targetPos.position;
        cameraTransform.rotation = targetPos.rotation;
    }
    /// <summary>
    /// 相机动画过度到目标位置
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    /// <param name="duration">过度时间</param>
    /// <param name="action">结束回调</param>
    public void SetPosition(Transform targetPos, float duration, Action action = null)
    {
        cameraTransform.DOMove(targetPos.position, duration).OnComplete(() =>
        {
            if (action != null)
            {
                action();
            }
        });
        cameraTransform.DORotate(targetPos.eulerAngles, duration);
    }
 

    public void SetCameraState(bool isEnable)
    {
        personCamera.enabled = isEnable;
    }
}
