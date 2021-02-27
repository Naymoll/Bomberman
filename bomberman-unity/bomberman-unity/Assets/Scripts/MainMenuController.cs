using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private MainMenuView view;

    private void Awake()
    {
        view.OnOpenServerList += OpenServerList;
        view.OnPlayerNameChanged += OnPlayerNameChanged;
        view.OnShow += OnViewShow;
    }

    private void OnDestroy()
    {
        view.OnOpenServerList -= OpenServerList;
        view.OnPlayerNameChanged -= OnPlayerNameChanged;
        view.OnShow -= OnViewShow;
    }

    private void OpenServerList()
    {
        ViewManager.SwitchToView(typeof(ServerListView));
    }

    private void OnViewShow()
    {

        if (!string.IsNullOrEmpty(GameState.GetInstance().PlayerName))
        {
            view.SetPlayerName(GameState.GetInstance().PlayerName);
            view.SetMultiplayerAvailable(true);
        }
        else
        {
            view.SetMultiplayerAvailable(false);
        }
    }

    private void OnPlayerNameChanged(string name)
    {
        GameState.GetInstance().PlayerName = string.Copy(name);
        view.SetMultiplayerAvailable(!string.IsNullOrEmpty(GameState.GetInstance().PlayerName));
    }
}
