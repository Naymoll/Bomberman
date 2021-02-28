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
    private GameObject grp_PlayersContent;

    [SerializeField]
    private GameObject playerItemPrefab;

    public event Action OnExit;

    public event Action OnReady;

    public void SetLobbyId(string id)
    {
        txt_LobbyID.text = id;
    }

    public void SetPlayersView(IEnumerable<PlayerData> data)
    {
        ClearPlayersView();

        foreach (var player in data)
        {
            var obj = Instantiate(playerItemPrefab);
            obj.transform.SetParent(grp_PlayersContent.transform, false);
            var item = obj.GetComponent<UIPlayerItem>();
            Debug.Assert(item != null);
            item.SetText(player.Name, player.Ready);
        }
    }

    public void ClearPlayersView()
    {
        var items = grp_PlayersContent.GetComponentsInChildren<UIPlayerItem>();
        foreach (var item in items)
        {
            GameObject.Destroy(item.gameObject);
        }
    }

    private void Awake()
    {
        btn_Exit.onClick.AddListener(OnBtnExitClick);
        btn_Ready.onClick.AddListener(OnBtnReadyClick);
    }

    private void OnDestroy()
    {
        btn_Exit.onClick.RemoveListener(OnBtnExitClick);
        btn_Ready.onClick.RemoveListener(OnBtnReadyClick);

    }

    private void OnBtnExitClick()
    {
        OnExit?.Invoke();
    }

    private void OnBtnReadyClick()
    {
        OnReady?.Invoke();
    }
}
