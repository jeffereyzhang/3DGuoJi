using UnityEngine;
using System.Collections.Generic;

public class PoolManager : Singleton<PoolManager>
{

    public bool willGrow = true;
    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<PoolObjectID, List<GameObject>> poolDic = new Dictionary<PoolObjectID, List<GameObject>>();

    /// <summary>
    /// 对象池中激活的物体
    /// </summary>
    //private Dictionary<PoolObjectID, List<GameObject>> activeDic = new Dictionary<PoolObjectID, List<GameObject>>();
    
    private Dictionary<PoolObjectID, GameObject> prefabDic = new Dictionary<PoolObjectID, GameObject>();

   

    // Use this for initialization
	void Start ()
    {
    }
    /// <summary>
    /// 将需要对象池管理的物体注册到对象池
    /// </summary>
    /// <param name="id"></param>
    /// <param name="go"></param>
    public void Register(PoolObjectID id ,GameObject go)
    {
        if (!prefabDic.ContainsKey(id))
        {
            prefabDic.Add(id,go);
            poolDic.Add(id, new List<GameObject>());
        }
    }

    /// <summary>
    /// 从对象池获取物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetObjectFromPool(PoolObjectID id)
    {
        if (poolDic.ContainsKey(id))
        {
            //先看对象池中是否有空闲的物体
            for (int i = 0; i < poolDic[id].Count; i++)
            {
                GameObject go = poolDic[id][i];
                if (!go.activeSelf)
                {
                    go.SetActive(true);
                    return go;
                }
            }
            //如果没有就新建
            if (willGrow)
            {
                GameObject go = Instantiate(prefabDic[id]);
                poolDic[id].Add(go);
                return go;
            }
        }
        else
        {
            Debug.Log("还没有在对象池中注册该物体");
        }
       
        return null;
    }
    /// <summary>
    /// 对象池回收物体
    /// </summary>
    /// <param name="obj"></param>
    public void HideObj(GameObject obj)
    {
        obj.SetActive(false);
    }
}
