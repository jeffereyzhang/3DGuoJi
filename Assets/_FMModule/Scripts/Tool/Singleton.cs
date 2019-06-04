using UnityEngine;
using System.Collections;

public class Singleton<T>: MonoBehaviour 
    where T:Component
{

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof (T)) as T;
                if (instance == null)
                {
                    Debuge.LogError("场景中缺少该单例");
                }
            }
            return instance;
        }
    }

}
