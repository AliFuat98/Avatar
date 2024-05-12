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

  [SerializeField] private TextMeshProUGUI leftTellerNameText;
  [SerializeField] private TextMeshProUGUI RightTellerNameText;

  [SerializeField] private TextMeshProUGUI lobbyNameText;
  [SerializeField] private TextMeshProUGUI lobbyCodeText;

  [SerializeField] private GameObject[] teamContainers;
  [SerializeField] private GameObject playerNameTextPrefab;
  [SerializeField] private int heightPerChild = 70;

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

    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_OnPlayerDataNetworkListChange;
  }

  private void GameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs e) {
    UpdateTeamUI();
  }

  private void Start() {
    try {
      Lobby joinedLobby = GameLobby.Instance.GetJoinedLobby();

      lobbyNameText.text = $"Lobby Name \n {joinedLobby.Name}";

      UpdateTeamUI();
    } catch (Exception e) {
      Debug.Log(e.Message);
    }
  }

  private void UpdateTeamUI() {
    ClearExistingTexts();

    var dataList = GameMultiplayer.Instance.GetPlayerDataList();

    int[] childCountList = new int[3] { 0, 0, 0 };
    int[] tellerCount = new int[3] { 0, 0, 0 };

    foreach (PlayerData playerData in dataList) {
      if (playerData.isTeller) {
        if (playerData.teamId == 0) {
          leftTellerNameText.text = playerData.playerName.ToString();
        } else if (playerData.teamId == 2) {
          RightTellerNameText.text = playerData.playerName.ToString();
        }

        tellerCount[playerData.teamId]++;

        continue;
      }

      GameObject container = teamContainers[playerData.teamId];

      GameObject tmpInstance = Instantiate(playerNameTextPrefab, container.transform);
      tmpInstance.GetComponentInChildren<TextMeshProUGUI>().text = playerData.playerName.ToString();

      childCountList[playerData.teamId]++;
    }

    // delete teller
    if (tellerCount[0] == 0) {
      leftTellerNameText.text = string.Empty;
    } else if (tellerCount[0] > 1) {
      leftTellerNameText.text = "HATA VAR";
    }

    if (tellerCount[2] == 0) {
      RightTellerNameText.text = string.Empty;
    } else if (tellerCount[2] > 1) {
      RightTellerNameText.text = "HATA VAR";
    }

    AdjustContainerHeights(childCountList);
  }

  void AdjustContainerHeights(int[] childCountList) {
    int index = 0;
    foreach (GameObject container in teamContainers) {
      RectTransform containerRectTransform = container.GetComponent<RectTransform>();

      float newHeight = childCountList[index] * heightPerChild;
      containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, newHeight);

      index++;
    }
  }

  void ClearExistingTexts() {
    foreach (var container in teamContainers) {
      foreach (Transform child in container.transform) {
        Destroy(child.gameObject);
      }
    }
  }
}