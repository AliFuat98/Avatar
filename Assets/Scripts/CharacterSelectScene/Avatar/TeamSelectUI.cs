using System;
using System.Collections;
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

    StartCoroutine(WaitForTeamSelect());
  }

  private IEnumerator WaitForTeamSelect() {
    while (TeamSelect.Instance == null) {
      yield return null; // wait for a frame
    }

    TeamSelect.Instance.OnTeamChanged += TeamSelect_OnTeamChanged;
  }

  private void Start() {
    try {
      Lobby joinedLobby = GameLobby.Instance.GetJoinedLobby();

      lobbyNameText.text = $"Lobby Name: {joinedLobby.Name}";
    } catch (Exception e) {
      Debug.Log(e.Message);
    }
  }

  private void TeamSelect_OnTeamChanged(object sender, System.EventArgs e) {
    UpdateTeamUI();
  }

  public void UpdateTeamUI() {
    ClearExistingTexts();

    var teamDic = TeamSelect.Instance.GetTeamDic();

    // to find the proper height
    int[] childCountList = new int[3] { 0, 0, 0 };

    foreach (var entry in teamDic) {
      int teamId = entry.Value.teamId;
      GameObject container = teamContainers[teamId];

      GameObject tmpInstance = Instantiate(playerNameTextPrefab, container.transform);
      tmpInstance.GetComponentInChildren<TextMeshProUGUI>().text = "Client ID: " + entry.Key.ToString();

      childCountList[teamId]++;
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