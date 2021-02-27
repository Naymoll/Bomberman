using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name;
    public string Id;
}

[System.Serializable]
public class LobbyData
{
    public string Id;
    public IEnumerable<PlayerData> Players;
}


