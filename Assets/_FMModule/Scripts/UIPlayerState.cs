using UnityEngine;
using System.Collections;

public class UIPlayerState : UIBase
{
    public UILabel HeathLabel;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnHeathChange(int health)
    {
        HeathLabel.text = health.ToString();
    }
}
