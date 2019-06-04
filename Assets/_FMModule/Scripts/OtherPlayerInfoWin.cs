using UnityEngine;
using System.Collections;

public class OtherPlayerInfoWin : MonoBehaviour
{

    private TweenScale myTweenScale;

    public UILabel NameLabel;
    public UILabel NumberLabel;
    public UILabel RoleLable;

	// Use this for initialization
	void Start ()
	{
	    myTweenScale = this.GetComponent<TweenScale>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnShow(string name,string number,string role)
    {
        NameLabel.text = name;
        NumberLabel.text = number;
        RoleLable.text = role;
        myTweenScale.PlayForward();
    }

    public void OnHide()
    {
        myTweenScale.PlayReverse();
    }
}
