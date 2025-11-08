# Planet Selector - Setup Guide

Click on planet names to change camera anchor/focus.

---

## 🎯 Quick Setup (2 Steps)

### **Step 1: Create PlanetSelector GameObject**

1. Right-click in **Hierarchy**
2. **Create Empty**
3. Rename to: **`PlanetSelector`**
4. **Add Component** → `PlanetSelector` script

---

### **Step 2: Assign Camera Controller**

1. Select `PlanetSelector` in Hierarchy
2. In Inspector:
   - **Camera Controller**: Drag the Main Camera (or GameObject with TouchCameraController)
   - **Show Planet Menu**: Keep checked ✓
   - **Menu Left Offset**: `20` (distance from left edge)
   - **Menu Top Offset**: `20` (distance from top edge)
   - **Planet Names**: Should show default list ✓

---

## 🎮 How It Works

### **When Playing:**

You'll see a menu on the **left side** of the screen:

```
Camera Anchor:
┌────────────┐
│ Sun        │  ← Click to focus on Sun
│ Mercury    │  ← Click to focus on Mercury
│ Venus      │
│ Earth      │
│ Mars       │
│ Jupiter    │
│ Saturn     │
│ Uranus     │
│ Neptune    │
└────────────┘
```

### **Active Planet:**
- The current anchor is shown in **cyan** color
- Other planets are shown in **white**
- Hover over a name to see it turn **yellow**

### **Clicking:**
- Click any planet name
- Camera instantly re-anchors to that planet
- You can still use touch/mouse controls to rotate and zoom

---

## 📺 Screen Layout

```
Left Side:                          Right Side:
┌─ Camera Anchor ─┐                 2x  ← Speed (if enabled)
│ Sun (cyan)       │
│ Mercury          │
│ Venus            │
│ Earth            │
│ ...              │
└──────────────────┘

                    Bottom-Center:
                    Date: 05-11-2025
                    Time: 20:00:00
                    G: 2.00E+00
```

---

## ⚙️ Configuration

### **Change Menu Position:**

In PlanetSelector Inspector:
- **Menu Left Offset**: Distance from left (default: 20)
- **Menu Top Offset**: Distance from top (default: 20)

Examples:
- Right side: `Screen.width - 150` (edit in script)
- Bottom: Increase Top Offset to move down

### **Customize Planet List:**

In Inspector, edit **Planet Names** array:
- Add planets: Click `+` button
- Remove: Click `-` button
- Rename: Edit each element

**Important**: Names must match (or partially match) your GameObject names!

### **Hide/Show Menu:**

- In Inspector: Uncheck **"Show Planet Menu"**
- Or call from script: `planetSelector.showPlanetMenu = false;`

---

## 🔧 Planet Name Matching

The script tries to find planets using multiple methods:

1. **Exact match**: "Sun" finds GameObject named "Sun"
2. **With suffix**: "Sun" finds "Sun Sphere"
3. **Case-insensitive**: "earth" finds "Earth"
4. **Partial match**: Searches Celestial-tagged objects
5. **Fallback**: Searches all GameObjects

### **Supported Name Variations:**
- "Sun" → finds "Sun", "Sun Sphere", "SunSphere"
- "Earth" → finds "Earth", "Earth_Planet", "earth"
- etc.

---

## 🎨 Customization

### **Change Button Colors:**

Edit `OnGUI()` in PlanetSelector.cs:

```csharp
// Normal state
buttonStyle.normal.textColor = Color.white;

// Hover state
buttonStyle.hover.textColor = Color.yellow;

// Active/selected state
activeButtonStyle.normal.textColor = Color.cyan;
```

### **Change Button Size:**

Edit in `OnGUI()`:

```csharp
float buttonWidth = 120f;   // Change width
float buttonHeight = 30f;   // Change height
float spacing = 5f;         // Space between buttons
```

### **Change Font Size:**

```csharp
buttonStyle.fontSize = 16;  // Change to 14, 18, 20, etc.
```

---

## 🐛 Troubleshooting

### Menu doesn't appear
- Check "Show Planet Menu" is enabled
- Press Play to see it (doesn't show in Edit mode)

### Clicking doesn't change anchor
- Check Camera Controller is assigned in Inspector
- Check Console for error messages
- Verify TouchCameraController script is on camera

### Planet not found when clicked
- Check planet GameObject name matches the button text
- Check planet has "Celestial" tag (helps with searching)
- Check Console for warning messages showing which name couldn't be found

### Camera doesn't move to planet
- TouchCameraController must have `SetTarget()` method public
- Check that the planet GameObject exists in scene

---

## 💡 Tips

### **Initial Anchor:**
The script starts with "Sun" as default. To change:

Edit in PlanetSelector.cs:
```csharp
private string currentAnchor = "Earth"; // Start focused on Earth
```

### **Add More Celestial Bodies:**
Add moons, asteroids, etc. to the list:

In Inspector, expand **Planet Names** and add:
- "Moon"
- "Phobos"
- "Deimos"
- etc.

### **Keyboard Shortcuts (Optional):**
Add to PlanetSelector.cs `Update()`:

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.Alpha1)) SetCameraAnchor("Sun");
    if (Input.GetKeyDown(KeyCode.Alpha2)) SetCameraAnchor("Mercury");
    // etc.
}
```

---

## ✅ Complete Checklist

- [ ] PlanetSelector GameObject created
- [ ] PlanetSelector script attached
- [ ] Camera Controller assigned
- [ ] Planet names match your GameObjects
- [ ] Pressed Play
- [ ] Menu appears on left side
- [ ] Clicking planet names changes camera focus ✓

---

## 🎯 Advanced Usage

### **From Other Scripts:**

```csharp
PlanetSelector selector = FindFirstObjectByType<PlanetSelector>();

// Set camera to specific planet
selector.SetAnchor("Mars");

// Get current anchor
string current = selector.GetCurrentAnchor();
Debug.Log($"Currently focused on: {current}");
```

### **Combine with Speed Control:**

You now have:
- **Left**: Planet selection menu
- **Right**: Speed multiplier (if TimeSpeedController active)
- **Bottom**: Date, Time, G value

Perfect for a complete solar system explorer! 🚀

---

## 🌟 Summary

**Simple 2-step setup:**
1. Create PlanetSelector GameObject + script
2. Assign Camera Controller

**Click planet names to change focus** - that's it! 🎉

