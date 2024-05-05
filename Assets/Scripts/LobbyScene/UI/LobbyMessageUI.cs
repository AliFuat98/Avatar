using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {
  [SerializeField] private Button closeButton;
  [SerializeField] private TextMeshProUGUI messageText;

  private void Awake() {
    closeButton.onClick.AddListener(() => {
      Hide();
    });
  }

  private void Start() {
    GameMultiplayer.Instance.OnFailedToJoinGame += GameMultiplayer_OnFailedToJoinGame;

    GameLobby.Instance.OnCreateLobbyFailedStarted += GameLobby_OnCreateLobbyFailedStarted;
    GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;

    GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
    GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
    Hide();
  }

  private void OnDestroy() {
    GameMultiplayer.Instance.OnFailedToJoinGame -= GameMultiplayer_OnFailedToJoinGame;

    GameLobby.Instance.OnCreateLobbyFailedStarted -= GameLobby_OnCreateLobbyFailedStarted;
    GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;

    GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
    GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
  }

  private void GameLobby_OnJoinFailed(object sender, System.EventArgs e) {
    ShowMessage("Failed to join lobby...");
  }

  private void GameLobby_OnJoinStarted(object sender, System.EventArgs e) {
    ShowMessage("Joining lobby..");
  }

  private void GameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
    ShowMessage("Creating lobby...");
  }

  private void GameLobby_OnCreateLobbyFailedStarted(object sender, System.EventArgs e) {
    ShowMessage("Fail to Create lobby!");
  }

  private void GameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
    if (NetworkManager.Singleton.DisconnectReason == "") {
      ShowMessage("Failed to Connect");
    } else {
      ShowMessage(NetworkManager.Singleton.DisconnectReason);
    }
  }

  private void ShowMessage(string message) {
    Show();

    messageText.text = message;
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}