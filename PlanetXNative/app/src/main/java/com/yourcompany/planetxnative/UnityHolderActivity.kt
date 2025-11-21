package com.yourcompany.planetxnative

import android.os.Bundle
import android.os.Handler
import android.os.Looper

class UnityHolderActivity : com.unity3d.player.UnityPlayerGameActivity() {
    companion object {
        const val EXTRA_SCENE_NAME = "unitySceneName"
    }
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        
        // Check for scene index first (new approach)
        val sceneIndex = intent.getIntExtra("targetSceneIndex", -1)
        
        // Fall back to scene name if scene index not provided (legacy support)
        val sceneName = intent.getStringExtra(EXTRA_SCENE_NAME)
        
        // Wait for Unity to initialize, then send the appropriate message
        Handler(Looper.getMainLooper()).postDelayed({
            try {
                // Use reflection to call UnityPlayer.UnitySendMessage
                val unityPlayerClass = Class.forName("com.unity3d.player.UnityPlayer")
                val sendMessageMethod = unityPlayerClass.getDeclaredMethod(
                    "UnitySendMessage",
                    String::class.java,
                    String::class.java,
                    String::class.java
                )
                
                if (sceneIndex >= 0) {
                    // Send scene index to Unity's SceneMenu
                    sendMessageMethod.invoke(null, "SceneMenu", "LoadSceneByIndex", sceneIndex.toString())
                } else if (!sceneName.isNullOrEmpty()) {
                    // Legacy: Send scene name to Unity's SceneLoader
                    sendMessageMethod.invoke(null, "SceneLoader", "LoadSceneByName", sceneName)
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }, 1000)
    }
}

