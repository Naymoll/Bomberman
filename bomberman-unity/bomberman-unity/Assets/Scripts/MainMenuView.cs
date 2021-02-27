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

    public event Action<string> OnPlayerNameChanged;

    public string PlayerName { get; private set; }

    public void SetMultiplayerAvailable(bool available)
    {
        btn_Multiplayer.interactable = available;
    }

    public void SetPlayerName(string name)
    {
        if_PlayerName.text = string.Copy(name);
    }

    private void Awake()
    {
        btn_Multiplayer.onClick.AddListener(OnBtnMultiplayerClick);
        if_PlayerName.onValueChanged.AddListener(OnIfPlayerNameValueChange);
    }

    private void OnDestroy()
    {
        btn_Multiplayer.onClick.RemoveListener(OnBtnMultiplayerClick);
        if_PlayerName.onValueChanged.RemoveListener(OnIfPlayerNameValueChange);
    }

    private void OnBtnMultiplayerClick()
    {
        OnOpenServerList?.Invoke();
    }

    private void OnIfPlayerNameValueChange(string arg)
    {
        OnPlayerNameChanged?.Invoke(if_PlayerName.text);
        PlayerName = if_PlayerName.text;
    }
}
