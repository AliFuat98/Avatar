using System;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour {
  public static TurnManager Instance { get; private set; }

  public event EventHandler OnTurnChanged;

  private bool isFirstTeamTurn = true;
  private bool isTellerTurn = true;

  private void Awake() {
    Instance = this;
  }

  public void EndTurn() {
    // First team Teller ==> First team Finder
    if (isFirstTeamTurn && isTellerTurn) {
      isTellerTurn = !isTellerTurn;
    }

    // First team Finder ==> Second Team Teller
    if (isFirstTeamTurn && !isTellerTurn) {
      isFirstTeamTurn = !isFirstTeamTurn;
      isTellerTurn = !isTellerTurn;
    }

    // Second Team Teller ==> Second Team Finder
    if (!isFirstTeamTurn && isTellerTurn) {
      isTellerTurn = !isTellerTurn;
    }

    // Second Team Finder ==> First team Teller
    if (!isFirstTeamTurn && !isTellerTurn) {
      isFirstTeamTurn = !isFirstTeamTurn;
      isTellerTurn = !isTellerTurn;
    }

    OnTurnChanged?.Invoke(this, EventArgs.Empty);
  }
}