using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour {
  private const string KEY_RELAY_JOIN_CODE = "relayJoinCode";
  public static GameLobby Instance { get; private set; }

  public event EventHandler OnCreateLobbyStarted;

  public event EventHandler OnCreateLobbyFailedStarted;

  public event EventHandler OnJoinStarted;

  public event EventHandler OnJoinFailed;

  public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

  public class OnLobbyListChangedEventArgs : EventArgs {
    public List<Lobby> lobbyList;
  }

  private Lobby joinedLobby;

  private float heartBeatTimer;
  private readonly float heartBeatTimerMax = 15f;

  private float listLobbyTimer;
  private readonly float listLobbyTimerMax = 3f;

  private void Awake() {
    Instance = this;

    DontDestroyOnLoad(gameObject);
    InitializeUnityAuthentication();
  }

  public async void InitializeUnityAuthentication() {
    if (UnityServices.State != ServicesInitializationState.Initialized) {
      // önceden baþlatýlmadýysa 2 kez baþlatmamak için

      // ayný pc de build alýnca id deðiþmesi için options geçiyoruz
      InitializationOptions options = new();
      options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

      // servisi baþlat
      await UnityServices.InitializeAsync(options);

      // giriþ yap
      await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
  }

  private void Update() {
    HandleHeartBeat();
    HandlePeriodicListLobbies();
  }

  #region Refresh Lobby

  private void HandleHeartBeat() {
    if (!IsLobbyHost()) {
      return;
    }

    heartBeatTimer -= Time.deltaTime;
    if (heartBeatTimer < 0) {
      heartBeatTimer = heartBeatTimerMax;

      LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
    }
  }

  private void HandlePeriodicListLobbies() {
    if (joinedLobby != null ||
      UnityServices.State != ServicesInitializationState.Initialized ||
      !AuthenticationService.Instance.IsSignedIn ||
      SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()) {
      return;
    }

    listLobbyTimer -= Time.deltaTime;
    if (listLobbyTimer < 0f) {
      listLobbyTimer = listLobbyTimerMax;
      ListLobbies();
    }
  }

  private async void ListLobbies() {
    try {
      QueryLobbiesOptions options = new() {
        Count = 25,

        // Filter for open lobbies only
        Filters = new List<QueryFilter> {
          new (QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
        },

        // Order by newest lobbies first
        Order = new List<QueryOrder> {
          new(asc: false, field: QueryOrder.FieldOptions.Created)
        }
      };

      QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(options);

      OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs {
        lobbyList = queryResponse.Results
      });
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  #endregion Refresh Lobby

  #region Create Lobby

  public async void CreateLobby(string lobbyName, bool isPrivate) {
    OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayer.MAX_PLAYER_AMOUNT,
        new CreateLobbyOptions { IsPrivate = isPrivate }
      );

      Allocation allocation = await AllocateRelay();

      string relayJoincode = await GetRelayJoinCode(allocation);

      await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
        Data = new Dictionary<string, DataObject> {
          {KEY_RELAY_JOIN_CODE ,new DataObject(DataObject.VisibilityOptions.Member, relayJoincode) }
        }
      });

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

      GameMultiplayer.Instance.StartHost();
      Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnCreateLobbyFailedStarted?.Invoke(this, EventArgs.Empty);
    }
  }

  private async Task<Allocation> AllocateRelay() {
    try {
      Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameMultiplayer.MAX_PLAYER_AMOUNT - 1);

      return allocation;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return default;
    }
  }

  private async Task<string> GetRelayJoinCode(Allocation allocation) {
    try {
      string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

      return relayJoinCode;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return default;
    }
  }

  #endregion Create Lobby

  #region Join Lobby

  public async void QuickJoin() {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

      string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
      JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

      GameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
    }
  }

  public async void JoinWithCode(string lobbyCode) {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

      string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
      JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

      GameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
    }
  }

  public async void JoinWithId(string lobbyId) {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try {
      joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

      string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
      JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

      NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

      GameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e) {
      Debug.Log(e);
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
    }
  }

  public async Task<JoinAllocation> JoinRelay(string joinCode) {
    try {
      JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

      return joinAllocation;
    } catch (RelayServiceException e) {
      Debug.Log(e);
      return default;
    }
  }

  #endregion Join Lobby

  #region Delete Lobby Player

  public async void DeleteLobby() {
    try {
      if (joinedLobby != null) {
        await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

        joinedLobby = null;
      }
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  public async void LeaveLobby() {
    try {
      if (joinedLobby != null) {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

        joinedLobby = null;
      }
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  public async void KickPlayer(string playerID) {
    try {
      if (IsLobbyHost()) {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerID);
      }
    } catch (LobbyServiceException e) {
      Debug.Log(e);
    }
  }

  #endregion Delete Lobby Player

  #region Helper

  public Lobby GetJoinedLobby() {
    return joinedLobby;
  }

  private bool IsLobbyHost() {
    return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
  }

  #endregion Helper
}