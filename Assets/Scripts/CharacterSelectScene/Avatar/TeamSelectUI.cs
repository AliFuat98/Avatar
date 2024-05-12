using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectUI : MonoBehaviour {
  [SerializeField] private Button mainMenuButton;
  [SerializeField] private Button leftTeamButton;
  [SerializeField] private Button rightTeamButton;
  [SerializeField] private Button leftTellerButton;
  [SerializeField] private Button rightTellerButton;

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
      TeamSelect.Instance.ChangeTeam(leftSite: true);
    });

    rightTeamButton.onClick.AddListener(() => {
      TeamSelect.Instance.ChangeTeam(leftSite: false);
    });

    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_OnPlayerDataNetworkListChange;
  }

  private void GameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs e) {
    UpdateTeamUI();
  }

  private void Start() {
    try {
      Lobby joinedLobby = GameLobby.Instance.GetJoinedLobby();

      lobbyNameText.text = $"Lobby Name: {joinedLobby.Name}";

      UpdateTeamUI();
    } catch (Exception e) {
      Debug.Log(e.Message);
    }
  }

  private void UpdateTeamUI() {
    ClearExistingTexts();

    var dataList = GameMultiplayer.Instance.GetPlayerDataList();

    int[] childCountList = new int[3] { 0, 0, 0 };

    foreach (PlayerData playerData in dataList) {
      GameObject container = teamContainers[playerData.teamId];

      GameObject tmpInstance = Instantiate(playerNameTextPrefab, container.transform);
      tmpInstance.GetComponentInChildren<TextMeshProUGUI>().text = playerData.playerName.ToString();

      childCountList[playerData.teamId]++;
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