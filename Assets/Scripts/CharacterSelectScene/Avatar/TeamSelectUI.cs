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
  }

  private void Start() {
    Lobby joinedLobby = GameLobby.Instance.GetJoinedLobby();

    lobbyNameText.text = $"Lobby Name: {joinedLobby.Name}";
    //lobbyCodeText.text = $"Lobby Name: {joinedLobby.LobbyCode}";
  }
}