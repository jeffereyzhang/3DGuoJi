using UnityEngine;
using System.Collections;
using System.Linq;

public class StandAlonePlayerManager : Singleton<StandAlonePlayerManager>
{
    private PlayerStandAlone[] playerList;
	// Use this for initialization
	void Awake ()
	{
        //网络版关闭单机玩家
	    if (GameManager.IsNet)
	    {
	        gameObject.SetActive(false);
	    }
	    playerList = transform.GetComponentsInChildren<PlayerStandAlone>(true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public GameObject GetPlayer(TaskType taskFlow)
    {
        UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(GameManager.GetTaskRoleName(taskFlow));
        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].MyTaskFlow == taskFlow)
            {
                playerList[i].gameObject.SetActive(true);
                playerList[i].enabled = true;
                UIContainer.Instance.GetUI<UIPlayerInfo>().UpDataUI(GameManager.GetTaskRoleName(taskFlow));
                return playerList[i].gameObject;
            }
        }
        return null;
    }
}
