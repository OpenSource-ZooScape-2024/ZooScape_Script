using Firebase.Database;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Firebase.Extensions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine.SceneManagement;

[Serializable]
public class RankData
{
    public string name;
    public int score;
    public RankData() { }
    public RankData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;
    private Firebase.Database.DatabaseReference _reference;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    AppOptions options = new AppOptions { DatabaseUrl = new Uri("https://zooscape-7eebf-default-rtdb.firebaseio.com/") };
                    FirebaseApp app = FirebaseApp.Create(options);
                    _reference = FirebaseDatabase.DefaultInstance.GetReference("rank");
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private Lobby _hostLobby = null;
    private Lobby _joinedLobby = null;
    private float _heartbeatTimer;
    private float _lobbyUpdateTimer;
    public string _playerName = "NULL";
    public string _characterName = "Panda";
    public string _isReady = "false";

    public event EventHandler OnLeftLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;

    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
        public List<RankData> rankList;
    }

    public enum PlayerCharacter
    {
        Tiger,
        Tapir,
        Sloth,
        Gorilla,
        Bat,
        Peacock,
        Cobra,
        Panda,
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    private async void HandleLobbyHeartbeat()
    {//호스트일 경우 로비 정보 지속적으로 전송 (30초 이상 전송 X시 로비 제거)
        if (IsLobbyHost())
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer < 0f)
            {
                _heartbeatTimer = 15;

                await LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyPollForUpdates()
    {//현재 로비에 대한 정보 업데이트 요청
        if (_joinedLobby != null)
        {
            _lobbyUpdateTimer -= Time.deltaTime;
            if (_lobbyUpdateTimer < 0f)
            {
                try
                {
                    _lobbyUpdateTimer = 1.1f;
                    _joinedLobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
                    if (!IsPlayerInLobby())
                    {
                        Debug.Log("Kicked from Lobby!");
                        OnKickedFromLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                        _joinedLobby = null;
                        return;
                    }
                    OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
                    if (_joinedLobby.Data["RelayJoinCode"].Value != "0" && !IsLobbyHost())
                    {
                        LoadingPanelUI.instance.gameObject.SetActive(true);
                        RelayManager.instance.JoinRelay(_joinedLobby.Data["RelayJoinCode"].Value);
                        _joinedLobby = null;
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogWarning(e);
                }
            }
        }
    }

    //로비 생성 함수
    public async void CreateLobby(string lobbyName, bool isPrivate, int maxPlayers)
    {
        try
        {
            _isReady = "True";
            //로비 데이터 설정
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, "0")},
                }
            };
            //로비 생성 요청
            _hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            _joinedLobby = _hostLobby;
            //로비 참가 이벤트 발생
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public async void RefreshLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            List<RankData> rankList = await GetSortedRankData();
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = queryResponse.Results , rankList = rankList});
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    //로비 참가 함수
    public async void JoinLobby(string lobbyId)
    {
        try
        {
            //로비 참가 데이터 설정
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer(),
            };
            //로비 참가 요청
            _joinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            Debug.Log("Joined Lobby with code " + lobbyId);

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer(),
            };
            _joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            Debug.Log("Joined Lobby with code " + lobbyCode);
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            _joinedLobby = null;
            _hostLobby = null;
            OnLeftLobby?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    public async void UpdatePlayerCharacter(bool isLef)
    {
        if (_joinedLobby != null)
        {
            try
            {
                int ind = (int)System.Enum.Parse<PlayerCharacter>(_characterName);
                if (isLef)
                {
                    ind = (ind + 7) % 8;
                }
                else
                {
                    ind = (ind + 1) % 8;
                }
                _characterName = ((PlayerCharacter)ind).ToString();

                UpdatePlayerOptions options = new UpdatePlayerOptions();
                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "PlayerCharacter", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: _characterName)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }
    }
    public async void UpdatePlayerReady(bool isReady)
    {
        if (_joinedLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                _isReady = isReady.ToString();
                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        "PlayerIsReady", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: _isReady)
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, playerId, options);
                _joinedLobby = lobby;

                OnJoinedLobbyUpdate?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    public async void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                Debug.Log("Start Game");
                LoadingPanelUI.instance.gameObject.SetActive(true);
                string relayCode = await RelayManager.instance.CreateRelay(_joinedLobby.Players.Count);

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                    }
                });
                await WaitForAllPlayersToJoinRelay();
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);

                _joinedLobby = null; _hostLobby = null;
                //NetworkManager.Singleton.SceneManager.LoadScene("_IN_GAME_", LoadSceneMode.Single);
                NetworkManager.Singleton.SceneManager.LoadScene("_LOADING_", LoadSceneMode.Single);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);
            }
        }
    }

    private async Task WaitForAllPlayersToJoinRelay()
    {
        bool allPlayersJoined = false;

        while (!allPlayersJoined)
        {
            await Task.Delay(1100);
            int connectedPlayers = NetworkManager.Singleton.ConnectedClients.Count;

            Debug.Log("Called" + connectedPlayers +  " " + _joinedLobby.Players.Count );
            if (_joinedLobby.Players.Count == connectedPlayers)
                allPlayersJoined = true;
        }
    }

    public async Task<List<RankData>> GetSortedRankData()
    {
        List<RankData> rankList = new List<RankData>();
        try
        {
            var query = _reference.OrderByChild("score").LimitToLast(10);
            var snapshot = await query.GetValueAsync();
            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
                string name = childSnapshot.Child("name").Value.ToString();
                int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                RankData rankData = new RankData(name, score);
                rankList.Add(rankData);
            }

            rankList.Sort((a, b) => b.score.CompareTo(a.score));
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to get rank data: " + ex.Message);
            return null;
        }
        return rankList;
    }

    public async Task AddRankData(string playerName, int playerScore)
    {
        try
        {
            // 랭크 데이터를 Firebase에 추가
            var newRankRef = _reference.Push(); // 새로운 랭크 엔트리 생성
            var rankData = new RankData(playerName, playerScore);

            // 데이터를 명시적으로 JSON으로 변환
            string jsonData = JsonUtility.ToJson(rankData);
            await newRankRef.SetRawJsonValueAsync(jsonData);

            Debug.Log($"Rank data added for {playerName} with score {playerScore}");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to add rank data: " + ex.Message);
            Debug.Log($"Adding Rank Data: Name={playerName}, Score={playerScore}");
        }
    }

    private bool IsPlayerInLobby()
    {
        if (_joinedLobby != null && _joinedLobby.Players != null)
        {
            foreach (Player player in _joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
               {
                   {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)},
                   {"PlayerIsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _isReady)},
                   {"PlayerCharacter", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, _characterName)},
               }
        };
    }

    public Lobby GetJoinedLobby()
    {
        return _joinedLobby;
    }
    public bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    private void PrintPlayers(Lobby lobby)
    {
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + player.Data["PlayerName"].Value);
        }
    }

    public bool IsPlayerHost(Player player)
    {
        if (_joinedLobby.HostId == player.Id)
            return true;
        return false;
    }
}
