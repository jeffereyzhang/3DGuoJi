using System;
using UnityEngine;
using System.Collections;

public class PointToObject : Singleton<PointToObject>
{

    public  GameObject Arrow;
    private Action<GameObject> action;
    // Use this for initialization
  

    public void Show(Vector3 pos, Action<GameObject> action = null)
    {
        transform.position = pos;
        Arrow.SetActive(true);

        this.action = action;
    }
    public void Hide()
    {
        Arrow.SetActive(false);
        this.action = null;
    }
}
