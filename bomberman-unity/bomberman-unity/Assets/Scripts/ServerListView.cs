using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerListView : ViewBase
{
    [SerializeField]
    private Button btn_CreateLobby;

    [SerializeField]
    private Button btn_Exit;

    [SerializeField]
    private TMP_InputField if_Filter;

    [SerializeField]
    private GameObject grp_ServerListContent;

    [SerializeField]
    private Button btn_Refresh;

    [SerializeField]
    private GameObject lobbyItemPrefab;

    public event Action OnExit;

    public event Action OnCreateLobby;

    public event Action OnLobbiesRefresh;

    public event Action<string> OnLobbiesFilterChanged;

    public event Action<string> OnLobbySelected;

    public string LobbyFilter { get; private set; }

    public void SetLobbiesView(IEnumerable<LobbyData> data)
    {
        ClearLobbiesView();

        foreach (var lobby in data)
        {
            var obj = Instantiate(lobbyItemPrefab);
            obj.transform.SetParent(grp_ServerListContent.transform, false);
            var item = obj.GetComponent<UILobbyItem>();
            Debug.Assert(item != null);
            item.OnClick += OnLobbyItemClick;
            item.SetText("# " + lobby.Id, lobby.Id);
        }
    }

    private void ClearLobbiesView()
    {
        var items = grp_ServerListContent.GetComponentsInChildren<UILobbyItem>();
        foreach (var item in items)
        {
            item.OnClick -= OnLobbyItemClick;
            GameObject.Destroy(item.gameObject);
        }
    }

    private void Awake()
    {
        btn_Exit.onClick.AddListener(OnBtnExitClick);
        btn_Refresh.onClick.AddListener(OnBtnRefreshClick);
        btn_CreateLobby.onClick.AddListener(OnBtnCreateLobbyClick);
        if_Filter.onValueChanged.AddListener(OnIfFilterValueChange);
    }

    private void OnDestroy()
    {
        btn_Exit.onClick.RemoveListener(OnBtnExitClick);
        btn_Refresh.onClick.RemoveListener(OnBtnRefreshClick);
        btn_CreateLobby.onClick.RemoveListener(OnBtnCreateLobbyClick);
        if_Filter.onValueChanged.RemoveListener(OnIfFilterValueChange);

    }

    private void OnBtnExitClick()
    {
        OnExit?.Invoke();
    }

    private void OnBtnRefreshClick()
    {
        OnLobbiesRefresh?.Invoke();
    }

    private void OnBtnCreateLobbyClick()
    {
        OnCreateLobby?.Invoke();
    }

    private void OnIfFilterValueChange(string arg)
    {
        LobbyFilter = if_Filter.text;
        OnLobbiesFilterChanged?.Invoke(if_Filter.text);
        Debug.Log("OnIfFilterValueChange");
    }

    private void OnLobbyItemClick(string lobbyId)
    {
        OnLobbySelected?.Invoke(lobbyId);
    }
}
