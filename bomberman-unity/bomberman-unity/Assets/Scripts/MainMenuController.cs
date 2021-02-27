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
    }

    private void OnDestroy()
    {
        view.OnOpenServerList -= OpenServerList;
    }

    private void OpenServerList()
    {
        ViewManager.SwitchToView<ServerListView>();
    }
}
