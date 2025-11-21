using UnityEngine;

/// <summary>
/// Provides functionality to exit Unity and return to the native Android/iOS application.
/// Used when Unity is integrated as a library in a native app.
/// </summary>
public class ExitToNativeApp : MonoBehaviour
{
    /// <summary>
    /// Call this method to unload Unity and return control to the native app.
    /// This triggers IUnityPlayerLifecycleEvents.onUnityPlayerUnloaded() in the native code.
    /// </summary>
    public void ReturnToNativeApp()
    {
        Debug.Log("ExitToNativeApp: Returning to native application...");
        
        // Unload Unity runtime - this will trigger the lifecycle event in the native app
        // where you can handle the transition back to native UI
        Application.Unload();
    }
    
    /// <summary>
    /// Alternative method to completely quit Unity (ends the process).
    /// Only use this if you don't need to reload Unity again.
    /// </summary>
    public void QuitUnityCompletely()
    {
        Debug.Log("ExitToNativeApp: Quitting Unity completely...");
        Application.Quit();
    }
}

