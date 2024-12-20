using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObstacleSpawner : NetworkBehaviour
{
    public GameObject rockPrefab;
    public GameObject tirePrefab;
    public GameObject treePrefab;
    public float cityInterval = 10f;
    public Vector3 spawnVector;

    public GameObject obstacleParent;

    void Start()
    {
        StartCoroutine(CitySpawnObstacleCoroutine()); // ���� ���� ��ֹ� ����(����)
        StartCoroutine(GrassSpawnObstacleCoroutine()); // ���� ���� ��ֹ� ����(�ڿ�)
    }

    IEnumerator CitySpawnObstacleCoroutine() // ���� �׸� ��ֹ� �ڷ�ƾ
    {
        while (true)
        {
            CitySpawnObstacle(); // ��ֹ� ����
            yield return new WaitForSeconds(cityInterval); // 3�� ���
        }
    }
    IEnumerator GrassSpawnObstacleCoroutine() // ���� �׸� ��ֹ� �ڷ�ƾ
    {
        while (true)
        {
            GrassSpawnObstacle(); // ��ֹ� ����
            yield return new WaitForSeconds(cityInterval); // 2�� ���
        }
    }

    void CitySpawnObstacle()
    {
        //GameObject cityObstacleToSpawn = Random.Range(0f, 1f) > 0.5f ? rockPrefab : tirePrefab;
        string cityObstacleToSpawn = Random.Range(0f, 1f) > 0.5f ? "Obstacles/Obstacle_Tire" : "Obstacles/Obstacle_Rock";
        Vector3 spawnPosition = new Vector3(0, 1, 680);

        //GameObject cityObstacle = Instantiate(cityObstacleToSpawn, spawnPosition, Quaternion.identity);
        //NetworkObject networkObject = cityObstacle.GetComponent<NetworkObject>();
        //if (networkObject != null)
        //{
        //    networkObject.Spawn();
        //}
        //cityObstacle.transform.SetParent(cityParent.transform);

        GameObject cityObstacle = Managers.Resource.Instantiate(cityObstacleToSpawn, spawnPosition);
        cityObstacle.transform.rotation = Quaternion.identity;

        if(obstacleParent != null) 
            cityObstacle.transform.SetParent(obstacleParent.transform);
    }

    void GrassSpawnObstacle()
    {
        GameObject grassObstacleToSpawn = treePrefab;
        for(int i = 0; i < 30; i++)
        {
            float randX = Random.Range(0f, 1f) > 0.5 ? -28 : 28;
            float randZ = Random.Range(1150, 710);
            Vector3 spawnPosition = new Vector3(randX, 1, randZ);
            Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);

            GameObject grassObstacle = Managers.Resource.Instantiate("Obstacles/Obstacle_Tree", spawnPosition);
            grassObstacle.transform.rotation = rotation;
    
            if(obstacleParent != null)
                grassObstacle.transform.SetParent(obstacleParent.transform);

            grassObstacle.GetComponent<Obtsacles_Tree>().Activate();

            //GameObject grassObstacle = Instantiate(grassObstacleToSpawn, spawnPosition, rotation);
            //NetworkObject networkObject = grassObstacle.GetComponent<NetworkObject>();
            //if (networkObject != null)
            //{
            //    networkObject.Spawn();
            //}
            //grassObstacle.transform.SetParent(grassParent.transform);
        }
    }
}
