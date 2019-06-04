using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

//������������
public class Helper:MonoBehaviour
{

    public static IEnumerator DelayToInvokeDo(Action action, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
        /**
        StartCoroutine(Helper.DelayToInvokeDo(() =>
        {
            Debug.Log("DelayToInvokeDo......");
        }, 0.3f));
        **/
    }

    //�õ���������
    public static GameObject GetObject(string name)
    {
        return GameObject.Find(name);
    }

    //�õ�����λ��
    public static Vector3 GetObjectPos(string name)
    {
        GameObject obj = GetObject(name); 
        return GetObjectPos(obj);
    }

    //�õ�����λ��
    public static Vector3 GetObjectPos(GameObject obj)
    {
        if (obj == null) { Debug.Log("GetObjectPos ������.");return Vector3.zero; }
        if (obj.GetComponent<Renderer>()) return obj.GetComponent<Renderer>().bounds.center; 

        Debug.Log("GetObjectPos����û�а�Χ��."+obj.name);
        return obj.transform.position;

    }

    //��������λ��
    public static void SetObjectPositon(string name,Vector3 pos)
    {
        GameObject obj = GetObject(name);
        SetObjectPositon(obj, pos);
    }

    //��������λ��
    public static void SetObjectPositon(GameObject obj, Vector3 pos)
    {
        if (obj == null) return;
        Vector3 cur_pos = GetObjectPos(obj);
        Vector3 offset = pos - cur_pos;
        obj.transform.Translate(offset,Space.World);
    }

    //����ģ��
    public static GameObject CloneObj(GameObject obj,Vector3 pos)
    {
        if (obj == null) return null;
        return (GameObject) UnityEngine.Object.Instantiate(obj, pos, obj.transform.rotation);
    }

    public static void DestroyObj(GameObject obj)
    {
        if (obj == null) return;
        GameObject.Destroy(obj);
    }
    public static void DestroyObj(string obj)
    {
        GameObject.Destroy(GetObject(obj));
    }

    //�ַ�ָ��ת��string
    public static string PCharToString(IntPtr pchar)
    {
       return  Marshal.PtrToStringUni(pchar);
    }

    //stringת��char*
    public static IntPtr StringToPChar(string s)
    {
       return  Marshal.StringToBSTR(s);
    }


    //�õ����������������壬��������Ҫ�и���������ײ��
    public static GameObject GetMousePickObj()
    {
        //�����㵽��ɶ
        Camera cam = Camera.main;
        if (cam == null) { Debug.Log("GetMousePickObj,�Ҳ���������"); return null; }
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit hitInfo = new RaycastHit();
        float distance = 100f;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            if (hitInfo.collider)
            {
                return hitInfo.collider.gameObject;
            }
            
        }
        return null;
    }


    //�õ�������������������������
    public static GameObject GetMousePickPhyObj()
    {
        //�����㵽��ɶ
        Camera cam = Camera.main;
        if (cam == null) { Debug.Log("GetMousePickPhyObj,�Ҳ���������"); return null; }
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        float distance = 100f;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {

            if (hitInfo.rigidbody && hitInfo.collider)
            {
                return hitInfo.rigidbody.gameObject;
            }
        }
        return null;
    }


    //�õ�����ʰȡ��λ��
    public static Vector3 GetMousePickPos()
    {
        Camera cam = Camera.main;
        if (cam == null) { Debug.Log("GetMousePickPos,�Ҳ���������"); return Vector3.zero; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        float distance = 100f;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            if (hitInfo.collider)
            {
                Debug.DrawLine(ray.origin, hitInfo.point);
                return hitInfo.point;
                
            }
        }
        return Vector3.zero;
    }

    //��ʾ����ģ�ͣ�������������
    public static void ShowObject(string name,bool isShow)
    {
        GameObject obj = GetObject(name);
        if (obj == null) return;
        MeshRenderer[] mrs = obj.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer mr in mrs)
        {
            mr.enabled = isShow;
        }
    }


    //�������Ƶ�ʱ��ת�ɱ�׼ʱ����ʽ00:00:00
    public static string SecondToTimeString(float t)
    {
        if(t<=0) return "00:00";
	    string timeStr="";
	
        //int h=Mathf.FloorToInt(t/3600%24);
        //if(h/10>=1)	timeStr+=""+h;
        //else timeStr+="0"+h;

        int m = Mathf.FloorToInt(t / 60 % 60);
	    if(m/10>=1) timeStr+=m;
	    else timeStr+="0"+m;

        int s = Mathf.FloorToInt(t % 60);
	    if(s/10>=1) timeStr+=":"+s;
	    else timeStr+=":0"+s;
	
	    return timeStr;
    }

    //�������Ƶ�ʱ��ת��ʱ���ַ���,��ʽ 2:02'22"
    public static string SecondToTimeString2(float t)
    {
        if (t <= 0) return "0\"";

        string timeStr = "";
        int h = Mathf.FloorToInt(t / 3600 % 24);
        if (h >0) timeStr += h.ToString()+":";
 
        int m = Mathf.FloorToInt(t / 60 % 60);
        timeStr += m.ToString()+"\'";

        int s = Mathf.FloorToInt(t % 60);
        timeStr += s.ToString("00") + "\"";

        return timeStr;
    }

    
    public static TimeSpan SecondToTimeSpan(float t)
    {
        int h = Mathf.FloorToInt(t / 3600 % 24);
        int m = Mathf.FloorToInt(t / 60 % 60);
        int s = Mathf.FloorToInt(t % 60);

        return new TimeSpan(h, m, s);
    }

    public static GameObject GetChildrenObject(GameObject parent,string childrenName)
    {
        if (parent == null || string.IsNullOrEmpty(childrenName)) return null;

        Transform[] trans = parent.GetComponentsInChildren<Transform>();
        foreach (Transform t in trans)
        {
            if (t.name == childrenName) return t.gameObject;
        }

        return null;

    }

}