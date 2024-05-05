using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour {
  [SerializeField] private Button playAgainButton;

  private void Start() {
    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    Hide();

    playAgainButton.onClick.AddListener(() => {
      NetworkManager.Singleton.Shutdown();
      Loader.Load(Loader.Scene.MainMenuScene);
    });
  }

  private void OnDestroy() {
    NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
  }

  private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
    if (clientId == NetworkManager.ServerClientId) {
      // server is shutting down

      Show();
    }
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}