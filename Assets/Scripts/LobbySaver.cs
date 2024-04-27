using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks.Data;
using UnityEngine;

public class LobbySaver : MonoBehaviour
{
    public Lobby? CurrentLobby;
    public static LobbySaver Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}