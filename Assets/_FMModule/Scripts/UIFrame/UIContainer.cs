using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIContainer : Singleton<UIContainer>
{
    public List<UIGroup> UIGroups = new List<UIGroup>();

	/// <summary>
	/// 添加UI组
	/// </summary>
	/// <param name="group">Group.</param>
    public void AddUIGroup(UIGroup group)
    {
        if (!UIGroups.Contains(group))
        {
            UIGroups.Add(group);
        }
    }
	/// <summary>
	/// 获取UI组中的某个特定UI
	/// </summary>
	/// <returns>The U.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
    public T GetUI<T>() where T : UIBase
    {
        UIBase ui = null;
        foreach (var g in UIGroups)
        {
            ui = g.GetUI<T>();
            if (ui != null)
            {
                return ui as T;
            }
        }
        return null;
    }
	/// <summary>
	/// 获取UI组中的所有UI
	/// </summary>
	/// <returns>The U is.</returns>
    public List<UIBase> GetUIs()
    {
        List<UIBase> uis = new List<UIBase>();
        foreach (var g in UIGroups)
        {
            uis.AddRange(g.GetUIs());
        }
        return uis;
    }
	/// <summary>
	/// 隐藏UI组中的所有UI
	/// </summary>
    public void HideAll()
    {
        foreach (var group in UIGroups)
        {
            group.HideAll();
        }
    }
}
