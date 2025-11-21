# 🚀 Loading Scenes from Kotlin

This guide shows how to launch Unity scenes directly from your Kotlin app, bypassing the menu screen.

---

## 📋 **Scene Index Reference**

Based on your Unity Build Settings order:

```kotlin
// Scene indices (0-based)
const val SCENE_MENU = 0                    // Menu screen (default)
const val SCENE_SOLAR_SYSTEM_3D = 1         // Solar System 3D View
const val SCENE_SOLAR_SYSTEM_AR = 2         // AR Solar System
const val SCENE_MBR_EXPLORER = 3            // MBR Explorer Parts
const val SCENE_GA_TRAJECTORY = 4           // GA Trajectory
const val SCENE_CUSTOM_LAUNCH = 5           // Custom Launch
```

---

## ✅ **Method 1: Intent Extras (Recommended)**

Launch Unity and skip directly to a scene:

```kotlin
import android.content.Intent
import com.unity3d.player.UnityPlayerActivity

// In your Kotlin Activity:
fun launchUnityScene(sceneIndex: Int) {
    val intent = Intent(this, UnityPlayerActivity::class.java)
    intent.putExtra("targetSceneIndex", sceneIndex)
    startActivity(intent)
}

// Examples:
launchUnityScene(SCENE_SOLAR_SYSTEM_AR)    // Go directly to AR view
launchUnityScene(SCENE_MBR_EXPLORER)       // Go directly to MBR explorer
launchUnityScene(SCENE_MENU)               // Show menu (default behavior)
```

### **Complete Example:**

```kotlin
class MainActivity : AppCompatActivity() {
    
    companion object {
        const val SCENE_SOLAR_SYSTEM_3D = 1
        const val SCENE_SOLAR_SYSTEM_AR = 2
        const val SCENE_MBR_EXPLORER = 3
        const val SCENE_GA_TRAJECTORY = 4
        const val SCENE_CUSTOM_LAUNCH = 5
    }
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        // Button click handlers
        btnARView.setOnClickListener {
            launchUnityWithScene(SCENE_SOLAR_SYSTEM_AR)
        }
        
        btnMBRExplorer.setOnClickListener {
            launchUnityWithScene(SCENE_MBR_EXPLORER)
        }
        
        btnShowMenu.setOnClickListener {
            launchUnityWithScene(-1)  // Don't pass index, show menu
        }
    }
    
    private fun launchUnityWithScene(sceneIndex: Int) {
        val intent = Intent(this, UnityPlayerActivity::class.java)
        
        if (sceneIndex >= 0) {
            intent.putExtra("targetSceneIndex", sceneIndex)
        }
        
        startActivity(intent)
    }
}
```

---

## 🔄 **Method 2: Runtime Scene Switching**

Switch scenes while Unity is already running:

```kotlin
import com.unity3d.player.UnityPlayer

// Switch to scene by index (0-4)
UnityPlayer.UnitySendMessage("SceneLoader", "LoadSceneByIndex", "2")

// Switch to scene by name
UnityPlayer.UnitySendMessage("SceneLoader", "LoadSceneByName", "Solar System Animated View AR")
```

**Note:** This requires the `SceneLoader` GameObject to exist in your scene.

---

## 🧪 **Testing in Unity Editor**

Before building for Android, test the auto-load in Unity Editor:

1. Open `SceneMenu.cs`
2. Find line 81: `int testSceneIndex = -1;`
3. Change to: `int testSceneIndex = 2;` (or any scene 0-5)
4. Press Play in Unity
5. It should skip the menu and load scene 2 directly!
6. **Don't forget to set it back to -1 before building!**

---

## 📱 **How It Works**

1. **Kotlin launches Unity** with `targetSceneIndex` Intent extra
2. **Unity always starts at Scene 0** (the Menu Scene)
3. **Menu Scene reads Intent** in `Start()`
4. If `targetSceneIndex` exists, it **immediately jumps** to that scene
5. Menu is hidden and bypassed (~0.1 second, imperceptible)

---

## ⚠️ **Important Notes**

### **Scene Indices Must Match Build Settings:**

Your scenes must be in this exact order in Unity:
1. Open Unity: `File` → `Build Settings`
2. Verify scene order matches your Kotlin constants
3. Scene 0 should be your Menu Scene
4. Scenes 1-5 should be in the order you expect

### **UnityPlayerActivity Setup:**

