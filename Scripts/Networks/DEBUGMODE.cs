using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using static LobbyManager;

public class DEBUGMODE : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject _playerCharacter;
    void Start()
    {
        StartLocalHost();
        HUD.instance.SetPlayerObject(_playerCharacter);

        GameObject go = GameObject.Find("SettingManager");
        if (go == null)
        {
            go = new GameObject { name = "SettingManager" };
            go.AddComponent<SettingManager>();
        }
    }

    public bool shouldChase = false;
    public GameObject enemy;

    // Update is called once per frame
    void Update()
    {
        if(shouldChase)
            enemy.GetComponent<PoliceCar>().StartChase();
    }

    private void StartLocalHost()
    {
        // UnityTransport 설정
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData("127.0.0.1", 7777); // 로컬 IP와 포트 설정

        // 호스트 시작
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started on 127.0.0.1:7777");
        }
        else
        {
            Debug.LogError("Failed to start Host.");
        }
    }

}
