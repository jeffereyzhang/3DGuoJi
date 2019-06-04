using UnityEngine;
using System.Collections;

public class ChatManager : MonoBehaviour
{

    public UIButton CloseBtn;
    public UIButton OpenBtn;

    public GameObject ChatWinGameObject;
    public GameObject LastChatGameObject;

    public GameObject BGObj;
    public GameObject BJObj;
	// Use this for initialization
	void Start ()
	{
	    OnCloseChatWin();

	    EventDelegate.Add(CloseBtn.onClick, OnCloseChatWin);
        EventDelegate.Add(OpenBtn.onClick, OnOpenChatWin);

	    if (GameManager.IsNet)
	    {
	        if (GameManager._curModuleType == ModuleType.进口报关流程)
	        {
	            BGObj.SetActive(true);
                BJObj.SetActive(false);
	        }
	        else if (GameManager._curModuleType == ModuleType.进口报检流程)
	        {
                BJObj.SetActive(true);
                BGObj.SetActive(false);
	        }
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnOpenChatWin()
    {
        ChatWinGameObject.SetActive(true);
        LastChatGameObject.SetActive(false);
    }

    private void OnCloseChatWin()
    {
        ChatWinGameObject.SetActive(false);
        LastChatGameObject.SetActive(true);
    }
}
