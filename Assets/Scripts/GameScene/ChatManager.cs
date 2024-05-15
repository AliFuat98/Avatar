using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour {
  public static ChatManager Instance { get; private set; }

  [SerializeField] private GameObject giveClueUI;
  [SerializeField] private TextMeshProUGUI firstTeamChat;
  [SerializeField] private TextMeshProUGUI secondTeamChat;
  private int firstTeamclueCount = 0;
  private int secondTeamclueCount = 0;

  private void Awake() {
    Instance = this;
  }

  private void Start() {
    firstTeamChat.text = string.Empty;
    secondTeamChat.text = string.Empty;
  }

  public void OpenGiveClueUI() {
    giveClueUI.SetActive(true);
  }

  public void AddNewClue(string clue) {
    var isFirstTeamTurn = TurnManager.Instance.isFirstTeamTurn;

    if (isFirstTeamTurn) {
      firstTeamclueCount++;
      firstTeamChat.text += $"\n{firstTeamclueCount}-{clue}";
    } else {
      secondTeamclueCount++;
      secondTeamChat.text += $"\n{secondTeamclueCount}-{clue}";
    }
  }
}