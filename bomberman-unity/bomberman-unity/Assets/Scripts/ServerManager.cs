using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    private static ServerManager instance;

    private IServer server;

    private void Awake()
    {
        Debug.Assert(instance == null);
        instance = this;

        server = new MockServer();
    }

    public static IServer GetServer()
    {
        return instance.server;
    }
}
