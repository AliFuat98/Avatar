using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamSelect : NetworkBehaviour {
  public static TeamSelect Instance { get; private set; }

  private void Awake() {
    Instance = this;
  }

  public override void OnNetworkSpawn() {
  }

  public void ChangeTeam(bool leftSite) {
    var id = NetworkManager.Singleton.LocalClientId;
    var (teamId, isTeller) = (0, false);
    if (leftSite) {
      // left Site

      if (teamId == 0) {
        // already in left site
        return;
      } else {
        // change team
        ChangeTeamServerRpc(teamId - 1);
        return;
      }
    } else {
      // right Site

      if (teamId == 1) {
        // already in right site
      } else {
        ChangeTeamServerRpc(teamId + 1);
      }
    }
  }

  [ServerRpc(RequireOwnership = false)]
  private void ChangeTeamServerRpc(int teamID, ServerRpcParams serverRpcParams = default) {
    ChangeTeamClientRpc(serverRpcParams.Receive.SenderClientId, teamID);
  }

  [ClientRpc]
  private void ChangeTeamClientRpc(ulong clintId, int teamID) {
  }
}