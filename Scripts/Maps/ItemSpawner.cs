using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    public GameObject coinPrefab; // Steak ������
    public GameObject[] itemPrefabs; // 아이템 프리팹 추가

    public int spawnStart = 12; // ������ z��ǥ �ּ� ��ġ
    public int spawnEnd = 1690; // z��ǥ �ִ� ��ġ
    // x��ǥ, z��ǥ�� �Ҵ��� ���
    private float[] xPositions = { -9f, -4.5f, 0f, 4.5f, 9f };
    private float[] zPositions;

    public GameObject coinParent;
    public GameObject itemParent;


    void Start()
    {
        // z��ǥ ���ϴ� �ǵ� 1010 �κ��� �ʿ� ������ ������ũ ���� �ϸ� ��
        int count = (spawnEnd - spawnStart) / 6 + 1;
        zPositions = new float[count];
        for (int i = 0; i < count; i++)
        {
            zPositions[i] = spawnStart + i * 6; // 6�������� ����
        }

        SpawnAllCoins();
        SpawnAllItems();
    }

    void SpawnAllCoins()
    {
        foreach (float z in zPositions)
        {
            float randomX = xPositions[Random.Range(0, xPositions.Length)];
            Vector3 spawnPosition = new Vector3(randomX, 0.5f, z);
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            NetworkObject networkObject = coin.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            if(coinParent != null)
                coin.transform.SetParent(coinParent.transform);
        }
    }

    void SpawnAllItems()
    {
        if (itemPrefabs.Length == 0)
        {
            return;
        }

        foreach (float z in zPositions)
        {
            if (Random.Range(0f, 1f) < 0.9f)
            {
                float randomX = xPositions[Random.Range(0, xPositions.Length)];
                Vector3 spawnPosition = new Vector3(randomX, 0.5f, z);

                // 여러 아이템 프리팹 중 하나를 랜덤으로 선택
                GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
                NetworkObject networkObject = item.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    networkObject.Spawn();
                }
                if(itemParent != null)
                    item.transform.SetParent(itemParent.transform); // itemParent 하위로 설정
            }
        }
    }
}
