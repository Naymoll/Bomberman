using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServer
{
    public event Action OnLobbyPlayersChanged;
    public event Action OnConnectionLost;

    IEnumerable<LobbyData> GetLobbiesList();
    LobbyData CreateLobby();
    string EnterLobby(string lobbyId, string playerName);
    IEnumerable<PlayerData> GetPlayersInLobby(string lobbyId, string playerId);
    bool SetPlayerReady(string lobbyId, string playerId);
    void DisconnectPlayer(string lobbyId, string playerId);
}
