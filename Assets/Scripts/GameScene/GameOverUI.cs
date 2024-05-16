using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
  [SerializeField] private Button mainMenuButton;

  private void Awake() {
    mainMenuButton.onClick.AddListener(() => {
      NetworkManager.Singleton.Shutdown();

      Loader.Load(Loader.Scene.MainMenuScene);
    });
  }

  private void Start() {
    TurnManager.Instance.OnRemaningCardCountChanged += TurnManager_OnRemaningCardCountChanged;
    gameObject.SetActive(false);
  }

  private void TurnManager_OnRemaningCardCountChanged(object sender, System.EventArgs e) {
    if (TurnManager.Instance.isGameOver) {
      gameObject.SetActive(true);
    }
  }
}