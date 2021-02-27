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
    }

    private void OnDestroy()
    {
        view.OnExit -= Exit;
    }

    private void Exit()
    {
        ViewManager.SwitchToView<MainMenuView>();
    }
}
