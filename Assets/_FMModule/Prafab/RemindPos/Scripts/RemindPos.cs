using System;
using UnityEngine;
using System.Collections;

public class RemindPos : Singleton<RemindPos>
{

    private GameObject remind;
    private BoxCollider triggerBoxCollider;
    private MeshCollider quadCollider;

    private Action<GameObject> action; 
	// Use this for initialization
	void Awake ()
	{
        remind = transform.FindChild("Remind").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void Show(Vector3 pos,Action<GameObject> action = null)
    {
        transform.position = pos;
        remind.SetActive(true);

        this.action = action;
    }
    public void Hide()
    {
        remind.SetActive(false);
        this.action = null;
    }

    public void OnTrigger(GameObject  obj)
    {
        if (action != null)
        {
            action(obj);
        }
    }
}
