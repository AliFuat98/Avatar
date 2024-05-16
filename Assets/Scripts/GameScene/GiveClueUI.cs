using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiveClueUI : MonoBehaviour {
  [SerializeField] private Button closeButton;
  [SerializeField] private Button sendButton;
  [SerializeField] private TMP_InputField clueInput;
  [SerializeField] private TMP_InputField clueCountInput;

  private void Awake() {
    closeButton.onClick.AddListener(() => {
      gameObject.SetActive(false);
    });

    sendButton.onClick.AddListener(() => {
      SendClue();
    });
  }

  private void OnEnable() {
    clueInput.text = string.Empty;
    clueCountInput.text = string.Empty;
  }

  private void SendClue() {
    int clueCount;

    if (!int.TryParse(clueCountInput.text, out clueCount)) {
      return;
    }

    if (clueInput.text == string.Empty) {
      return;
    }

    ChatManager.Instance.AddNewClue($"{clueInput.text} {clueCount}", TurnManager.Instance.IsFirstTeamTurn);
    TurnManager.Instance.EndTurn();
    gameObject.SetActive(false);
  }
}