using System;
using TMPro;
using UnityEngine;

public class TeamPanelUI : MonoBehaviour {
  [SerializeField] private int teamId = -1;
  [SerializeField] private TextMeshProUGUI tellerNameText;

  [SerializeField] private GameObject teamNameContainer;
  [SerializeField] private GameObject playerNameTextPrefab;
  [SerializeField] private int heightPerChild = 70;

  private void Awake() {
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_OnPlayerDataNetworkListChange;
  }

  private void GameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs e) {
    UpdatePanelUI();
  }

  private void Start() {
    if (teamId == -1) {
      Debug.LogError("assign teamid of the team panel");
      return;
    }
    UpdatePanelUI();
  }

  private void UpdatePanelUI() {
    ClearExistingTexts();

    var playerNames = GameMultiplayer.Instance.GetPlayerNamesWithTeamId(teamId);
    //List<string> playerNames = new() {
    //  "ali","ebrar","yusuf",
    //};

    int childCount = 0;
    foreach (var name in playerNames) {
      GameObject tmpInstance = Instantiate(playerNameTextPrefab, teamNameContainer.transform);
      tmpInstance.GetComponentInChildren<TextMeshProUGUI>().text = name;
      childCount++;
    }

    var tellerName = GameMultiplayer.Instance.GetTellerNameWithTeamId(teamId);
    //string tellerName = "Ahmet";
    tellerNameText.text = tellerName;

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