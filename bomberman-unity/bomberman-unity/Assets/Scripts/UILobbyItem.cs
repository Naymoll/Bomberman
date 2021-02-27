using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Button btn_LobbyItem;

    private string lobbyId;

    public event Action<string> OnClick;

    public void SetText(string newText, string newId)
    {
        text.text = newText;
        lobbyId = newId;
    }

    private void OnBtnLobbyItemClick()
    {
        OnClick?.Invoke(lobbyId);
    }

    private void Awake()
    {
        btn_LobbyItem.onClick.AddListener(OnBtnLobbyItemClick);
    }

    private void OnDestroy()
    {
        btn_LobbyItem.onClick.RemoveListener(OnBtnLobbyItemClick);
    }
}
