using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class MyLobbyHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer,
        GameObject gamePlayer)
    {
        gamePlayer.GetComponent<Player>().Name = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
    }
}
