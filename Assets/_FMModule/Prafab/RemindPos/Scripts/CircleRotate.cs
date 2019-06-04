using UnityEngine;
using System.Collections;

public class CircleRotate : MonoBehaviour {

	public Vector3 rotateCoor;
	void Start () 
	{
	
	}
	

	void Update () 
	{
		gameObject.transform.Rotate(rotateCoor*Time.deltaTime);
	}
}
