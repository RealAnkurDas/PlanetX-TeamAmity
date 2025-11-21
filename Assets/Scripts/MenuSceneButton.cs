using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Attach to UI buttons to load specific scenes
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuSceneButton : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("Name of the scene to load when this button is clicked")]
    [SerializeField] private string sceneName;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        if (showDebugLogs)
        {
            Debug.Log($"MenuSceneButton: Button '{gameObject.name}' will load scene '{sceneName}'");
        }
    }

    void OnButtonClick()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"MenuSceneButton: Scene name not set for button '{gameObject.name}'!");
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log($"🎬 MenuSceneButton: Loading scene '{sceneName}'...");
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"❌ MenuSceneButton: Scene '{sceneName}' not found in Build Settings!");
            Debug.LogError("Add the scene to File → Build Settings → Scenes in Build");
        }
    }

    // Public method to set scene name from code
    public void SetSceneName(string newSceneName)
    {
        sceneName = newSceneName;
    }
}

