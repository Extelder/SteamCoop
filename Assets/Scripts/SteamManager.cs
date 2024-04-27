using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using Steamworks.Data;

public class SteamManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _lobbyIdInputField;
    [SerializeField] private TextMeshProUGUI _lobbyID;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _inLobbyMenu;

    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += LobbyCreated;
        SteamMatchmaking.OnLobbyEntered += LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
    }

    private async void GameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        await lobby.Join();
    }

    private void LobbyEntered(Lobby lobby)
    {
        LobbySaver.Instance.CurrentLobby = lobby;
        _lobbyID.text = lobby.Id.ToString();
        CheckUI();

        Debug.Log("ENTERED LOBBY");
    }

    private void LobbyCreated(Result result, Lobby lobby)
    {
        if (result == Result.OK)
        {
            lobby.SetPublic();
            lobby.SetJoinable(true);
        }
    }


    public async void HostLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(4);
    }

    public async void JoinLobbyWithID()
    {
        ulong ID;
        if (!ulong.TryParse(_lobbyIdInputField.text, out ID))
            return;
        Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();

        foreach (Lobby lobby in lobbies)
        {
            if (lobby.Id == ID)
            {
                await lobby.Join();
                return;
            }
        }
    }

    public void CopyID()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = _lobbyID.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    public void LeaveLobby()
    {
        LobbySaver.Instance.CurrentLobby?.Leave();
        LobbySaver.Instance.CurrentLobby = null;
        CheckUI();
    }

    private void CheckUI()
    {
        if (LobbySaver.Instance.CurrentLobby == null)
        {
            _mainMenu.SetActive(true);
            _inLobbyMenu.SetActive(false);
        }
        else
        {
            _mainMenu.SetActive(false);
            _inLobbyMenu.SetActive(true);
        }
    }
}