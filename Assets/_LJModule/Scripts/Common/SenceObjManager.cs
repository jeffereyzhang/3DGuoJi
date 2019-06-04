using UnityEngine;
using System.Collections;

public class SenceObjManager : MonoBehaviour
{
    public SceneObjItemList _CompanyOffice;
    public SceneObjItemList _Port;
    public SceneObjItemList _HuoDaiCompany;
    public SceneObjItemList _HaiGuanDaTing;
    public SceneObjItemList _JianYanJianYiDaTing;


    void Start()
    {
        switch (GameManager._curTaskType)
        {
          
            case TaskType.进口报关_委托报关:
            case TaskType.进口报检_委托报检:
                StartCoroutine(InitSenceObject(_CompanyOffice));
                break;

            case TaskType.进口报关_电子申报:
            case TaskType.进口报检_电子申报:
                StartCoroutine(InitSenceObject(_HuoDaiCompany));
                break;

            case TaskType.进口报关_现场查验:
            case TaskType.进口报检_实施检验检疫:
                StartCoroutine(InitSenceObject(_Port));
                break;

            case TaskType.进口报检_出证放行:
            case TaskType.进口报检_审核报检材料:
                StartCoroutine(InitSenceObject(_JianYanJianYiDaTing));
                break;
            
            case TaskType.进口报关_审核单证:
            case TaskType.进口报关_通关放行:
                StartCoroutine(InitSenceObject(_HaiGuanDaTing));
                break;



        }
    }

    void Update()
    {

    }


    public IEnumerator InitSenceObject(SceneObjItemList _ObjItem)
    {
        foreach (GameObject go in _ObjItem.buildItemList)
        {
            go.SetActive(true);
            yield return new WaitForEndOfFrame();
        }
    }

}
