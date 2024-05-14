using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectUI : MonoBehaviour {
  [SerializeField] private Button mainMenuButton;

  [SerializeField] private Button leftTeamButton;
  [SerializeField] private Button middleTeamButton;
  [SerializeField] private Button rightTeamButton;

  [SerializeField] private Button leftTellerButton;
  [SerializeField] private Button rightTellerButton;

  [SerializeField] private TextMeshProUGUI lobbyNameText;
  [SerializeField] private TextMeshProUGUI lobbyCodeText;

  private void Awake() {
    mainMenuButton.onClick.AddListener(() => {
      GameLobby.Instance.LeaveLobby();
      NetworkManager.Singleton.Shutdown();

      Loader.Load(Loader.Scene.MainMenuScene);
    });

    leftTeamButton.onClick.AddListener(() => {
      GameMultiplayer.Instance.ChangePlayerTeam(0);
    });

    middleTeamButton.onClick.AddListener(() => {
      GameMultiplayer.Instance.ChangePlayerTeam(1);
    });

    rightTeamButton.onClick.AddListener(() => {
      GameMultiplayer.Instance.ChangePlayerTeam(2);
    });

    leftTellerButton.onClick.AddListener(() => {
      GameMultiplayer.Instance.ChangeTeller(0);
    });

    rightTellerButton.onClick.AddListener(() => {
      GameMultiplayer.Instance.ChangeTeller(2);
    });
  }

  private void Start() {
    try {
      Lobby joinedLobby = GameLobby.Instance.GetJoinedLobby();

      lobbyNameText.text = $"Lobby Name \n {joinedLobby.Name}";
    } catch (Exception e) {
      Debug.Log(e.Message);
    }
  }
}