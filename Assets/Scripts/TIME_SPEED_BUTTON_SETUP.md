# Time Speed Controller - UI Setup Guide

This guide will help you set up the time speed button and display.

## ✅ Scripts Created:
1. **SolarSystem.cs** - Updated (G is now public, default: 4e-10)
2. **TimeSpeedController.cs** - New script for controlling time speed

---

## 🎨 UI Setup Steps

### **Step 1: Create Canvas (if you don't have one)**

1. Right-click in **Hierarchy**
2. UI → **Canvas**
3. Canvas will auto-create an **EventSystem** (needed for buttons)

---

### **Step 2: Create Speed Button**

1. Right-click on **Canvas** in Hierarchy
2. UI → **Button**
3. Rename it to: **`SpeedButton`**

#### **Configure SpeedButton:**
In Inspector:
- **Rect Transform**:
  - Anchor Preset: **Top-Right** (click the square icon, hold Alt+Shift, click top-right)
  - Pos X: `-140`
  - Pos Y: `-50`
  - Width: `120`
  - Height: `50`

- **Button (Script)**:
  - Leave default for now (we'll connect it via script)

- **Image**:
  - Color: Choose your preferred color (e.g., semi-transparent blue)

#### **Configure Button Text (child of SpeedButton):**
1. Expand **SpeedButton** in Hierarchy
2. Select **Text** child object
3. In Inspector:
   - **Text**: "⏱ Speed"
   - **Font Size**: 18
   - **Alignment**: Center/Middle
   - **Color**: White

---

### **Step 3: Create Speed Display Text**

1. Right-click on **Canvas** in Hierarchy
2. UI → **Text**
3. Rename it to: **`SpeedText`**

#### **Configure SpeedText:**
In Inspector:
- **Rect Transform**:
  - Anchor Preset: **Top-Right**
  - Pos X: `-140`
  - Pos Y: `-110` (below button)
  - Width: `200`
  - Height: `40`

- **Text**:
  - **Text**: "Real Time" (will be updated by script)
  - **Font Size**: 16
  - **Font Style**: Bold
  - **Alignment**: Center/Middle
  - **Color**: White

- **Shadow (Optional - for better readability)**:
  - Add Component → **Shadow**
  - Effect Distance: X: 1, Y: -1
  - Color: Black with some transparency

---

### **Step 4: Create TimeSpeedController GameObject**

1. Right-click in **Hierarchy**
2. Create Empty
3. Rename it to: **`TimeSpeedController`**
4. Add Component → **TimeSpeedController** script

#### **Configure TimeSpeedController:**
In Inspector:

**References Section:**
1. **Orbit Script**: 
   - Drag the GameObject that has the **Orbit** script (usually called "Orbit Manager" or similar)
   
2. **Speed Button**:
   - Drag the **SpeedButton** from Hierarchy
   
3. **Speed Text**:
   - Drag the **SpeedText** from Hierarchy

**Speed Settings Section:**
- **G Values**: Should show `[4E-10, 1, 10, 20, 50]`
  - These are the G values it will cycle through
  - You can add/remove values here if desired

---

## 🎮 Testing

### **Press Play:**
1. You should see the button in top-right corner
2. Below it should show "Real Time"
3. Click the button to cycle through speeds:

| Click | G Value | Display |
|-------|---------|---------|
| 1st (default) | 4e-10 | "Real Time" |
| 2nd | 1 | "Sped Up x2.50E+12" |
| 3rd | 10 | "Sped Up x2.50E+13" |
| 4th | 20 | "Sped Up x5.00E+13" |
| 5th | 50 | "Sped Up x1.25E+14" |
| 6th | 4e-10 | "Real Time" (cycles back) |

### **Check Console:**
You should see messages like:
```
TimeSpeedController: Found Orbit script
TimeSpeedController: Button listener added
TimeSpeedController: Set G to 4.00E-10
TimeSpeedController: Cycled to speed index 1
```

---

## 🎨 Customization Options

### **Change Button Position:**
Edit the **SpeedButton** Rect Transform:
- Top-Left: Anchor top-left, set Pos X: 140, Pos Y: -50
- Bottom-Right: Anchor bottom-right, set Pos X: -140, Pos Y: 50
- Center: Anchor center, set Pos X: 0, Pos Y: 0

### **Change Speed Values:**
In **TimeSpeedController** Inspector, modify the **G Values** array:
- Add more speeds: Click `+` icon
- Remove speeds: Click `-` icon
- Change values: Click on each element

### **Example Speed Presets:**

#### **Slow Observation:**
```
[4e-10, 4e-9, 4e-8, 1, 10]
```
- Real Time (1000x)
- 10,000x
- 100,000x
- 2.5 trillion x
- 25 trillion x

#### **Quick Preview:**
```
[1, 10, 50, 100, 500]
```
All very fast for quick testing

### **Change Display Format:**
Edit `UpdateSpeedText()` in TimeSpeedController.cs to customize:

```csharp
// Show as regular number instead of scientific notation:
speedText.text = $"Sped Up x{speedMultiplier:F0}";

// Show with comma separators:
speedText.text = $"Sped Up x{speedMultiplier:N0}";

// Add icons:
speedText.text = $"⚡ x{speedMultiplier:E1}";
```

---

## 🐛 Troubleshooting

### Issue: Button doesn't appear
**Solution:**
- Check Canvas is in Scene
- Check EventSystem exists (auto-created with Canvas)
- Check Canvas Render Mode is "Screen Space - Overlay"

### Issue: Button visible but not clickable
**Solution:**
- Make sure EventSystem exists in Hierarchy
- Check Button has **Button (Script)** component
- Check Canvas has **GraphicRaycaster** component

### Issue: "Could not find Orbit script"
**Solution:**
- Manually drag the GameObject with Orbit script to the Inspector field
- Make sure the Orbit script is actually in the scene

### Issue: Speed text not updating
**Solution:**
- Check SpeedText is assigned in TimeSpeedController Inspector
- Check SpeedText has a **Text** component
- Check Console for error messages

### Issue: G value doesn't change
**Solution:**
- Make sure G is public in SolarSystem.cs (should be after update)
- Check Console for "Set G to..." messages
- Verify Orbit Script is assigned in TimeSpeedController

---

## 📊 Understanding the Speeds

With **Sun Mass = 333,000**:

| G Value | Speed Factor | Earth Orbit Time |
|---------|--------------|------------------|
| 4e-13 | 1x (true real-time) | 365.25 days |
| 4e-10 | 1,000x | 8.76 hours |
| 1 | 2.5 trillion x | 0.01 seconds |
| 10 | 25 trillion x | 0.001 seconds |
| 20 | 50 trillion x | 0.0005 seconds |
| 50 | 125 trillion x | 0.0002 seconds |

The default "Real Time" (4e-10) is actually 1000x faster than true real-time, making Earth orbit in ~9 hours - perfect for observation!

---

## ✅ Final Checklist

- [ ] Canvas exists in scene
- [ ] SpeedButton created and positioned
- [ ] SpeedText created and positioned
- [ ] TimeSpeedController GameObject created
- [ ] TimeSpeedController script attached
- [ ] All three references assigned (Orbit, Button, Text)
- [ ] Button clicks and changes speed
- [ ] Text updates to show speed
- [ ] G value in SolarSystem.cs is public

**You're all set!** 🎉

