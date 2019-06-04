using UnityEngine;
using System.Collections;
/// <summary>
/// 控制箭头上下浮动
/// 李佼 2014-12-18
/// </summary>
public class JianTouUpDown : MonoBehaviour {

	private Vector3 origionPos;
    public float distance = 1;
    public float duaringTime = 2;
    //void Start () 
    //{
    //    origionPos = transform.position;
    //}
	
    //void Update () 
    //{
    //    transform.position = new Vector3(origionPos.x, origionPos.y + Mathf.PingPong(Time.time * duaringTime, distance), origionPos.z);
    //}

    void Start()
    {
        origionPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = new Vector3(origionPos.x, origionPos.y + Mathf.PingPong(Time.time * duaringTime, distance), origionPos.z);
    }
}
