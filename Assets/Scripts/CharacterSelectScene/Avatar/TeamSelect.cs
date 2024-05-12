using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamSelect : NetworkBehaviour {
  public static TeamSelect Instance { get; private set; }

  public event EventHandler OnTeamChanged;

  Dictionary<ulong, (int teamId, bool isTeller)> teamDic;

  private void Awake() {
    Instance = this;

    teamDic = new();
  }

  public override void OnNetworkSpawn() {
    InitiateTeamDictionaryServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  private void InitiateTeamDictionaryServerRpc(ServerRpcParams serverRpcParams = default) {
    InitiateTeamDictionaryClientRpc(serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void InitiateTeamDictionaryClientRpc(ulong clientId) {
    teamDic.Add(clientId, (1, false));
    OnTeamChanged?.Invoke(this, EventArgs.Empty);
  }

  public void PrintTeamDictionary() {
    foreach (var kvp in teamDic) {
      Debug.Log($"client ID: {kvp.Key}, Room ID: {kvp.Value.teamId}, Is Teller: {kvp.Value.isTeller}");
    }
  }

  public Dictionary<ulong, (int teamId, bool isTeller)> GetTeamDic() {
    return teamDic;
  }

  public void ChangeTeam(bool leftSite) {
    var id = NetworkManager.Singleton.LocalClientId;
    var (teamId, isTeller) = teamDic[id];
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
    teamDic[clintId] = (teamID, false);
    OnTeamChanged?.Invoke(this, EventArgs.Empty);
  }
}