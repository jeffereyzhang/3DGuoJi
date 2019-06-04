using UnityEngine;
using System.Collections;
using System;

public class UITaskIntroduction : UIBase 
{
    private Action click;

    public UILabel taskName;
    public UILabel uiScene;
    public UILabel uiDesc;

    void Awake()
    {
        gameObject.SetActive(false);
    }

	void Start () 
    {

	}

    public void Show(string name, string scene, string desc, Action cl)
    {
        gameObject.SetActive(true);

        taskName.text = "任务："+name;
        uiScene.text = "场景："+scene;
        uiDesc.text = "介绍："+desc;
        click = cl;
    }

    public void OnClickButton()
    {
        if (click != null)
        {
            click();
            click = null;
        }

        gameObject.SetActive(false);
    }
	
}
