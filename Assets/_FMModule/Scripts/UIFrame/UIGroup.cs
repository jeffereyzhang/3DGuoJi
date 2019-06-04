using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

    public class UIGroup : MonoBehaviour
    {
        private Dictionary<string, UIBase> UIBaseDic = new Dictionary<string, UIBase>();
        private bool isInit = false;

        void Awake()
        {
			//获取UI组下所有的BaseUI，先加入这个组，然后将这个组加入UIContainer的组的字典中
            UIBase[] uis = transform.GetComponentsInChildren<UIBase>(true);
            foreach (UIBase ui in uis)
            {
                ui.Group = this;
                string uiName = ui.GetType().Name;
                if (!UIBaseDic.ContainsKey(uiName))
                {
                    UIBaseDic.Add(uiName, ui);
                }
               // ui.gameObject.SetActive(false);
            }
            Debug.Log(gameObject.name + " load ui count = " + UIBaseDic.Count);
            UIContainer.Instance.AddUIGroup(this);
            isInit = true;
            //uis[0].gameObject.SetActive(true);
		}
		/// <summary>
		/// 获取组中某个名字的UI
		/// </summary>
		/// <returns>The U.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public T GetUI<T>() where T : UIBase
        {
            UIBase ui = null;
            if (UIBaseDic.TryGetValue(typeof(T).Name, out ui))
            {
                return ui as T;
            }
            return null;
        }
		/// <summary>
		/// 获取组中所有的UI
		/// </summary>
		/// <returns>The U is.</returns>
        public List<UIBase> GetUIs()
        {
            return new List<UIBase>(UIBaseDic.Values);
        }
		/// <summary>
		/// 设置组中某个UI的状态
		/// </summary>
		/// <param name="isVisible">If set to <c>true</c> is visible.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
        public void SetVisible<T>(bool isVisible) where T : UIBase
        {
            string uiName = typeof(T).Name;
            if (UIBaseDic.ContainsKey(uiName))
            {
                UIBaseDic[uiName].SetVisible(isVisible);
            }
        }
		/// <summary>
		/// 隐藏这个组
		/// </summary>
        public void HideAll()
        {
            foreach (KeyValuePair<string, UIBase> ui in UIBaseDic)
            {
                if (ui.Value.IsVisible)
                {
                    ui.Value.SetVisible(false);
                }
            }
        }
    }


