using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : NetworkBehaviour
{
    public static LoadingSceneManager instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject loadingMap;

    private void Start()
    {
        if(IsHost)
        {
            Debug.Log("호출시도");
            NetworkManager.Singleton.SceneManager.LoadScene("_IN_GAME_", LoadSceneMode.Additive);
        }
    }

    private void Update()
    {
        Vector3 curPos = loadingMap.transform.position;
        curPos.z += 10 * Time.deltaTime;
        loadingMap.transform.position = curPos;
    }
}