Make sure your `AndroidManifest.xml` has:

```xml
<activity
    android:name="com.unity3d.player.UnityPlayerActivity"
    android:exported="true"
    android:launchMode="singleTask"
    android:configChanges="mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density"
    android:screenOrientation="fullSensor">
</activity>
```

---

## 🎯 **Example: Menu with Scene Buttons**

```kotlin
class UnityLauncherActivity : AppCompatActivity() {
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_unity_launcher)
        
        // Create buttons for each scene
        setupSceneButtons()
    }
    
    private fun setupSceneButtons() {
        val scenes = listOf(
            Triple(1, "Solar System 3D", R.id.btn_solar_3d),
            Triple(2, "AR Solar System", R.id.btn_solar_ar),
            Triple(3, "MBR Explorer", R.id.btn_mbr),
            Triple(4, "GA Trajectory", R.id.btn_ga),
            Triple(5, "Custom Launch", R.id.btn_custom)
        )
        
        scenes.forEach { (index, name, buttonId) ->
            findViewById<Button>(buttonId).setOnClickListener {
                Toast.makeText(this, "Loading $name...", Toast.LENGTH_SHORT).show()
                launchUnity(index)
            }
        }
    }
    
    private fun launchUnity(sceneIndex: Int) {
        val intent = Intent(this, UnityPlayerActivity::class.java).apply {
            putExtra("targetSceneIndex", sceneIndex)
        }
        startActivity(intent)
    }
}
```

---

## 🐛 **Debugging**

If scenes aren't loading:

1. **Check Unity Logs** (via Android Logcat):
   - Look for: `"🚀 SceneMenu: Direct scene load requested"`
   - Or errors: `"❌ SceneMenu: Invalid scene index"`

2. **Verify Scene Indices**:
   - In Unity: `File` → `Build Settings`
   - Count scenes from 0 (Menu = 0, first scene = 1, etc.)

3. **Check Intent Extra Name**:
   - Must be exactly: `"targetSceneIndex"`
   - Case-sensitive!

---

---

## 🎲 **User Scene (Custom Launch) - Random Velocity**

The **Custom Launch scene** (SCENE_CUSTOM_LAUNCH = 5) now automatically generates **random velocity values** when launched!

### **Simple Launch (No Velocity Needed):**

```kotlin
// Just launch the scene - Unity will generate random velocity automatically!
fun launchUserScene() {
    val intent = Intent(this, UnityPlayerActivity::class.java)
    intent.putExtra("targetSceneIndex", SCENE_CUSTOM_LAUNCH)  // Index 5
    startActivity(intent)
    // No velocity parameters needed! 🎉
}
```

### **How It Works:**

1. **Unity checks** for velocity values from Kotlin Intent
2. **If none found** → Generates random velocity automatically
3. **Random ranges** (configurable in Unity Inspector):
   - X: -0.02 to 0.02 m/s
   - Y: -0.02 to 0.02 m/s  
   - Z: 0.02 to 0.04 m/s (forward motion)

### **Override with Custom Velocity (Optional):**

If you DO want to pass specific velocity from Kotlin:

```kotlin
fun launchUserSceneWithVelocity(vx: Float, vy: Float, vz: Float) {
    val intent = Intent(this, UnityPlayerActivity::class.java)
    intent.putExtra("targetSceneIndex", SCENE_CUSTOM_LAUNCH)
    intent.putExtra("velocityX", vx)
    intent.putExtra("velocityY", vy)
    intent.putExtra("velocityZ", vz)
    startActivity(intent)
    // Unity will use these instead of random
}
```

### **Configure Random Ranges in Unity:**

1. Open **"Solar System Animated View 3D User"** scene
2. Find **KotlinLaunchReceiver** component
3. Adjust **Random Velocity Settings**:
   - **Use Random Velocity**: ☑ Enabled (default)
   - **Random Velocity X Range**: Min/Max values
   - **Random Velocity Y Range**: Min/Max values
   - **Random Velocity Z Range**: Min/Max values

---

## 🎉 **You're All Set!**

Now you can:
- ✅ Launch Unity directly to any scene from Kotlin
- ✅ Bypass the menu screen completely
- ✅ **User Scene uses random velocity automatically** 🎲
- ✅ Create a native Kotlin menu for better UX
- ✅ Test in Unity Editor before building

Happy coding! 🚀

