using System.Collections;
using System.Runtime.InteropServices;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.ThirdPerson;

[NetworkSettings(channel = 0,sendInterval = 0.033f)]
public class Player : NetworkBehaviour
{
    [SyncVar(hook = "OnMyName")]
    public  string Name;
    [SyncVar]
    private Vector3 syncPos;
    [SyncVar] 
    private Quaternion syncRotation;

    private Vector3 lastPos;
    private float Threshold = 0.05f;
    private Quaternion qua;
    private NetworkClient client;
    private void Start()
    {
        EnablePlayer();
    }

    private void EnablePlayer()
    {
        //网络版相机看向本地玩家
        if (GameManager.IsNet)
        {
            if (isLocalPlayer)
            {
                GetComponent<ThirdPersonCharacter>().enabled = true;
                GetComponent<ThirdPersonUserControl>().enabled = true;
                CameraCtrl.Instance.SetCameraTarget(this.transform);
                GameManager.MyPlayer = this;
                client = LobbyManager.Instance.client;
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(GameManager.GetTaskRoleName(GameManager._curTaskType));
            }
            else
            {
                GetComponent<Rigidbody>().mass = 100;
            }
        }
    }
    void OnMyName(string name)
    {
        Name = name;
    }
    

    private void LerpPostion()
    {
        if (!isLocalPlayer)
        {
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime*15f);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * 15f);
        }
    }
    [Command]
    private void CmdUpdatePos(Vector3 pos)
    {
        syncPos = pos;
    }
    [Command]
    private void CmdUpdateRot(Quaternion rot)
    {
        syncRotation = rot;
    }
    [ClientCallback]
    private void TransmitPos()
    {
        //如果玩家移动或者旋转，则向服务器发布同步的消息
        if (isLocalPlayer)
        {
            if (Vector3.Distance(transform.position, lastPos) > Threshold)
            {
                CmdUpdatePos(transform.position);
                lastPos = transform.position;
            }
            if (Quaternion.Angle(transform.rotation, qua) > Threshold)
            {
                CmdUpdateRot(transform.rotation);
                qua = transform.rotation;
            }
        }
    }

    public void FixedUpdate()
    {
        TransmitPos();
        LerpPostion();
    }
}