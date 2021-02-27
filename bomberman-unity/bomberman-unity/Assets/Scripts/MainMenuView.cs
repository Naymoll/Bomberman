using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : ViewBase
{
    [SerializeField]
    private TMP_InputField if_PlayerName;

    [SerializeField]
    private Button btn_Multiplayer;

    public event Action OnOpenServerList;

    private void Awake()
    {
        btn_Multiplayer.onClick.AddListener(OnBtnMultiplayerClick);
    }

    private void OnDestroy()
    {
        btn_Multiplayer.onClick.RemoveListener(OnBtnMultiplayerClick);
    }

    private void OnBtnMultiplayerClick()
    {
        OnOpenServerList?.Invoke();
    }
}
