# 🚀 Kotlin Scene Loader Setup

This guide shows how to set up Unity to receive scene load requests from your Kotlin app.

---

## 📋 **What This Does**

Your Kotlin app sends scene names to Unity using:
```kotlin
UnityPlayer.UnitySendMessage("SceneLoader", "LoadSceneByName", sceneName)
```

Unity receives this message and loads the requested scene.

---

## ⚙️ **Unity Setup Steps**

### **Step 1: Create SceneLoader GameObject**

1. Open your **initial/startup scene** (the one that loads first when Unity starts)
   - This is probably your first scene in Build Settings
   - Or create a new empty scene called `"Startup"` if you want

2. **Create the GameObject:**
   - In Hierarchy: Right-click → `Create Empty`
   - Name it **exactly** `"SceneLoader"` (case-sensitive!)

3. **Attach the Script:**
   - Select the `SceneLoader` GameObject
   - In Inspector: `Add Component` → Search for `SceneLoader`
   - The script is at `Assets/Scripts/SceneLoader.cs`

4. **Verify Settings:**
   - Check `Show Debug Logs` is enabled (to see messages from Kotlin)

---

### **Step 2: Add All Scenes to Build Settings**

**CRITICAL:** Unity can only load scenes that are in Build Settings!

1. Open `File` → `Build Settings`

2. **Add your 5 scenes** (drag from Project window into "Scenes in Build"):
   - ✅ `MBR Part View 3D`
   - ✅ `Solar System Animated View 3D`
   - ✅ `Solar System Animated View AR`
   - ✅ `Solar System Animated View 3D GA`
   - ✅ `Solar System Animated View 3D User`

3. **Also add your startup scene** (if different):
   - The scene with the SceneLoader GameObject should be **index 0** (first in list)

4. **Order doesn't matter** (except for the startup scene at top)

**Example Build Settings:**
```
☑ Startup (index 0)                    <- Has SceneLoader GameObject
☑ MBR Part View 3D (index 1)
☑ Solar System Animated View 3D (index 2)
☑ Solar System Animated View AR (index 3)
☑ Solar System Animated View 3D GA (index 4)
☑ Solar System Animated View 3D User (index 5)
```

---

### **Step 3: Verify Scene Names**

The scene names in Kotlin **must exactly match** the names in Unity.

**Check your scene names:**
1. In Project window, find each scene file (`.unity`)
2. **The filename** (without `.unity`) is the scene name Kotlin should use

**Example:**
- File: `Assets/Scenes/MBR Part View 3D.unity`
- Kotlin should send: `"MBR Part View 3D"` ✅
- NOT: `"MBRPartView3D"` ❌

---

### **Step 4: Export to Android**

1. `File` → `Build Settings` → `Android`

2. **Verify Build Settings:**
   - ✅ All 6 scenes checked
   - ✅ Startup scene at index 0

3. **Export as Library:**
   - Click `Export` button (NOT "Build")
   - Choose export location: `C:\Users\ankur\UnityExports\PlanetX`
   - Wait for export to complete

4. **Update Kotlin Path:**
   - In Android Studio, update `settings.gradle.kts`:
   ```kotlin
   project(":unityLibrary").projectDir = file("C:/Users/ankur/UnityExports/PlanetX/unityLibrary")
   ```

---

## 🧪 **Testing in Unity Editor**

Before exporting, test that the SceneLoader works:

1. **Enter Play Mode** in your startup scene

2. **Open Console** (Window → General → Console)

3. You should see:
   ```
   ✅ SceneLoader initialized and ready to receive messages from Kotlin
   ```

4. **Test manually** (optional):
   - Select the `SceneLoader` GameObject in Hierarchy
   - In Inspector, find the `LoadSceneByName` method
   - You can't call it directly, but you can verify the script compiles

---

## ❌ **Troubleshooting**

### **"Scene not found in Build Settings"**
```
❌ SceneLoader: Scene 'MBR Part View 3D' not found in Build Settings!
```

**Fix:**
1. `File` → `Build Settings`
2. Drag the missing scene into "Scenes in Build"
3. Re-export Unity library

---

### **"No messages received from Kotlin"**

**Check in Unity Console:**
- You should see log messages when Kotlin sends messages
- If nothing appears, the GameObject name might be wrong

**Fix:**
1. Select `SceneLoader` GameObject in Hierarchy
2. Verify the name is **exactly** `"SceneLoader"` (no spaces, case-sensitive)
3. Verify it's in the first scene that loads

---

### **"Wrong scene loads"**

**Scene names are case-sensitive!**

**Fix:**
1. In Project window, note the **exact** scene filename
2. In Kotlin, use **exactly** that name (without `.unity`)

Example:
- Unity scene file: `Solar System Animated View 3D.unity`
- Kotlin code: `"Solar System Animated View 3D"` ✅
- NOT: `"solar system animated view 3d"` ❌

---

## 📱 **Kotlin Side (Already Done)**

Your Kotlin app is already set up correctly:

```kotlin
// Example: Load MBR Part View scene
val intent = Intent(this, UnityHolderActivity::class.java).apply {
    putExtra("sceneName", "MBR Part View 3D")
}
UnityPlayer.UnitySendMessage("SceneLoader", "LoadSceneByName", "MBR Part View 3D")
startActivity(intent)
```

---

## ✅ **Verification Checklist**

Before exporting:

- [ ] SceneLoader GameObject exists in startup scene
- [ ] GameObject is named **exactly** `"SceneLoader"`
- [ ] SceneLoader.cs script is attached
- [ ] All 5 target scenes are in Build Settings
- [ ] Startup scene is at index 0 in Build Settings
- [ ] Scene names in Kotlin match Unity exactly
- [ ] Console shows "SceneLoader initialized" in Play Mode

---

## 🎯 **Expected Behavior**

**When working correctly:**

1. Kotlin button pressed → `UnitySendMessage("SceneLoader", "LoadSceneByName", "MBR Part View 3D")`
2. Unity receives message → `LoadSceneByName("MBR Part View 3D")` called
3. Unity Console shows: `📩 SceneLoader: Received scene load request from Kotlin: 'MBR Part View 3D'`
4. Unity Console shows: `✅ SceneLoader: Loading scene 'MBR Part View 3D'...`
5. Scene loads successfully

---

That's it! Once you complete these steps and re-export, your Kotlin buttons will load the correct Unity scenes. 🚀

