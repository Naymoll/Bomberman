using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockServer : IServer
{
    private List<LobbyData> lobbies = new List<LobbyData>();

    private int serialCount;

    public IEnumerable<LobbyData> GetLobbiesList()
    {
        return lobbies;
    }

    public LobbyData CreateLobby()
    {
        var lobby = new LobbyData { id = (++serialCount).ToString() };
        lobbies.Add(lobby);
        return lobby;
    }
}
