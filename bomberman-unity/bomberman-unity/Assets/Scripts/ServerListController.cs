using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ServerListController : MonoBehaviour
{
    [SerializeField]
    private ServerListView view;

    private IEnumerable<LobbyData> latestLobbyData;

    private void Awake()
    {
        view.OnExit += Exit;
        view.OnLobbiesRefresh += FetchAndRefreshLobbies;
        view.OnLobbiesFilterChanged += RefreshLobbies;
        view.OnCreateLobby += CreateLobby;
    }

    private void OnDestroy()
    {
        view.OnExit -= Exit;
        view.OnLobbiesRefresh -= FetchAndRefreshLobbies;
        view.OnLobbiesFilterChanged -= RefreshLobbies;
        view.OnCreateLobby -= CreateLobby;
    }

    private void Exit()
    {
        ViewManager.SwitchToView<MainMenuView>();
    }

    private void CreateLobby()
    {
        var server = ServerManager.GetServer();
        server.CreateLobby();
        FetchAndRefreshLobbies();
    }

    private IEnumerable<LobbyData> FilterLobbies(IEnumerable<LobbyData> lobbies, string filter)
    {

        if (string.IsNullOrEmpty(view.LobbyFilter))
        {
            return lobbies.ToList();
        }

        var result = new List<LobbyData>();

        foreach (var lobby in lobbies)
        {
            if (lobby.id.StartsWith(filter))
            {
                result.Add(lobby);
            }
        }

        return result;
    }

    private void RefreshLobbies(string filter)
    {
        var data = FilterLobbies(latestLobbyData, filter);
        view.SetLobbiesView(data);
    }

    private void FetchAndRefreshLobbies()
    {
        var server = ServerManager.GetServer();
        latestLobbyData = server.GetLobbiesList();
        RefreshLobbies(view.LobbyFilter);
    }
}
