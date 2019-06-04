using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour
{

    [SerializeField]
    private float shotCoolDown = 0.3f;
	[SerializeField]
    private Transform firePositon;

    private float ellapsedTime;
    private bool canShoot;

        // Use this for initialization
	void Start ()
	{
	    if (isLocalPlayer)
	        canShoot = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!canShoot)
	        return;

	    ellapsedTime += Time.deltaTime;
	    if (Input.GetButtonDown("Fire1") && ellapsedTime > shotCoolDown)
	    {
	        ellapsedTime = 0;
	       // CmdFireShot(firePositon.forward,firePositon.forward);
	    }
	}
    [Command]
    private void CmdFireShot(Vector3 origin,Vector3 direction)
    {
        RaycastHit hit;
        
        Ray ray = new Ray(origin,direction);
        Debug.DrawRay(ray.origin, ray.direction*3f,Color.green);

        bool result = Physics.Raycast(ray, out hit, 50f);
        if (result)
        {
            
        }
    }
}
