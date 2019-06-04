using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    //List of players in the lobby
    public class LobbyPlayerList : Singleton<LobbyPlayerList>
    {
        //public static LobbyPlayerList _instance = null;

        public Transform playerListContentTransform;

        protected UIGrid _layout;
        public List<LobbyPlayer> _players = new List<LobbyPlayer>();

        public void Start()
        {
           // _instance = this;
            _layout = playerListContentTransform.GetComponent<UIGrid>();
        }

        public void AddPlayer(LobbyPlayer player)
        {
            if (_players.Contains(player))
                return;

            _players.Add(player);

            player.transform.SetParent(_layout.transform, false);

            _layout.repositionNow = true;
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            _players.Remove(player);
            _layout.repositionNow = true;
        }
    }
}
