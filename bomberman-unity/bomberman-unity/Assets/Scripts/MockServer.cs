using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockLobbyData
{
    public string Id;
    public List<PlayerData> Players = new List<PlayerData>();

    public MockLobbyData(int id)
    {
        Id = id.ToString();
    }
}

public class MockServer : IServer
{
    private List<MockLobbyData> lobbies = new List<MockLobbyData>();

    private int serialCount;

    private int playerSerialCount;

    public event Action OnLobbyPlayersChanged;

    public event Action OnConnectionLost;

    public IEnumerable<LobbyData> GetLobbiesList()
    {
        return lobbies.ConvertAll<LobbyData>(it => new LobbyData { Id = it.Id, Players = it.Players });
    }

    public LobbyData CreateLobby()
    {
        var lobby = new MockLobbyData(++serialCount);
        lobbies.Add(lobby);
        return new LobbyData { Id = lobby.Id, Players = lobby.Players};
    }

    public string EnterLobby(string lobbyId, string playerName)
    {
        string result = null;

        var lobby = lobbies.Find( it => it.Id == lobbyId);


        if (lobby != null)
        {
            var playerId = (++playerSerialCount).ToString();

            lobby.Players.Add(new PlayerData
            {
                Name = playerName,
                Id = playerId
            });

            result = playerId;
        }

        return result;
    }

    public IEnumerable<PlayerData> GetPlayersInLobby(string lobbyId, string playerId)
    {
        IEnumerable<PlayerData> result = null;

        var lobby = lobbies.Find( it => it.Id == lobbyId);
        if (lobby != null)
        {
            result = lobby.Players;
        }

        return result;
    }

    public bool SetPlayerReady(string lobbyId, string playerId)
    {
        bool result = false;
        var lobby = lobbies.Find( it => it.Id == lobbyId);
        if (lobby != null)
        {
            var player = lobby.Players.Find(it => it.Id == playerId);
            if (player != null)
            {
                player.Ready = true;
                OnLobbyPlayersChanged?.Invoke();
                result = true;
            }
        }

        return result;
    }

    public void DisconnectPlayer(string lobbyId, string playerId)
    {
        var lobby = lobbies.Find(it => it.Id == lobbyId);
        if (lobby != null)
        {
            var player = lobby.Players.Find(it => it.Id == playerId);
            if (player != null)
            {
                lobby.Players.Remove(player);
                OnLobbyPlayersChanged?.Invoke();
            }
        }
    }


}
