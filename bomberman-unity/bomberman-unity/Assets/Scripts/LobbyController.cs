using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    [SerializeField]
    private LobbyView view;

    private void Awake()
    {
        view.OnExit += Exit;
        view.OnShow += OnViewShow;
        view.OnHide += OnViewHide;
        view.OnReady += SetPlayerReady;
    }

    private void OnDestroy()
    {
        view.OnExit -= Exit;
        view.OnShow -= OnViewShow;
        view.OnHide -= OnViewHide;
        view.OnReady -= SetPlayerReady;
    }

    private void SetPlayerReady()
    {
        var server = ServerManager.GetServer();
        var gameState = GameState.GetInstance();
        server.SetPlayerReady(gameState.LobbyId, gameState.PlayerId);
    }

    private void OnViewShow()
    {
        var gameState = GameState.GetInstance();

        view.SetLobbyId("#" + gameState.LobbyId);

        view.ClearPlayersView();

        FetchAndRefreshPlayers();

        var server = ServerManager.GetServer();
        server.OnLobbyPlayersChanged += FetchAndRefreshPlayers;
        server.OnConnectionLost += HandleConnectionLost;
    }

    private void OnViewHide()
    {
        var server = ServerManager.GetServer();
        server.OnLobbyPlayersChanged -= FetchAndRefreshPlayers;
        server.OnConnectionLost -= HandleConnectionLost;
    }

    private void FetchAndRefreshPlayers()
    {
        view.ClearPlayersView();

        var server = ServerManager.GetServer();
        var gameState = GameState.GetInstance();
        var players = server.GetPlayersInLobby(gameState.LobbyId, gameState.PlayerId);

        if (players != null)
        {
            view.SetPlayersView(players);
        }

        Debug.Log("Refresh players");
    }

    private void HandleConnectionLost()
    {
        ViewManager.SwitchToView(typeof(MainMenuView));
    }

    private void Exit()
    {
        var server = ServerManager.GetServer();
        var gameState = GameState.GetInstance();
        server.DisconnectPlayer(gameState.LobbyId, gameState.PlayerId);

        ViewManager.SwitchToView(typeof(MainMenuView));
    }
}
