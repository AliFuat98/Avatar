using System;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour {
  public static TurnManager Instance { get; private set; }

  public event EventHandler OnTurnChanged;

  public bool isFirstTeamTurn { get; private set; } = true;
  public bool isTellerTurn { get; private set; } = true;

  private void Awake() {
    Instance = this;
  }

  public void EndTurn() {
    // First team Teller ==> First team Finder
    if (isFirstTeamTurn && isTellerTurn) {
      isTellerTurn = !isTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }

    // First team Finder ==> Second Team Teller
    if (isFirstTeamTurn && !isTellerTurn) {
      isFirstTeamTurn = !isFirstTeamTurn;
      isTellerTurn = !isTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }

    // Second Team Teller ==> Second Team Finder
    if (!isFirstTeamTurn && isTellerTurn) {
      isTellerTurn = !isTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }

    // Second Team Finder ==> First team Teller
    if (!isFirstTeamTurn && !isTellerTurn) {
      isFirstTeamTurn = !isFirstTeamTurn;
      isTellerTurn = !isTellerTurn;
      OnTurnChanged?.Invoke(this, EventArgs.Empty);
      return;
    }
  }
}