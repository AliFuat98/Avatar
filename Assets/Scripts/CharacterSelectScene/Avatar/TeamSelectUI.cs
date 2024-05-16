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

  [SerializeField] private Button startGameButton;

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

    startGameButton.onClick.AddListener(() => {
      if (!CanGameStart()) {
        return;
      }

      GameLobby.Instance.DeleteLobby();
      Loader.LoadNetwork(Loader.Scene.GameScene);
    });
  }

  private void Start() {
    try {
      if (!NetworkManager.Singleton.IsHost) {
        startGameButton.gameObject.SetActive(false);
      }

      Lobby joinedLobby = GameLobby.Instance.GetJoinedLobby();

      lobbyNameText.text = $"Lobby Name \n {joinedLobby.Name}";
    } catch (Exception) {
    }
  }

  private bool CanGameStart() {
    var playerDataList = GameMultiplayer.Instance.GetPlayerDataList();

    int tellerCount = 0;
    int firstTeamCount = 0;
    int secondTeamCount = 0;
    foreach (var playerData in playerDataList) {
      if (playerData.isTeller) {
        tellerCount++;
      }

      if (playerData.teamId == 0 && !playerData.isTeller) {
        firstTeamCount++;
      }

      if (playerData.teamId == 2 && !playerData.isTeller) {
        secondTeamCount++;
      }
    }

    if (tellerCount != 2) {
      Debug.Log("teller is empty");
      return false;
    }

    if (firstTeamCount < 1) {
      Debug.Log("first team is empty");
      return false;
    }

    if (secondTeamCount < 1) {
      Debug.Log("second team is empty");
      return false;
    }

    return true;
  }
}