using System;
using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour {
  public static CharacterSelectReady Instance { get; private set; }

  public event EventHandler OnReadyChanged;

  private Dictionary<ulong, bool> playerReadyDictionary;

  private void Awake() {
    Instance = this;

    playerReadyDictionary = new();
  }

  public void SetPlayerReady() {
    SetPlayerReadyServerRpc();
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
    SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

    bool allClientsAreReady = true;
    foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
      if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
        // this player is not ready
        allClientsAreReady = false;
      }
    }

    if (allClientsAreReady) {
      GameLobby.Instance.DeleteLobby();
      Loader.LoadNetwork(Loader.Scene.GameScene);
    }
  }

  [ClientRpc]
  private void SetPlayerReadyClientRpc(ulong clintId) {
    playerReadyDictionary[clintId] = true;
    OnReadyChanged?.Invoke(this, EventArgs.Empty);
  }

  public bool IsPlayerReady(ulong clientID) {
    return playerReadyDictionary.ContainsKey(clientID) && playerReadyDictionary[clientID];
  }
}