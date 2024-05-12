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
    switch (changeEvent.Type) {
      case NetworkListEvent<PlayerData>.EventType.Add:
        Debug.Log("Player added to team list.");
        break;

      case NetworkListEvent<PlayerData>.EventType.Remove:
        Debug.Log("Player removed from team list.");
        break;

      case NetworkListEvent<PlayerData>.EventType.Value:
        Debug.Log("Player team data updated.");
        break;
        // Handle other cases as necessary
    }

    OnPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty);
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.P)) {
      foreach (var playerData in playerDataNetworkList) {
        PrintPlayerData(playerData);
      }
    }
  }

  private void PrintPlayerData(PlayerData playerData) {
    Debug.Log("------");
    Debug.Log($"clientId: {playerData.clientId}");
    Debug.Log($"playerName: {playerData.playerName}");
    Debug.Log($"isTeller: {playerData.isTeller}");
    Debug.Log($"teamId: {playerData.teamId}");
    Debug.Log($"playerId: {playerData.playerId}");
    Debug.Log($"colorId: {playerData.colorId}");
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
    var colorId = GetFirstUnusedColorId();
    playerDataNetworkList.Add(new() {
      clientId = clientId,
      colorId = colorId,
      teamId = 1,
      isTeller = false,
      playerName = GetPlayerName(),
      playerId = AuthenticationService.Instance.PlayerId,
    });
  }

  private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId) {
    try {
      for (int i = 0; i < playerDataNetworkList.Count; i++) {
        PlayerData playerData = playerDataNetworkList[i];
        if (playerData.clientId == clientId) {
          // disconnected

          playerDataNetworkList.RemoveAt(i);
        }
      }
    } catch (Exception e) {
      Debug.Log(e.Message);
    }
  }

  #endregion Start Host

  #region Start Client

  public void StartClient() {
    OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

    NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;

    NetworkManager.Singleton.StartClient();
  }

  private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId) {
    OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
  }

  private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId) {
    PlayerData playerData = new() {
      playerId = AuthenticationService.Instance.PlayerId,
      playerName = GetPlayerName()
    };

    SetPlayerDataServerRpc(playerData);
  }

  [ServerRpc(RequireOwnership = false)]
  private void SetPlayerDataServerRpc(PlayerData newPlayerData, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.playerName = newPlayerData.playerName;
    playerData.playerId = newPlayerData.playerId;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  #endregion Start Client

  #region Player Data

  public List<PlayerData> GetPlayerDataList() {
    var result = new List<PlayerData>();
    foreach (PlayerData playerData in playerDataNetworkList) {
      result.Add(playerData);
    }
    return result;
  }

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

  #region Player Team

  public void ChangePlayerTeam(int teamId) {
    var playerData = GetPlayerData();
    if (playerData.teamId == teamId && !playerData.isTeller) {
      // try to switch same team and it is not a teller
      return;
    }

    ChangePlayerTeamServerRpc(teamId);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ChangePlayerTeamServerRpc(int teamId, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.teamId = teamId;
    playerData.isTeller = false;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  // be a teller
  public void ChangeTeller(int teamId) {
    if (!IsTellerEmpty(teamId)) {
      return;
    }

    ChangeTellerServerRpc(teamId);
  }

  [ServerRpc(RequireOwnership = false)]
  public void ChangeTellerServerRpc(int teamId, ServerRpcParams serverRpcParams = default) {
    int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

    PlayerData playerData = playerDataNetworkList[playerDataIndex];
    playerData.teamId = teamId;
    playerData.isTeller = true;

    playerDataNetworkList[playerDataIndex] = playerData;
  }

  private bool IsTellerEmpty(int teamId) {
    foreach (PlayerData playerData in playerDataNetworkList) {
      if (playerData.teamId == teamId && playerData.isTeller) {
        // same team and there is a teller
        return false;
      }
    }

    return true;
  }

  #endregion Player Team

  public void KickPlayer(ulong clientId) {
    NetworkManager.Singleton.DisconnectClient(clientId);
    NetworkManager_Server_OnClientDisconnectCallback(clientId);
  }

  public bool IsPlayerIndexConnected(int playerIndex) {
    return playerIndex < playerDataNetworkList.Count;
  }
}