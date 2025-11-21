# 🎬 Scene Menu Setup (Fast)

A simple scene selector menu for your landing scene.

---

## ⚡ **Quick Setup (2 Steps)**

### **Step 1: Add to Landing Scene**

1. Open `Solar System Animated View 3D` scene
2. In Hierarchy: Right-click → `Create Empty`
3. Name it `SceneMenu`
4. In Inspector: `Add Component` → `SceneMenu`
5. **Save scene** (Ctrl+S)

Done! ✅

---

### **Step 2: Test It**

1. Enter Play Mode
2. You'll see a centered menu with 5 scene buttons:
   - 🌍 Solar System 3D View
   - 📱 AR Solar System
   - 🛰️ MBR Explorer Parts
   - 🎯 GA Trajectory
   - 🚀 Custom Launch

3. Click any button to load that scene!

---

## 🎨 **Customize (Optional)**

Select the `SceneMenu` GameObject and edit in Inspector:

### **Menu Styling:**
- **Menu Width**: Default 400px
- **Button Height**: Default 60px
- **Padding**: Default 20px
- **Font Size**: Default 20

### **Scene Names:**
- These must **exactly match** your scene names in Build Settings
- Default scenes already configured

### **Display Names:**
- Friendly names shown on buttons (with emojis!)
- Can edit to remove emojis or change text

---

## 🔧 **Advanced Features**

### **Show/Hide Menu from Code:**

```csharp
SceneMenu menu = FindFirstObjectByType<SceneMenu>();
menu.HideMenu();  // Hide the menu
menu.ShowMenu();  // Show the menu
```

### **Load Scene from Kotlin:**

The menu also has a `LoadSceneByName` method that works with Kotlin:

```kotlin
UnityPlayer.UnitySendMessage("SceneMenu", "LoadSceneByName", "MBR Part View 3D")
```

---

## 🎯 **Scene Menu vs SceneLoader**

You now have **two ways** to load scenes:

### **SceneMenu** (New):
- ✅ Visual UI menu
- ✅ Great for landing page
- ✅ User-friendly buttons

### **SceneLoader** (Previous):
- ✅ Headless (no UI)
- ✅ Perfect for Kotlin communication
- ✅ Loads scenes silently

**Both can coexist!** Keep both GameObjects in your scene. The menu is for Unity users, SceneLoader is for Kotlin.

---

## 🚀 **Export for Android**

After adding the SceneMenu:

1. Verify all scenes in `File` → `Build Settings`
2. `File` → `Build Settings` → `Export`
3. Export to: `C:\Users\ankur\Planet X - AR\UnityExports\`

The menu will work in both Unity Editor and Android builds!

---

## 📱 **Behavior on Phone**

- Menu appears centered on screen
- Touch any button to load scene
- All buttons scaled for mobile
- Works in portrait and landscape

---

That's it! Fast, simple, and works everywhere. 🎉

