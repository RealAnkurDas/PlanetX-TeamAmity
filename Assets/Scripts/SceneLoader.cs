using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Receives scene load requests from Kotlin via UnitySendMessage
/// GameObject must be named "SceneLoader" to receive messages
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    void Awake()
    {
        // Persist across scene loads
        DontDestroyOnLoad(gameObject);
        
        // Ensure GameObject has the correct name for Kotlin to find it
        gameObject.name = "SceneLoader";
        
        if (showDebugLogs)
        {
            Debug.Log("✅ SceneLoader initialized and ready to receive messages from Kotlin");
        }
    }

    /// <summary>
    /// Called from Kotlin via UnityPlayer.UnitySendMessage("SceneLoader", "LoadSceneByName", sceneName)
    /// </summary>
    /// <param name="sceneName">Name of the scene to load (must be in Build Settings)</param>
    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("❌ SceneLoader: Received empty scene name from Kotlin!");
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log($"📩 SceneLoader: Received scene load request from Kotlin: '{sceneName}'");
        }

        // Check if scene exists in build settings
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            if (showDebugLogs)
            {
                Debug.Log($"✅ SceneLoader: Loading scene '{sceneName}'...");
            }
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"❌ SceneLoader: Scene '{sceneName}' not found in Build Settings!");
            Debug.LogError("Please add this scene to File → Build Settings → Scenes in Build");
            
            // List available scenes for debugging
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            Debug.Log($"Available scenes in Build Settings ({sceneCount}):");
            for (int i = 0; i < sceneCount; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                Debug.Log($"  [{i}] {name}");
            }
        }
    }

    /// <summary>
    /// Alternative method to load scene by build index (if Kotlin sends an integer)
    /// </summary>
    public void LoadSceneByIndex(string indexString)
    {
        if (int.TryParse(indexString, out int index))
        {
            if (showDebugLogs)
            {
                Debug.Log($"📩 SceneLoader: Loading scene by index: {index}");
            }
            
            if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(index);
            }
            else
            {
                Debug.LogError($"❌ SceneLoader: Scene index {index} out of range (0-{SceneManager.sceneCountInBuildSettings - 1})");
            }
        }
        else
        {
            Debug.LogError($"❌ SceneLoader: Invalid scene index: '{indexString}'");
        }
    }
}

