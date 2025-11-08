# Speed Button - Final Setup Guide

Simple guide to set up the speed control button.

---

## 🎯 Quick Setup (3 Steps)

### **Step 1: Create the Speed Button**

1. Right-click on **Canvas** in Hierarchy (create Canvas first if you don't have one)
2. **UI** → **Button**
3. Rename it to: **`SpeedButton`**

#### **Position the button:**
- Select `SpeedButton`
- In **Rect Transform**:
  - Click **Anchor Preset** (hold Alt+Shift, click **Top-Right**)
  - Pos X: `-140`
  - Pos Y: `-50`
  - Width: `120`
  - Height: `50`

#### **Customize button text:**
- Expand `SpeedButton` → select **Text** child
- Change text to: **"Speed"** or **"⏱ Speed"**
- Font Size: `18`
- Color: White

---

### **Step 2: Create TimeSpeedController**

1. Right-click in Hierarchy → **Create Empty**
2. Rename to: **`TimeSpeedController`**
3. Add Component → **`TimeSpeedController`** script

---

### **Step 3: Connect Everything**

Select `TimeSpeedController` in Hierarchy, then in Inspector:

1. **Orbit Script**: Drag the GameObject with Orbit component
2. **Speed Button**: Drag the `SpeedButton` from Hierarchy
3. **Show Speed Display**: Keep checked ✓
4. **G Values**: Should show `[2, 4, 8, 16, 32]` ✓

---

## 🎮 How It Works

### **When Playing:**

1. **Initial State:**
   - G = 2
   - No speed text shown

2. **Click Button Once:**
   - G = 4
   - Shows **"2x"** in top-right corner

3. **Click Again:**
   - G = 8
   - Shows **"4x"**

4. **Click Again:**
   - G = 16
   - Shows **"8x"**

5. **Click Again:**
   - G = 32
   - Shows **"16x"**

6. **Click Again:**
   - Cycles back to G = 2
   - Hides speed text

---

## 📺 What You'll See

### **Screen Layout:**

```
Top-Right Corner:
┌─────────────┐
│    2x       │  ← Speed multiplier (only when active)
└─────────────┘

[Speed Button]  ← Click to cycle


Bottom-Center:
05-11-2025      ← Date
20:00:00        ← Time
```

---

## ✅ Complete Checklist

- [ ] Canvas exists in scene
- [ ] SpeedButton created and positioned
- [ ] TimeSpeedController GameObject created
- [ ] TimeSpeedController script attached
- [ ] Orbit Script assigned in Inspector
- [ ] Speed Button assigned in Inspector
- [ ] G Values array shows [2, 4, 8, 16, 32]
- [ ] Pressed Play and clicked button - speed changes! ✓

---

## 🎨 Optional Customization

### **Change Button Position:**

Edit SpeedButton's Rect Transform:
- **Top-Left**: Anchor top-left, Pos X: 20, Y: -50
- **Bottom-Right**: Anchor bottom-right, Pos X: -140, Y: 50

### **Change Button Appearance:**

On SpeedButton:
- **Image → Color**: Choose your color
- **Add Component → Shadow**: For depth effect

### **Change Speed Display Position:**

Edit `OnGUI()` in TimeSpeedController.cs:

```csharp
// Top-Left instead of Top-Right
float xPosition = 20f;
float yPosition = 20f;
```

---

## 🐛 Troubleshooting

### Button doesn't appear
- Check Canvas exists and is set to "Screen Space - Overlay"
- Check EventSystem exists (auto-created with Canvas)

### Button visible but not clickable
- Make sure Button has **Button (Script)** component
- Check EventSystem is in scene

### Clicking button does nothing
- Check SpeedButton is assigned in TimeSpeedController Inspector
- Check Console for "Button listener added" message
- Make sure Orbit Script is also assigned

### Speed text doesn't show
- This is normal for first state (G=2)
- Click button once to see "2x" appear
- Check "Show Speed Display" is enabled in Inspector

### G value doesn't change
- Check Orbit Script is assigned
- Check G is public in SolarSystem.cs
- Check Console for "Set G to..." messages

---

## 📊 Speed Reference

| Button Clicks | G Value | Display | Speed Factor |
|---------------|---------|---------|--------------|
| 0 (start) | 2 | (none) | 5 trillion x |
| 1 | 4 | 2x | 10 trillion x |
| 2 | 8 | 4x | 20 trillion x |
| 3 | 16 | 8x | 40 trillion x |
| 4 | 32 | 16x | 80 trillion x |
| 5 (cycle) | 2 | (none) | Back to start |

---

## 🎉 You're Done!

Your speed control is ready. Just:
1. Click the button to speed up
2. Watch the multiplier appear in top-right
3. See the date/time advance faster at bottom-center

Enjoy your solar system simulation! 🌍🪐🌟

