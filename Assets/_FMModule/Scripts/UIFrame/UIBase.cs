using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
    {
        /// <summary>
        /// 返回上一级的按钮
        /// </summary>
        public UIButton BackBtn;
		/// <summary>
		///当前UI的上一级UI，用于返回
		/// </summary>
        protected UIBase BackUi;
		/// <summary>
		/// 所在的组
		/// </summary>
		/// <value>The group.</value>
        public UIGroup Group { get; set; }
		/// <summary>
		/// 隐藏当前UI
		/// </summary>
		/// <param name="isVisible">If set to <c>true</c> is visible.</param>
        public virtual void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public bool IsVisible { get { return gameObject.activeSelf; } }
		/// <summary>
		/// 打开组
		/// </summary>
		/// <param name="args">Arguments.</param>
        public virtual void OpenWindow(params object[] args)
        {
            SetVisible(true);
        }
    }
