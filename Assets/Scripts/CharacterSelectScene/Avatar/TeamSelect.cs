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
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += test;

  }

  private void Start() {
    GameMultiplayer.Instance.OnPlayerDataNetworkListChange += GameMultiplayer_OnPlayerDataNetworkListChange;
  }

  private void GameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs e) {
    Debug.Log("TEAM SELECT list changes in ");
  }

  private void test(object sender, EventArgs e) {
    Debug.Log("TEAM SELECT list changes in test");
  }


  private void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
      foreach (var kvp in teamDic) {
        Console.WriteLine($"Player ID: {kvp.Key}, Room ID: {kvp.Value.teamId}, Is Teller: {kvp.Value.isTeller}");
      }
    }
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