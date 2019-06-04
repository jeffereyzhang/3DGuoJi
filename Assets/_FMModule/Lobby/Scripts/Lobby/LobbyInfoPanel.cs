using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Prototype.NetworkLobby 
{
    public class LobbyInfoPanel : MonoBehaviour
    {
        public UILabel InfoLabel;
        public UILabel BtnLable;
        public UIButton CancelButton;

        public void Display(string info, string buttonInfo, Action buttonClbk = null)
        {
            InfoLabel.text = info;

            BtnLable.text = buttonInfo;

            CancelButton.onClick.Clear();

            if (buttonClbk != null)
            {
                EventDelegate.Add(CancelButton.onClick, () =>
                {
                    buttonClbk();
                });
            }

            gameObject.SetActive(true);
            //点击关闭该界面
            EventDelegate.Add(CancelButton.onClick, () =>
            {
                Debug.Log("关闭提示");
                gameObject.SetActive(false);
            });
        }
    }
}