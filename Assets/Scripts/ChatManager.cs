using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _messageInputfield;
    [SerializeField] private TextMeshProUGUI _messageTemplate;
    [SerializeField] private GameObject _messagesContainer;

    private void Start()
    {
        _messageTemplate.text = "";
    }

    private void OnEnable()
    {
        SteamMatchmaking.OnChatMessage += ChatSent;
        SteamMatchmaking.OnLobbyEntered += LobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += LobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += LobbyMemberLeave;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnChatMessage -= ChatSent;
        SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
        
        SteamMatchmaking.OnLobbyMemberJoined -= LobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= LobbyMemberLeave;
    }

    private void AddMessageToBox(string arg)
    {
        GameObject message = Instantiate(_messageTemplate.gameObject, _messagesContainer.transform);
        message.GetComponent<TextMeshProUGUI>().text = arg;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ToggleChatBox();
        }
    }

    private void ToggleChatBox()
    {
        if (_messageInputfield.gameObject.activeSelf)
        {
            if (!String.IsNullOrEmpty((_messageInputfield.text)))
            {
                LobbySaver.Instance.CurrentLobby?.SendChatString(_messageInputfield.text);
                _messageInputfield.text = "";
            }

            _messageInputfield.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            _messageInputfield.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_messageInputfield.gameObject);
        }
    }
    
    
    private void LobbyMemberLeave(Lobby lobby, Friend friend)
    {
        AddMessageToBox(friend.Name + " - " + "Leaved");
    }

    private void LobbyMemberJoined(Lobby lobby, Friend friend)
    {
        AddMessageToBox(friend.Name + " - " + "Joined");
    }

    private void LobbyEntered(Lobby lobby)
    {
        AddMessageToBox("You enter the lobby");
    }
    
    private void ChatSent(Lobby lobby, Friend friend, string arg)
    {
        AddMessageToBox(friend.Name + ": " + arg);
    }
}