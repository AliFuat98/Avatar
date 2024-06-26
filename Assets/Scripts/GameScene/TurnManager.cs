using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour {
  public static TurnManager Instance { get; private set; }

  public event EventHandler OnTurnChanged;

  public event EventHandler OnRemaningCardCountChanged;

  [SerializeField] private TextMeshProUGUI turnText;

  public bool IsFirstTeamTurn { get; private set; } = true;
  public bool IsTellerTurn { get; private set; } = true;
  private int[] teamIdRemaningCardCount { get; set; }
  public bool isGameOver { get; private set; } = false;

  private void Awake() {
    Instance = this;
    teamIdRemaningCardCount = new int[3] {
      8,0,7,
    };

    turnText.text = "Red Team Teller";
  }

  public void DecreaseCardCount(int teamId) {
    teamIdRemaningCardCount[teamId]--;
 
    if (teamIdRemaningCardCount[teamId] <= 0) {
      MessageManager.Instance.SetTextWithoutTime(IsFirstTeamTurn ? "red team won" : "blue team won");
      isGameOver = true;
    }

    OnRemaningCardCountChanged?.Invoke(this, EventArgs.Empty);
  }

  public void BlackCard() {
    isGameOver = true;
    OnRemaningCardCountChanged?.Invoke(this, EventArgs.Empty);
    MessageManager.Instance.SetTextWithoutTime(IsFirstTeamTurn ? "blue team won" : "red team won");
  }

  public int GetRemainingCardCount(int teamId) {
    return teamIdRemaningCardCount[teamId];
  }

  public void EndTurn() {
    EndTurnServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  public void EndTurnServerRpc() {
    EndTurnClientRpc();
  }

  [ClientRpc]
  private void EndTurnClientRpc() {
    // First team Teller ==> First team Finder
    if (IsFirstTeamTurn && IsTellerTurn) {
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      turnText.text = "Red Team Finder";
      return;
    }

    // First team Finder ==> Second Team Teller
    if (IsFirstTeamTurn && !IsTellerTurn) {
      IsFirstTeamTurn = !IsFirstTeamTurn;
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      turnText.text = "Blue Team Teller";
      return;
    }

    // Second Team Teller ==> Second Team Finder
    if (!IsFirstTeamTurn && IsTellerTurn) {
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      turnText.text = "Blue Team Finder";
      return;
    }

    // Second Team Finder ==> First team Teller
    if (!IsFirstTeamTurn && !IsTellerTurn) {
      IsFirstTeamTurn = !IsFirstTeamTurn;
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      turnText.text = "Red Team Teller";
      return;
    }
  }

  public bool IsMyTurn() {
    var playerData = GameMultiplayer.Instance.GetPlayerData();

    return (IsFirstTeamTurn, playerData.teamId, IsTellerTurn, playerData.isTeller) switch {
      (true, 0, true, true) => true,
      (true, 0, false, false) => true,
      (false, 2, true, true) => true,
      (false, 2, false, false) => true,
      _ => false
    };
  }
}