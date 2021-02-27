using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : PersistantSceneObject<ServerManager>
{
    private IServer server;

    protected override void Awake()
    {
        base.Awake();
        server = new MockServer();
    }

    public static IServer GetServer()
    {
        return GetInstance().server;
    }
}
