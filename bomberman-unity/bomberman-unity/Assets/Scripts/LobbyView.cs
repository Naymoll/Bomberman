using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : ViewBase
{
    [SerializeField]
    private TMP_Text txt_LobbyID;

    [SerializeField]
    private Button btn_Ready;

    [SerializeField]
    private Button btn_Exit;

    [SerializeField]
    private GameObject grpPlayersContent;

    public event Action OnExit;

    public event Action OnReady;

    private void Awake()
    {
        btn_Exit.onClick.AddListener(OnBtnExitClick);
    }

    private void OnDestroy()
    {
        btn_Exit.onClick.RemoveListener(OnBtnExitClick);
    }

    private void OnBtnExitClick()
    {
        OnExit?.Invoke();
    }
}
