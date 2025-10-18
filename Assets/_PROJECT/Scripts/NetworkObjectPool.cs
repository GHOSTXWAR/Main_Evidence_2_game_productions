using DilmerGames.Core.Singletons;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkObjectPool : Singleton<NetworkObjectPool>
{
    [Header("Network Manager Reference")]
    [SerializeField]
    private NetworkManager m_NetworkManager;

    [Header("Prefabs to Pool")]
    [SerializeField]
    private List<PoolConfigObject> PooledPrefabsList = new List<PoolConfigObject>();

    private readonly HashSet<GameObject> prefabs = new HashSet<GameObject>();
    private readonly Dictionary<GameObject, Queue<NetworkObject>> pooledObjects = new Dictionary<GameObject, Queue<NetworkObject>>();

    private void Awake()
    {
        // Ensure the NetworkManager is assigned
        if (m_NetworkManager == null)
            m_NetworkManager = NetworkManager.Singleton;

        // Initialize the pool once at startup
        InitializePool();
    }

    private void OnValidate()
    {
        for (var i = 0; i < PooledPrefabsList.Count; i++)
        {
            var prefab = PooledPrefabsList[i].Prefab;
            if (prefab != null)
            {
                Assert.IsNotNull(prefab.GetComponent<NetworkObject>(),
                    $"{nameof(NetworkObjectPool)}: Pooled prefab \"{prefab.name}\" at index {i} has no {nameof(NetworkObject)} component.");
            }
        }
    }

    /// <summary>
    /// Gets an instance of the given prefab from the pool.
    /// </summary>
    public NetworkObject GetNetworkObject(GameObject prefab)
    {
        return GetNetworkObjectInternal(prefab, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Gets an instance of the given prefab from the pool.
    /// </summary>
    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return GetNetworkObjectInternal(prefab, position, rotation);
    }

    /// <summary>
    /// Returns an object to the pool and disables it.
    /// </summary>
    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab)
    {
        if (!pooledObjects.ContainsKey(prefab))
        {
            Debug.LogWarning($"Tried to return an object for prefab '{prefab.name}' that isn't registered in the pool!");
            Destroy(networkObject.gameObject);
            return;
        }

        var go = networkObject.gameObject;
        go.SetActive(false);
        pooledObjects[prefab].Enqueue(networkObject);
    }

    /// <summary>
    /// Adds a prefab to the pool registry.
    /// </summary>
    public void AddPrefab(GameObject prefab, int prewarmCount = 0)
    {
        var networkObject = prefab.GetComponent<NetworkObject>();
        Assert.IsNotNull(networkObject, $"{nameof(prefab)} must have a {nameof(NetworkObject)} component.");
        Assert.IsFalse(prefabs.Contains(prefab), $"Prefab {prefab.name} is already registered in the pool.");

        RegisterPrefabInternal(prefab, prewarmCount);
    }

    /// <summary>
    /// Builds up the cache for a prefab.
    /// </summary>
    private void RegisterPrefabInternal(GameObject prefab, int prewarmCount)
    {
        prefabs.Add(prefab);

        var prefabQueue = new Queue<NetworkObject>();
        pooledObjects[prefab] = prefabQueue;

        // Optionally pre-spawn objects to warm up the pool
        for (int i = 0; i < prewarmCount; i++)
        {
            var go = CreateInstance(prefab);
            ReturnNetworkObject(go.GetComponent<NetworkObject>(), prefab);
        }

        // Register MLAPI/Netcode spawn handlers
        if (m_NetworkManager != null)
        {
            m_NetworkManager.PrefabHandler.AddHandler(prefab, new DummyPrefabInstanceHandler(prefab, this));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private GameObject CreateInstance(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    /// <summary>
    /// Internal method that handles pulling from the queue or instantiating new objects.
    /// </summary>
    private NetworkObject GetNetworkObjectInternal(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!pooledObjects.ContainsKey(prefab))
        {
            Debug.LogError($"Prefab '{prefab.name}' is not registered in the NetworkObjectPool! " +
                           $"Make sure it is added to PooledPrefabsList and InitializePool() has been called.");
            return null;
        }

        var queue = pooledObjects[prefab];

        NetworkObject networkObject;
        if (queue.Count > 0)
        {
            networkObject = queue.Dequeue();
        }
        else
        {
            networkObject = CreateInstance(prefab).GetComponent<NetworkObject>();
        }

        // Activate and position the object
        var go = networkObject.gameObject;
        go.transform.SetParent(null);
        go.SetActive(true);
        go.transform.position = position;
        go.transform.rotation = rotation;

        return networkObject;
    }

    /// <summary>
    /// Registers all prefabs in the configured list to the cache.
    /// </summary>
    public void InitializePool()
    {
        foreach (var configObject in PooledPrefabsList)
        {
            if (configObject.Prefab == null)
            {
                Debug.LogWarning("One or more prefab slots in PooledPrefabsList are empty.");
                continue;
            }

            if (!prefabs.Contains(configObject.Prefab))
            {
                RegisterPrefabInternal(configObject.Prefab, configObject.PrewarmCount);
            }
        }
    }
}

[Serializable]
public struct PoolConfigObject
{
    public GameObject Prefab;
    public int PrewarmCount;
}

public class DummyPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    private readonly GameObject m_Prefab;
    private readonly NetworkObjectPool m_Pool;

    public DummyPrefabInstanceHandler(GameObject prefab, NetworkObjectPool pool)
    {
        m_Prefab = prefab;
        m_Pool = pool;
    }

    public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return m_Pool.GetNetworkObject(m_Prefab, position, rotation);
    }

    public void Destroy(NetworkObject networkObject)
    {
        m_Pool.ReturnNetworkObject(networkObject, m_Prefab);
    }
}
