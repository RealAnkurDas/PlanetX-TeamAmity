using UnityEngine;

/// <summary>
/// Exits the Unity application when triggered.
/// Attach this to your Exit to App button GameObject.
/// </summary>
public class ExitApplication : MonoBehaviour
{
    /// <summary>
    /// Exits the application immediately.
    /// Connect this to your button's OnClick event.
    /// </summary>
    public void ExitApp()
    {
        Debug.Log("Exiting application...");
        
        #if UNITY_EDITOR
        // Stop play mode in Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Quit the application on device/build
        Application.Quit();
        #endif
    }
}

