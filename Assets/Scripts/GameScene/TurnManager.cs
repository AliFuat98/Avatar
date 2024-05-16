using System;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour {
  public static TurnManager Instance { get; private set; }

  public event EventHandler OnTurnChanged;

  public event EventHandler OnRemaningCardCountChanged;

  public bool IsFirstTeamTurn { get; private set; } = true;
  public bool IsTellerTurn { get; private set; } = true;
  private int[] teamIdRemaningCardCount { get; set; }

  private void Awake() {
    Instance = this;
    teamIdRemaningCardCount = new int[3] {
      8,0,7,
    };
  }

  public void DecreaseCardCount() {
    var index = IsFirstTeamTurn ? 0 : 2;
    teamIdRemaningCardCount[index]--;
    OnRemaningCardCountChanged?.Invoke(this, EventArgs.Empty);
  }

  public int GetRemainingCardCount(int teamId) {
    return teamIdRemaningCardCount[teamId];
  }

  public void EndTurn() {
    // First team Teller ==> First team Finder
    if (IsFirstTeamTurn && IsTellerTurn) {
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }

    // First team Finder ==> Second Team Teller
    if (IsFirstTeamTurn && !IsTellerTurn) {
      IsFirstTeamTurn = !IsFirstTeamTurn;
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }

    // Second Team Teller ==> Second Team Finder
    if (!IsFirstTeamTurn && IsTellerTurn) {
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }

    // Second Team Finder ==> First team Teller
    if (!IsFirstTeamTurn && !IsTellerTurn) {
      IsFirstTeamTurn = !IsFirstTeamTurn;
      IsTellerTurn = !IsTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }
  }

  public bool IsMyTurn() {
    //var playerData = GameMultiplayer.Instance.GetPlayerData();

    //return (IsFirstTeamTurn, playerData.teamId, IsTellerTurn, playerData.isTeller) switch {
    return (IsFirstTeamTurn, 0, IsTellerTurn, true) switch {
      (true, 0, true, true) => true,
      (true, 0, false, false) => true,
      (false, 2, true, true) => true,
      (false, 2, false, false) => true,
      _ => false
    };
  }
}