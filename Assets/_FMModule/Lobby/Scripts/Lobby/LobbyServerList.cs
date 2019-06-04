using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    public class LobbyServerList : MonoBehaviour
    {
        private LobbyManager lobbyManager;

        public Transform serverListRect;
        public GameObject serverEntryPrefab;
        public GameObject noServerFound;

        protected int currentPage = 0;
        protected int previousPage = 0;
        private float timer = 1f;
        private PoolManager pm;
        public RoomPageController pageController;

        private void Start()
        {
          
            lobbyManager = LobbyManager.Instance;
            currentPage = 0;
            previousPage = 0;
            noServerFound.SetActive(false);

            RequestPage(0);
            pm = PoolManager.Instance;
            pm.Register(PoolObjectID.ServerPrefab, serverEntryPrefab);
        }


        private void ChangePage(int dir)
        {
            int newPage = Mathf.Max(0, currentPage + dir);

            //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
            if (noServerFound.activeSelf)
                newPage = 0;

            RequestPage(newPage);
        }

        public void NextPage()
        {
            ChangePage(1);
        }
        public void PreviousPage()
        {
            ChangePage(-1);
        }

        public void RequestPage(int page)
        {
            previousPage = currentPage;
            currentPage = page;
        }

        private void Update()
        {

            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 3f;
                OnGUIMatchList();
            }

        }
        public void OnGUIMatchList()
        {

            if (lobbyManager.Discovery.broadcastsReceived == null)
            {
                //清空页面信息
                pageController.ClearPageInfo();

                foreach (Transform t in serverListRect)
                    pm.HideObj(t.gameObject);
                return; 
            }
                
           List<NetworkBroadcastResult> nr = new List<NetworkBroadcastResult>();
           nr.AddRange(lobbyManager.Discovery.broadcastsReceived.Values);

            if (nr.Count == 0)
            {
                if (currentPage == 0)
                {
                    noServerFound.SetActive(true);
                }
                //清空页面信息
                pageController.ClearPageInfo();

                foreach (Transform t in serverListRect)
                    pm.HideObj(t.gameObject);
                currentPage = previousPage;

                return;
            }

            noServerFound.SetActive(false);
            foreach (Transform t in serverListRect)
                pm.HideObj(t.gameObject);

            pageController.ItemList.Clear();
            foreach (var kv in nr)
            {
                GameObject o = pm.GetObjectFromPool(PoolObjectID.ServerPrefab);

                o.GetComponent<LobbyServerEntry>().Populate(kv);

                o.transform.SetParent(serverListRect, false);
                serverListRect.GetComponent<UIGrid>().repositionNow = true;
                pageController.ItemList.Add(o);
            }
            //更新页数信息
            pageController.UpdatePage();
            lobbyManager.Discovery.broadcastsReceived.Clear();
        }
    }
}