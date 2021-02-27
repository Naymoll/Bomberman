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

    public LobbyData EnterLobby(string lobbyId, string playerName)
    {
        LobbyData result = null;

        var lobby = lobbies.Find( it => it.Id == lobbyId);

        if (lobby != null)
        {
            lobby.Players.Add(new PlayerData
            {
                Name = playerName,
                Id = (++playerSerialCount).ToString()
            });

            result = new LobbyData { Id = lobby.Id, Players = lobby.Players};
        }

        return result;
    }
}
