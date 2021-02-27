using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServer
{
    IEnumerable<LobbyData> GetLobbiesList();
    LobbyData CreateLobby();
    LobbyData EnterLobby(string lobbyId, string playerName);
}
