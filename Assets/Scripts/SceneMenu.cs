using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple scene selector menu for the landing scene
/// Shows buttons to load all available scenes
/// </summary>
public class SceneMenu : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] private bool showMenu = true;

    [Header("Scene Names")]
    [SerializeField] private string[] sceneNames = new string[]
    {
        "Solar System Animated View 3D",
        "Solar System Animated View AR",
        "MBR Part View 3D",
        "Solar System Animated View 3D GA",
        "Solar System Animated View 3D User"
    };

    [Header("Display Names (Optional)")]
    [SerializeField] private string[] displayNames = new string[]
    {
        "🌍 Solar System 3D View",
        "📱 AR Solar System",
        "🛰️ MBR Explorer Parts",
        "🎯 GA Trajectory",
        "🚀 Custom Launch"
    };

    [Header("Menu Styling")]
    [SerializeField] private int menuWidth = 400;
    [SerializeField] private int buttonHeight = 60;
    [SerializeField] private int padding = 20;
    [SerializeField] private int fontSize = 20;

    private GUIStyle buttonStyle;
    private GUIStyle titleStyle;
    private GUIStyle boxStyle;
    private bool stylesInitialized = false;

    void Start()
    {
        CheckForDirectSceneLoad();
    }

    void CheckForDirectSceneLoad()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            
            // Check if scene index was passed from Kotlin
            int targetScene = intent.Call<int>("getIntExtra", "targetSceneIndex", -1);
            
            if (targetScene >= 0 && targetScene < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"🚀 SceneMenu: Direct scene load requested from Kotlin: Scene {targetScene}");
                HideMenu();
                SceneManager.LoadScene(targetScene);
            }
            else if (targetScene >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"❌ SceneMenu: Invalid scene index {targetScene} (max: {SceneManager.sceneCountInBuildSettings - 1})");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ SceneMenu: Error reading Intent from Kotlin: {e.Message}");
        }
        #endif
        
        #if UNITY_EDITOR
        // For testing in Unity Editor: Change testSceneIndex to test auto-load
        // Set to -1 to show menu, or 0-4 to skip directly to that scene
        int testSceneIndex = -1;
        
        if (testSceneIndex >= 0 && testSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"🧪 SceneMenu: [EDITOR TEST] Auto-loading scene {testSceneIndex}");
            HideMenu();
            SceneManager.LoadScene(testSceneIndex);
        }
        #endif
    }

    void OnGUI()
    {
        if (!showMenu) return;

        // Initialize styles once
        if (!stylesInitialized)
        {
            InitializeStyles();
        }

        // Calculate menu dimensions
        int menuHeight = padding * 3 + 80 + (sceneNames.Length * (buttonHeight + 10));
        int menuX = (Screen.width - menuWidth) / 2;
        int menuY = (Screen.height - menuHeight) / 2;

        // Draw menu background
        GUI.Box(new Rect(menuX, menuY, menuWidth, menuHeight), "SCENE SELECTOR", boxStyle);

        // Draw title
        GUI.Label(new Rect(menuX + padding, menuY + 40, menuWidth - padding * 2, 40), 
            "Select a Scene to Load", titleStyle);

        // Draw scene buttons
        int currentY = menuY + 100;
        for (int i = 0; i < sceneNames.Length; i++)
        {
            string displayName = (displayNames != null && i < displayNames.Length) 
                ? displayNames[i] 
                : sceneNames[i];

            if (GUI.Button(new Rect(menuX + padding, currentY, menuWidth - padding * 2, buttonHeight), 
                displayName, buttonStyle))
            {
                LoadScene(sceneNames[i]);
            }

            currentY += buttonHeight + 10;
        }

        // Draw help text
        GUI.Label(new Rect(menuX + padding, menuY + menuHeight - 40, menuWidth - padding * 2, 30),
            "Click a button to load a scene", GUI.skin.label);
    }

    void InitializeStyles()
    {
        // Button style
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = fontSize;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.hover.textColor = Color.yellow;

        // Title style
        titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 18;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.white;

        // Box style
        boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 24;
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.alignment = TextAnchor.UpperCenter;
        boxStyle.normal.textColor = Color.cyan;
        boxStyle.padding = new RectOffset(10, 10, 20, 10);

        stylesInitialized = true;
    }

    void LoadScene(string sceneName)
    {
        Debug.Log($"🎬 SceneMenu: Loading scene '{sceneName}'...");
        
        // Check if scene exists
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"❌ SceneMenu: Scene '{sceneName}' not found in Build Settings!");
            Debug.LogError("Add the scene to File → Build Settings → Scenes in Build");
        }
    }

    // Public method that can be called from Kotlin via UnitySendMessage
    public void LoadSceneByName(string sceneName)
    {
        LoadScene(sceneName);
    }

    // Hide menu (can be called from other scripts)
    public void HideMenu()
    {
        showMenu = false;
    }

    // Show menu (can be called from other scripts)
    public void ShowMenu()
    {
        showMenu = true;
    }
}

