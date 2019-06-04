// This class contains some helper functions.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class Utils {



    public static float ClosestDistance(Collider a, Collider b) {
        var posA = a.transform.position;
        var posB = b.transform.position;
        return Vector3.Distance(a.ClosestPointOnBounds(posB),
                                b.ClosestPointOnBounds(posA));
    }

    // pretty print seconds as hours:minutes:seconds
    public static string PrettyTime(float seconds) {
        var t = System.TimeSpan.FromSeconds(seconds);
        var res = "";
        if (t.Days > 0) res += t.Days + "d";
        if (t.Hours > 0) res += " " + t.Hours + "h";
        if (t.Minutes > 0) res += " " + t.Minutes + "m";
        if (t.Seconds > 0) res += " " + t.Seconds + "s";
        // if the string is still empty because the value was '0', then at least
        // return the seconds instead of returning an empty string
        return res != "" ? res : "0s";
    }


    // find local player (clientsided)
    public static Player ClientLocalPlayer() {
        // note: ClientScene.localPlayers.Count cant be used as check because
        // nothing is removed from that list, even after disconnect. It still
        // contains entries like: ID=0 NetworkIdentity NetID=null Player=null
        // (which might be a UNET bug)
        var p = ClientScene.localPlayers.Find(pc => pc.gameObject != null);
        return p != null ? p.gameObject.GetComponent<Player>() : null;
    }
}
