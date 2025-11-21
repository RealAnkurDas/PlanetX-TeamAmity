using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Automatically scales specific objects when spawned in AR
/// Attach to Object Spawner GameObject
/// </summary>
public class ARSpawnScaleAdjuster : MonoBehaviour
{
    [Header("Scale Settings")]
    [Tooltip("Scale multiplier for MBR spacecraft when spawned")]
    [SerializeField] private float mbrSpawnScale = 0.1f; // 10% of original size
    
    [Tooltip("Name of the MBR prefab (to detect when it's spawned)")]
    [SerializeField] private string mbrPrefabName = "MBR";
    
    private ObjectSpawner spawner;
    private GameObject lastSpawnedObject;
    private int lastSpawnCount = 0;
    
    void Start()
    {
        spawner = GetComponent<ObjectSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning("ARSpawnScaleAdjuster: No ObjectSpawner found on this GameObject");
        }
    }
    
    void Update()
    {
        // Check if new objects were spawned
        if (spawner != null)
        {
            // Count spawned objects (children of spawner)
            int currentCount = transform.childCount;
            
            if (currentCount > lastSpawnCount)
            {
                // New object was spawned, check if it's MBR
                Transform lastChild = transform.GetChild(currentCount - 1);
                GameObject spawnedObject = lastChild.gameObject;
                
                if (spawnedObject.name.Contains(mbrPrefabName))
                {
                    // Scale down the MBR spacecraft
                    spawnedObject.transform.localScale = Vector3.one * mbrSpawnScale;
                    Debug.Log($"ARSpawnScaleAdjuster: Scaled MBR spacecraft to {mbrSpawnScale}x");
                }
            }
            
            lastSpawnCount = currentCount;
        }
    }
    
    /// <summary>
    /// Change the spawn scale for MBR spacecraft
    /// </summary>
    public void SetMBRScale(float scale)
    {
        mbrSpawnScale = scale;
        Debug.Log($"ARSpawnScaleAdjuster: MBR spawn scale set to {scale}");
    }
}

