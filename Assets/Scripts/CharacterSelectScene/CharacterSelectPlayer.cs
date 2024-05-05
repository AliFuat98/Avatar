using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {
  [SerializeField] private int playerIndex;
  [SerializeField] private Button kickButton;
  [SerializeField] private GameObject readyGameObject;
  [SerializeField] private TextMeshPro playerNameText;
  [SerializeField] private PlayerVisual playerVisual;

  private void Awake() {
    kickButton.onClick.AddListener(() => {
      PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
      GameMultiplayer.Instance.KickPlayer(playerData.clientId);
    });
  }

  private void Start() {
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_onPlayerDataNetworkListChange;
    CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_onReadyChanged;

    kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

    if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
      PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      if (NetworkManager.ServerClientId == playerData.clientId) {
        // kendini atma yok
        kickButton.gameObject.SetActive(false);
      }
    }

    UpdatePlayer();
  }

  private void OnDestroy() {
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange -= GameMultiplayer_onPlayerDataNetworkListChange;
  }

  private void CharacterSelectReady_onReadyChanged(object sender, System.EventArgs e) {
    UpdatePlayer();
  }

  private void GameMultiplayer_onPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdatePlayer();
  }

  private void UpdatePlayer() {
    if (GameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)) {
      Show();

      PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

      playerNameText.text = playerData.playerName.ToString();

      playerVisual.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    } else {
      Hide();
    }
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}