using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour {
  public static ChatManager Instance { get; private set; }

  [SerializeField] private GameObject giveClueUI;
  [SerializeField] private TextMeshProUGUI firstTeamChat;
  [SerializeField] private TextMeshProUGUI secondTeamChat;
  [SerializeField] private Button giveClueButton;
  [SerializeField] private Button endTurnButton;

  private int firstTeamclueCount = 0;
  private int secondTeamclueCount = 0;

  private void Awake() {
    Instance = this;

    giveClueButton.onClick.AddListener(() => {
      var playerData = GameMultiplayer.Instance.GetPlayerData();
      if (!playerData.isTeller) {
        MessageManager.Instance.SetText("you are not a teller");
        return;
      }

      if (!TurnManager.Instance.IsMyTurn()) {
        MessageManager.Instance.SetText("Not your turn");
        return;
      }

      giveClueUI.SetActive(true);
    });

    endTurnButton.onClick.AddListener(() => {
      if (!TurnManager.Instance.IsMyTurn()) {
        MessageManager.Instance.SetText("Not your turn");
        return;
      }

      TurnManager.Instance.EndTurn();
    });
  }

  private void Start() {
    firstTeamChat.text = string.Empty;
    secondTeamChat.text = string.Empty;
  }

  public void AddNewClue(string clue, bool firstTeamTurn) {
    AddNewClueServerRpc(clue, firstTeamTurn);
  }

  [ServerRpc(RequireOwnership = false)]
  private void AddNewClueServerRpc(string clue, bool firstTeamTurn, ServerRpcParams serverRpcParams = default) {
    AddNewClueClientRpc(clue, firstTeamTurn);
  }

  [ClientRpc]
  private void AddNewClueClientRpc(string clue, bool firstTeamTurn) {
    if (firstTeamTurn) {
      firstTeamclueCount++;
      firstTeamChat.text += $"\n{firstTeamclueCount}-{clue}";
    } else {
      secondTeamclueCount++;
      secondTeamChat.text += $"\n{secondTeamclueCount}-{clue}";
    }
  }
}