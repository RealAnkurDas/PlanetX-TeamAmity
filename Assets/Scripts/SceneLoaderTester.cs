using UnityEngine;

/// <summary>
/// Test script to simulate Kotlin calling LoadSceneByName
/// Attach to any GameObject and use GUI button or context menu to test
/// </summary>
public class SceneLoaderTester : MonoBehaviour
{
    [Header("Test Settings")]
    [Tooltip("Scene name to load when testing")]
    [SerializeField] private string testSceneName = "MBR Part View 3D";

    // No Update() needed - using GUI buttons and context menu instead

    void OnGUI()
    {
        // Draw test button on screen
        GUI.Box(new Rect(10, 10, 300, 150), "SceneLoader Tester");
        
        GUI.Label(new Rect(20, 35, 280, 20), $"Test Scene: {testSceneName}");
        
        if (GUI.Button(new Rect(20, 60, 280, 35), $"▶ Load Scene: {testSceneName}"))
        {
            TestLoadScene();
        }
        
        if (GUI.Button(new Rect(20, 100, 280, 35), "🔍 Test All Scenes (Validate)"))
        {
            TestAllScenes();
        }
    }

    void TestLoadScene()
    {
        Debug.Log("========================================");
        Debug.Log($"🧪 TESTING SceneLoader.LoadSceneByName('{testSceneName}')");
        Debug.Log("========================================");
        
        // Find the SceneLoader GameObject
        GameObject sceneLoaderObj = GameObject.Find("SceneLoader");
        
        if (sceneLoaderObj == null)
        {
            Debug.LogError("❌ FAILED: SceneLoader GameObject not found!");
            Debug.LogError("Create a GameObject named 'SceneLoader' and attach SceneLoader.cs");
            return;
        }
        
        // Get the SceneLoader component
        SceneLoader sceneLoader = sceneLoaderObj.GetComponent<SceneLoader>();
        
        if (sceneLoader == null)
        {
            Debug.LogError("❌ FAILED: SceneLoader component not found on GameObject!");
            Debug.LogError("Attach SceneLoader.cs script to the SceneLoader GameObject");
            return;
        }
        
        Debug.Log("✅ SceneLoader GameObject found");
        Debug.Log("✅ SceneLoader component found");
        Debug.Log($"🚀 Calling LoadSceneByName('{testSceneName}')...");
        
        // Call the function (simulates Kotlin calling it)
        sceneLoader.LoadSceneByName(testSceneName);
    }

    // Test multiple scenes in sequence
    [ContextMenu("Test All Scenes")]
    void TestAllScenes()
    {
        string[] scenesToTest = new string[]
        {
            "MBR Part View 3D",
            "Solar System Animated View 3D",
            "Solar System Animated View AR",
            "Solar System Animated View 3D GA",
            "Solar System Animated View 3D User"
        };

        Debug.Log("========================================");
        Debug.Log("🧪 TESTING ALL SCENES");
        Debug.Log("========================================");

        GameObject sceneLoaderObj = GameObject.Find("SceneLoader");
        if (sceneLoaderObj == null)
        {
            Debug.LogError("❌ SceneLoader GameObject not found!");
            return;
        }

        SceneLoader sceneLoader = sceneLoaderObj.GetComponent<SceneLoader>();
        if (sceneLoader == null)
        {
            Debug.LogError("❌ SceneLoader component not found!");
            return;
        }

        foreach (string sceneName in scenesToTest)
        {
            Debug.Log($"Checking scene: '{sceneName}'...");
            // Just validate, don't actually load (would cause issues)
            bool canLoad = UnityEngine.Application.CanStreamedLevelBeLoaded(sceneName);
            if (canLoad)
            {
                Debug.Log($"  ✅ Scene '{sceneName}' is accessible");
            }
            else
            {
                Debug.LogWarning($"  ❌ Scene '{sceneName}' NOT in Build Settings!");
            }
        }

        Debug.Log("========================================");
        Debug.Log("To load a scene, press Space or click the button");
        Debug.Log("========================================");
    }
}

