using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMultiplayer : NetworkBehaviour {
  public const int MAX_PLAYER_AMOUNT = 4;
  private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "playerNameMultiplayer";

  public static GameMultiplayer Instance { get; private set; }

  public event EventHandler OnTryingToJoinGame;

  public event EventHandler OnFailedToJoinGame;

  public event EventHandler OnPlayerDataNetworkListChange;

  [SerializeField] private List<Color> playerColorList = new();
  private NetworkList<PlayerData> playerDataNetworkList;
  private string playerName;

  private void Awake() {
    Instance = this;

    DontDestroyOnLoad(gameObject);

    playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, $"PlayerName{UnityEngine.Random.Range(100, 1000)}");

    playerDataNetworkList = new NetworkList<PlayerData>();
    playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
  }

  private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent) {
    OnPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty);
  }

  #region Start Host

  public void StartHost() {
    NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;

    NetworkManager.Singleton.StartHost();
  }

  public override void OnNetworkDespawn() {
    NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
  }

  private void NetworkManager_ConnectionApprovalCallback(
     NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
     NetworkManager.ConnectionApprovalResponse connectionApprovalResponse
   ) {
    if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()) {
      connectionApprovalResponse.Approved = false;
      connectionApprovalResponse.Reason = "the game is already Starterd ";
      return;
    }

    if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT) {
      connectionApprovalResponse.Approved = false;
      connectionApprovalResponse.Reason = "the game is full ";
      return;
    }

    connectionApprovalResponse.Approved = true;
  }

  private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId) {
    playerDataNetworkList.Add(new() {
      clientId = clientId,
      colorId = GetFirstUnusedColorId(),
    });

    SetPlayerNameServerRpc(GetPlayerName());
    SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
  }

  private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      PlayerData playerData = playerDataNetworkList[i];
      if (playerData.clientId == clientId) {
        // disconnected

        playerDataNetworkList.RemoveAt(i);
      }
    }
  }

  #endregion Start Host

  #region Start Client

  public void StartClient() {
    OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;

    NetworkManager.Singleton.StartClient();
  }

  private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
    OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
  }

  private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
    SetPlayerNameServerRpc(GetPlayerName());
    SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.playerName = playerName;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.playerId = playerId;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  #endregion Start Client

  #region Player Data

  public PlayerData GetPlayerData() {
    return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
  }

  public PlayerData GetPlayerDataFromClientId(ulong clientId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId == clientId) {
        return playerData;
      }
    }
    return default;
  }

  public int GetPlayerDataIndexFromClientId(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return i;
      }
    }
    return -1;
  }

  public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) {
    return playerDataNetworkList[playerIndex];
  }

  #endregion Player Data

  #region Player Color

  public int GetPlayerColorIDFromClientId(ulong clientId) {
    for (int i = 0; i < playerDataNetworkList.Count; i++) {
      if (playerDataNetworkList[i].clientId == clientId) {
        return playerDataNetworkList[i].colorId;
      }
    }
    return -1;
  }

  public void ChangePlayerColor(int colorId) {
    ChangePlayerColorServerRpc(colorId);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default) {
    if (!IsColorAvailable(colorId)) {
      return;
    }

    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.colorId = colorId;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  private int GetFirstUnusedColorId() {
    for (int i = 0; playerColorList.Count > 0; i++) {
      if (IsColorAvailable(i)) {
        return i;
      }
    }
    return -1;
  }

  private bool IsColorAvailable(int colorId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.colorId == colorId) {
        // already in use
        return false;
      }
    }

    return true;
  }

  public Color GetPlayerColor(int colorId) {
    return playerColorList[colorId];
  }

  #endregion Player Color

  #region Player Name

  public string GetPlayerName() {
    return playerName;
  }

  public string GetPlayerName(ulong clientID) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.clientId == clientID) {
        return playerData.playerName.ToString();
      }
    }

    return "default name";
  }

  public void SetPlayerName(string playerName) {
    this.playerName = playerName;

    PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
  }

  #endregion Player Name

  public void KickPlayer(ulong clientId) {
    NetworkManager.Singleton.DisconnectClient(clientId);
    NetworkManager_Server_OnClientDisconnectCallback(clientId);
  }

  public bool IsPlayerIndexConnected(int playerIndex) {
    return playerIndex < playerDataNetworkList.Count;
  }
}