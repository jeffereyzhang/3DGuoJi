using UnityEngine;
using System.Collections;

public class RemindStopTrigger : MonoBehaviour 
{

    public void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            SendMessageUpwards("OnTrigger", other.gameObject);
        }
    }

}
