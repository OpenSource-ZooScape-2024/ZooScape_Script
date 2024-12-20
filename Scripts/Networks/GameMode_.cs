using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using System;


public class GameMode_ : NetworkBehaviour
{
    public static GameMode_ instance;

    private int _connectedClients = 0;
    private int _deadPlayerCount = 0;

    [SerializeField] private List<Transform> _playerSpawnPoint;
    [SerializeField] private GameObject[] _playerPrefabs;
    [SerializeField] public GameObject _playerCharacter;
    [SerializeField] public List<GameObject> _players;
    [SerializeField] public GameObject _boss;
    [SerializeField] public Transform startPoint;
    [SerializeField] public Transform endPoint;
    [SerializeField] public GameEndUI _gameEndUI;
    [SerializeField] public ObstacleSpawner _obstacleManager;
    [SerializeField] public ItemSpawner _itemSpawner;

    private void Awake()
    {
        instance = this;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    // 서버 또는 로컬 클라이언트에게만 이벤트 발생
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} disconnected");
        if (IsHost)
        {
            _connectedClients--;
            ClientDisconectHandleClientRPC(clientId);
        }
        else
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
    }

    [ClientRpc]
    private void ClientDisconectHandleClientRPC(ulong clientId)
    {
        _players.RemoveAll(p => p.GetComponent<NetworkObject>().OwnerClientId == clientId);
    }

    // Start is called before the first frame update
    void Start()
    {
        OnSceneLoaded();
    }

    void OnSceneLoaded()
    {
        if (IsClient)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                AddPlayer(player);
            }

            RequestSpawnCharacterServerRpc(LobbyManager.instance._characterName);

            //호스트는 매니저 업데이트
            if(IsHost)
            {
                try
                {
                    Managers.Init();
                    _obstacleManager.gameObject.SetActive(true);
                    _itemSpawner.gameObject.SetActive(true);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                Light existingLight = FindObjectOfType<Light>();
                if (existingLight != null && existingLight.type == LightType.Directional)
                {
                    Destroy(existingLight);
                }
            }
        }
    }

    #region Init
    [ServerRpc(RequireOwnership = false)]
    //클라이언트의 씬 로드 시 서버에게 캐릭터 스폰을 요청하는 함수
    void RequestSpawnCharacterServerRpc(string playerCharacter, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId; // 요청을 보낸 클라이언트 ID
        Debug.Log($"Server: SpawnCharacter for Client {clientId}, Character: {playerCharacter}");
        SpawnCharacter(clientId, playerCharacter);
    }

    private int spawnPos = 0;
    //캐릭터 오브젝트 스폰 처리 함수
    void SpawnCharacter(ulong clientId, string playerCharacter)
    {
        Debug.Log($"Spawning character '{playerCharacter}' for client {clientId}");
        // NetworkObject 생성
        GameObject prefab = System.Array.Find(_playerPrefabs, p => p.name == playerCharacter);
        GameObject character = Instantiate(prefab, _playerSpawnPoint[spawnPos++].position, Quaternion.identity);
        if (character == null)
        {
            Debug.LogError($"Failed to instantiate character prefab '{playerCharacter}'");
            return;
        }

        //클라이언트에 소유권 할당
        NetworkObject networkObject = character.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError("NetworkObject component not found on instantiated character.");
            return;
        }
        networkObject.SpawnWithOwnership(clientId);
        SetOwnPlayerCharacterClientRpc(networkObject.NetworkObjectId);
    }

    [ClientRpc]
    //생성된 플레이어 오브젝트를 통해 각종 초기화 수행
    void SetOwnPlayerCharacterClientRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            if (networkObject != null && networkObject.IsOwner)
            {
                _playerCharacter = networkObject.gameObject;
                HUD.instance.SetPlayerObject(_playerCharacter);
                //여기서 로딩씬 변수 업데이트
                StartCoroutine(LoadGame());
            }
        }
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(3f);
        SetPlayerSpawnCountServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerSpawnCountServerRPC()
    {
        _connectedClients++;
        // 모든 클라이언트가 연결되었는지 확인
        if (_connectedClients == NetworkManager.Singleton.ConnectedClients.Count)
        {
            Debug.Log("All clients connected. Starting the game...");
            NotifyClientsStartCountdownClientRpc();
            // 네트워크 씬 언로드
            Scene loadingScene = SceneManager.GetSceneByName("_LOADING_");
            NetworkManager.Singleton.SceneManager.UnloadScene(loadingScene);  // Scene 객체를 전달
        }
    }
    #endregion

    #region GameStart
    [ClientRpc]
    private void NotifyClientsStartCountdownClientRpc()
    {
        HUD.instance.InitializeDistanceHUD();
        StartCoroutine(CountDownCoroutine());
    }

    private IEnumerator CountDownCoroutine()
    {
        HUD.instance.UpdateCountDown("3");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("2");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("1");
        yield return new WaitForSeconds(1f);

        // 카운트다운이 끝났을 때 게임 시작
        if (IsHost)
            StartGame();

        HUD.instance.UpdateCountDown("Start");
        yield return new WaitForSeconds(0.5f);
        HUD.instance.UpdateCountDown("");
    }
    public void StartGame()
    {
        // 게임 시작 로직 실행
        Debug.Log("Game is starting!");
        NotifyClientsGameStartedClientRpc();
        _boss.GetComponent<PoliceCar>().StartChase();
    }

    [ClientRpc]
    private void NotifyClientsGameStartedClientRpc()
    {
        //개인 캐릭터 인풋 활성화
        foreach(var player in _players)
        {
            player.GetComponent<PlayerController>()._isOnGame = true;
        }
    }
    #endregion

    #region GameEnd

    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerDeadServerRPC()
    {
        Debug.Log(_deadPlayerCount + _connectedClients);
        _deadPlayerCount++;
        if (_deadPlayerCount == _connectedClients)
        {
            HandleEndGameRoutineClientRPC(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void HandleEndGameRoutineServerRPC()
    {
        _obstacleManager.gameObject.SetActive(false);
        _itemSpawner.gameObject.SetActive(false);
        
        HandleEndGameRoutineClientRPC(true);
    }

    public int _finishedCount = 0;
    [ClientRpc]
    private void HandleEndGameRoutineClientRPC(bool isEnded)
    {
        if (isEnded)
        {
            if (_finishedCount++ == 0)
            {
                StartCoroutine(GameEndCountRoutine());
            }
        }
        else
            GameEnd();
    }

    private IEnumerator GameEndCountRoutine()
    {
        HUD.instance.UpdateCountDown("5");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("4");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("3");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("2");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("1");
        yield return new WaitForSeconds(1f);

        HUD.instance.UpdateCountDown("Game Over");
        yield return new WaitForSeconds(1f);
        HUD.instance.UpdateCountDown("");
        GameEnd();
    }

    private void GameEnd()
    {
        _boss.gameObject.SetActive(false);
        _gameEndUI.gameObject.SetActive(true);
        _gameEndUI.UpdateEndPanel(_players, _playerCharacter);
    }
    #endregion

    public void AddPlayer(GameObject player)
    {
        _players.Add(player);
    }

    public List<GameObject> GetPlayers()
    {
        return _players;
    }
}
