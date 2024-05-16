using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingleTeamPanelUI : MonoBehaviour {
  [SerializeField] private int teamId = -1;
  [SerializeField] private TextMeshProUGUI tellerNameText;
  [SerializeField] private TextMeshProUGUI remaningCardCountText;

  [SerializeField] private GameObject teamNameContainer;
  [SerializeField] private GameObject playerNameTextPrefab;
  [SerializeField] private int heightPerChild = 70;

  private void Awake() {
    try {
      GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_OnPlayerDataNetworkListChange;
    } catch (Exception) {
    }
  }

  private void GameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs e) {
    UpdatePanelUI();
  }

  private void Start() {
    if (teamId == -1) {
      MessageManager.Instance.SetText("ERROR: assign teamid of the team panel");
      return;
    }
    UpdatePanelUI();

    // card count
    if (TurnManager.Instance != null) {
      TurnManager.Instance.OnRemaningCardCountChanged += TurnManager_OnRemaningCardCountChanged;
      remaningCardCountText.gameObject.SetActive(true);
      UpdateRemainingCardCount();
    } else {
      remaningCardCountText.gameObject.SetActive(false);
    }
  }

  private void TurnManager_OnRemaningCardCountChanged(object sender, EventArgs e) {
    UpdateRemainingCardCount();
  }

  private void UpdateRemainingCardCount() {
    remaningCardCountText.text = $"({TurnManager.Instance.GetRemainingCardCount(teamId)})";
  }

  private void UpdatePanelUI() {
    ClearExistingTexts();

    List<string> playerNames = GameMultiplayer.Instance.GetPlayerNamesWithTeamId(teamId);
    string localPlayerName = GameMultiplayer.Instance.GetPlayerName();

    int childCount = 0;
    foreach (var name in playerNames) {
      GameObject tmpInstance = Instantiate(playerNameTextPrefab, teamNameContainer.transform);
      if (name == localPlayerName) {
        // ali fuat yýldýz deðiþtirelebilir.
        tmpInstance.GetComponentInChildren<TextMeshProUGUI>().text = name + " (*)";
      } else {
        tmpInstance.GetComponentInChildren<TextMeshProUGUI>().text = name;
      }
      childCount++;
    }

    string tellerName = GameMultiplayer.Instance.GetTellerNameWithTeamId(teamId);

    if (tellerName == localPlayerName) {
      tellerNameText.text = tellerName + " (*)";
    } else {
      tellerNameText.text = tellerName;
    }

    AdjustContainerHeights(childCount);
  }

  private void AdjustContainerHeights(int childCountList) {
    RectTransform containerRectTransform = teamNameContainer.GetComponent<RectTransform>();

    float newHeight = childCountList * heightPerChild;
    containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, newHeight);
  }

  private void ClearExistingTexts() {
    foreach (Transform child in teamNameContainer.transform) {
      Destroy(child.gameObject);
    }
  }
}