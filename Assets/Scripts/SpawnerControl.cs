using DilmerGames.Core.Singletons;
using Unity.Netcode;
using UnityEngine;

public class SpawnerControl : NetworkSingleton<SpawnerControl>
{
    [Header("Prefab Settings")]
    [SerializeField]
    private GameObject objectPrefab;

    [SerializeField]
    private int maxObjectInstanceCount = 3;

    private bool poolInitialized = false;

    private void Start()
    {
       
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }
        else
        {
            Debug.LogError("NetworkManager.Singleton is null! Make sure there is a NetworkManager in the scene.");
        }
    }

    private void OnServerStarted()
    {
        if (NetworkObjectPool.Instance != null)
        {
            NetworkObjectPool.Instance.InitializePool();
            poolInitialized = true;
        }
        else
        {
            Debug.LogError("NetworkObjectPool.Instance is null! Make sure the pool exists in the scene.");
        }
    }

    public void SpawnObjects()
    {
        
        if (!IsServer) return;

       
        if (objectPrefab == null)
        {
            Debug.LogError("objectPrefab is not assigned in the Inspector!");
            return;
        }

        if (!poolInitialized)
        {
            Debug.LogWarning("NetworkObjectPool is not initialized yet. Cannot spawn objects.");
            return;
        }

        for (int i = 0; i < maxObjectInstanceCount; i++)
        {
            var netObj = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab);

            if (netObj == null)
            {
                Debug.LogWarning($"Failed to get a network object for {objectPrefab.name}");
                continue;
            }

            GameObject go = netObj.gameObject;

            go.transform.position = new Vector3(
                Random.Range(-10f, 10f),
                10f,
                Random.Range(-10f, 10f)
            );

          
            netObj.Spawn();
        }
    }
}
