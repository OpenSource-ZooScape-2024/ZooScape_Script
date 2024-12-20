using Firebase.Database;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class RelayManager : MonoBehaviour
{
    public static RelayManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //NetworkManager.Singleton.OnClientDisconnectCallback += OnServerDisconnect;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async Task<String> CreateRelay(int maxPlayer)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Created Relay: " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
            return null;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Join Relay with" + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
        }
    }
    public void DisconnectRelay()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Stopping Host and Disconnecting Relay...");
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Stopping Client and Disconnecting Relay...");
            NetworkManager.Singleton.Shutdown();
        }
        else
        {
            Debug.LogWarning("Relay is not currently connected.");
        }
    }
}
