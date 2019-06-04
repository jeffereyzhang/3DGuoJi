using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.ThirdPerson;


public class PlayerStandAlone : MonoBehaviour
{
    //当前任务
    public TaskType MyTaskFlow;
	// Use this for initialization
	void Start () {
        GetComponent<ThirdPersonUserControl>().enabled = true;
        GetComponent<ThirdPersonCharacter>().enabled = true;
        CameraCtrl.Instance.SetCameraTarget(this.transform);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void ShowPlayer()
    {

    }
}
