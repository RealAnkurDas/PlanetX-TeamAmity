# 🎮 Menu Scene Setup - Complete Guide

Create a professional menu scene with buttons for all your scenes.

---

## 📋 **Quick Setup Steps**

### **Step 1: Create Menu Scene**

1. `File` → `New Scene`
2. `File` → `Save As` → Name it `Menu Scene`
3. Save in `Assets/Scenes/`

---

### **Step 2: Create Canvas**

1. Right-click Hierarchy → `UI` → `Canvas`
2. Select `Canvas`
3. In Inspector:
   - **Canvas Scaler** → **UI Scale Mode** → `Scale With Screen Size`
   - **Reference Resolution**: `1920 x 1080`

---

### **Step 3: Add Background (Optional)**

1. Right-click `Canvas` → `UI` → `Image`
2. Rename to `Background`
3. In Inspector:
   - Click Anchor Presets (top-left square)
   - Hold **Alt + Shift**
   - Click **bottom-right** option (stretch both)
   - **Color**: Dark color like `#1A1A2E` or `#0F0F1E`

---

### **Step 4: Add Title**

1. Right-click `Canvas` → `UI` → `TextMeshPro - Text`
   - If prompted to import TMP Essentials, click **Import**
2. Rename to `Title`
3. Set text: `SCENE SELECTOR` or `MISSION CONTROL`
4. In Inspector:
   - **Font Size**: `60`
   - **Alignment**: Center (both horizontal and vertical)
   - **Color**: White or Cyan
   - **Rect Transform**:
     - Anchor: Top-center
     - Pos X: `0`
     - Pos Y: `-100`
     - Width: `1000`
     - Height: `100`

---

### **Step 5: Create First Button**

1. Right-click `Canvas` → `UI` → `Button - TextMeshPro`
2. Rename to `Button_SolarSystem3D`
3. In Inspector:
   - **Rect Transform**:
     - Width: `500`
     - Height: `80`
     - Pos X: `0`
     - Pos Y: `150`

4. Expand the button in Hierarchy, select `Text (TMP)` child
5. Set text: `🌍 Solar System 3D View`
6. In Inspector:
   - **Font Size**: `28`
   - **Alignment**: Center
   - **Color**: White

7. Select the button again (parent)
8. In Inspector: `Add Component` → `MenuSceneButton`
9. In **Menu Scene Button** component:
   - **Scene Name**: `Solar System Animated View 3D`

---

### **Step 6: Duplicate for Other Buttons**

1. Select `Button_SolarSystem3D` in Hierarchy
2. Press **Ctrl+D** to duplicate (or Cmd+D on Mac)
3. Rename to `Button_ARSolarSystem`
4. Change **Pos Y** to `50`
5. Change text to: `📱 AR Solar System`
6. In **Menu Scene Button**:
   - **Scene Name**: `Solar System Animated View AR`

**Repeat for remaining scenes:**

| Button Name | Pos Y | Button Text | Scene Name |
|-------------|-------|-------------|------------|
| Button_SolarSystem3D | 150 | 🌍 Solar System 3D View | Solar System Animated View 3D |
| Button_ARSolarSystem | 50 | 📱 AR Solar System | Solar System Animated View AR |
| Button_MBRParts | -50 | 🛰️ MBR Explorer Parts | MBR Part View 3D |
| Button_GATrajectory | -150 | 🎯 GA Trajectory | Solar System Animated View 3D GA |
| Button_CustomLaunch | -250 | 🚀 Custom Launch | Solar System Animated View 3D User |

---

### **Step 7: Make Menu Scene First**

1. `File` → `Build Settings`
2. Drag `Menu Scene` to **index 0** (top of list)
3. Ensure all other scenes are also in the list:
   - Menu Scene (index 0) ← **Must be first!**
   - Solar System Animated View 3D
   - Solar System Animated View AR
   - MBR Part View 3D
   - Solar System Animated View 3D GA
   - Solar System Animated View 3D User
4. All scenes should be **checked** (enabled)

---

### **Step 8: Test**

1. Make sure you're in `Menu Scene`
2. Press **Play**
3. Click any button
4. Scene should load! ✅

---

## 🎨 **Styling Tips (Optional)**

### **Make Buttons Look Better:**

1. Select any button
2. In Inspector, find **Button** component
3. Under **Colors**:
   - **Normal Color**: `#2C3E50` (dark blue-gray)
   - **Highlighted Color**: `#3498DB` (bright blue)
   - **Pressed Color**: `#1ABC9C` (teal)
   - **Selected Color**: `#E74C3C` (red)

### **Add Icon/Logo:**

1. Right-click `Canvas` → `UI` → `Image`
2. Rename to `Logo`
3. Drag your logo texture to **Source Image**
4. Position at top-center above title

---

## 🚀 **Export for Android**

After creating the menu:

1. Verify `Menu Scene` is at **index 0** in Build Settings
2. Verify all other scenes are in the list
3. `File` → `Build Settings` → `Export`
4. Export to: `C:\Users\ankur\Planet X - AR\UnityExports\`

The menu will appear when the app starts on Android!

---

## 🔧 **Troubleshooting**

### **Button doesn't work:**
- Check if `MenuSceneButton` component is attached
- Verify **Scene Name** field is filled correctly
- Make sure scene is in Build Settings

### **Text looks blurry:**
- Select Canvas
- Canvas → Render Mode → **Screen Space - Camera**
- Drag Main Camera to **Render Camera** field

### **Buttons overlap on phone:**
- Select Canvas
- Canvas Scaler → **Match** slider → Set to `0.5` (balance)

---

## 📱 **Alternative: Use SceneMenu Script**

If you want a **quicker** setup without creating buttons manually:

1. Create `Menu Scene`
2. Create Empty GameObject named `SceneMenu`
3. Add Component → `SceneMenu` (the script already created)
4. Press Play - menu appears automatically!

**Pros:** Instant, no setup  
**Cons:** Can't customize button positions/colors in Inspector

---

## ✅ **Final Checklist**

- [ ] Menu Scene created
- [ ] Canvas created with proper scaler
- [ ] 5 buttons created with proper names
- [ ] Each button has `MenuSceneButton` script
- [ ] Each button's `Scene Name` is set correctly
- [ ] Menu Scene is at **index 0** in Build Settings
- [ ] All target scenes are in Build Settings
- [ ] Tested in Play Mode - buttons work

---

That's it! You now have a professional menu scene. 🎉

