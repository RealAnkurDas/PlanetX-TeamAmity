# Time Speed Controller - Simple Setup Guide

No UI buttons needed! The speed control uses OnGUI display and keyboard input.

---

## ✅ Super Simple Setup (2 Steps!)

### **Step 1: Create the Controller**

1. Right-click in **Hierarchy**
2. **Create Empty**
3. Rename it to: **`TimeSpeedController`**
4. Click **Add Component**
5. Search for and add: **`TimeSpeedController`** script

### **Step 2: Assign the Orbit Script**

1. With `TimeSpeedController` selected in Hierarchy
2. In Inspector, find **"Orbit Script"** field
3. Either:
   - Click the circle → search for the GameObject with **Orbit** component
   - Or drag the GameObject with Orbit script from Hierarchy

That's it! ✅

---

## 🎮 How to Use

### **During Play:**

1. **Press Play** ▶️
2. You'll see in **top-right corner**:
   ```
   ⏱ Real Time
   (Press 'S' to change)
   ```

3. **Press 'S' key** to cycle through speeds:
   - Real Time
   - Fast
   - Faster
   - Fastest
   - (cycles back to Real Time)

---

## 📊 What You'll See

### **Top-Right Display:**
```
⏱ Real Time
(Press 'S' to change)
```

### **Bottom-Center Display (from SimulationTimeTracker):**
```
05-11-2025
20:00:00
```

Both displays work together to show current speed and date/time!

---

## ⚙️ Configuration Options

### **Hide the Speed Display:**

1. Select `TimeSpeedController` in Hierarchy
2. In Inspector, uncheck **"Show Speed Display"**
3. You can still press 'S' to change speed, but won't see the label

### **Change Speed Values:**

1. Select `TimeSpeedController` in Hierarchy
2. In Inspector, expand **"G Values"** array
3. Current values: `[4E-10, 1, 10, 20]`
4. Add more: Click `+` button
5. Remove: Click `-` button
6. Change: Click on each element to edit

---

## 🎯 Speed Breakdown

| Label | G Value | Actual Speed Factor |
|-------|---------|---------------------|
| **Real Time** | 4e-10 | 1,000x (watchable) |
| **Fast** | 1 | 2.5 trillion x |
| **Faster** | 10 | 25 trillion x |
| **Fastest** | 20 | 50 trillion x |

**Note:** "Real Time" is actually 1000x faster than true real-time, making Earth orbit in ~9 hours - perfect for observation!

---

## 🐛 Troubleshooting

### Issue: "Could not find Orbit script"
**Solution:**
- Make sure you assigned the Orbit script in Inspector
- The GameObject with Orbit component must be active in scene

### Issue: Speed display doesn't appear
**Solution:**
- Check "Show Speed Display" is enabled in Inspector
- Press Play to see it (doesn't show in Edit mode)

### Issue: Pressing 'S' doesn't work
**Solution:**
- Make sure the Game window has focus (click on it)
- Check Console for messages when pressing 'S'

### Issue: Speed doesn't actually change
**Solution:**
- Check that G value in Orbit script is public
- In SolarSystem.cs, line 8 should be: `public float G = 4e-10f;`
- Check Console for "Set G to..." messages

---

## ✅ Quick Checklist

- [ ] TimeSpeedController GameObject created
- [ ] TimeSpeedController script attached
- [ ] Orbit Script assigned in Inspector
- [ ] Pressed Play
- [ ] Speed display appears in top-right
- [ ] Pressing 'S' cycles through speeds
- [ ] Date/time advances correctly

---

## 🎨 Customizing Display Position

To change where the speed display appears, edit the `OnGUI()` method in TimeSpeedController.cs:

### **Current (Top-Right):**
```csharp
float xPosition = Screen.width - textSize.x - 20f;
float yPosition = 20f;
```

### **Top-Left:**
```csharp
float xPosition = 20f;
float yPosition = 20f;
```

### **Bottom-Right:**
```csharp
float xPosition = Screen.width - textSize.x - 20f;
float yPosition = Screen.height - textSize.y - 20f;
```

### **Bottom-Left:**
```csharp
float xPosition = 20f;
float yPosition = Screen.height - textSize.y - 20f;
```

---

## 🚀 Advanced: Touch/Mobile Support

To add touch button support for mobile, you could add:

```csharp
void OnGUI()
{
    // ... existing display code ...
    
    // Add clickable button
    if (GUI.Button(new Rect(xPosition, yPosition + 50, 100, 40), "Change Speed"))
    {
        CycleSpeed();
    }
}
```

---

## 📝 Summary

**No UI setup needed!** Just:
1. Add TimeSpeedController GameObject
2. Assign Orbit script
3. Press 'S' to change speed

Simple and clean! 🎉

