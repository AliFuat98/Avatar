using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectedSingleUI : MonoBehaviour {
  [SerializeField] private int colorId;
  [SerializeField] private Image image;
  [SerializeField] private GameObject selectedGameObject;

  private void Awake() {
    GetComponent<Button>().onClick.AddListener(() => {
      GameMultiplayer.Instance.ChangePlayerColor(colorId);
    });
  }

  private void Start() {
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_onPlayerDataNetworkListChange;
    image.color = GameMultiplayer.Instance.GetPlayerColor(colorId);
    UpdateIsSelected();
  }

  private void OnDestroy() {
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange -= GameMultiplayer_onPlayerDataNetworkListChange;
  }

  private void GameMultiplayer_onPlayerDataNetworkListChange(object sender, System.EventArgs e) {
    UpdateIsSelected();
  }

  private void UpdateIsSelected() {
    if (GameMultiplayer.Instance.GetPlayerData().colorId == colorId) {
      selectedGameObject.SetActive(true);
    } else {
      selectedGameObject.SetActive(false);
    }
  }
}