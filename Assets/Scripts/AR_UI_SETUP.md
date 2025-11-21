# 🎮 AR UI Setup - Zoom Controls

Quick guide to set up zoom in/out buttons for AR scale control.

---

## 🎯 Quick Setup (You Already Have Buttons!)

### Step 1: Create UI Buttons (if you haven't)

1. **Right-click in Hierarchy** → UI → **Canvas** (if you don't have one)

2. **Create 3 Buttons:**
   - Right-click Canvas → UI → Button → Rename to "Zoom In Button"
   - Right-click Canvas → UI → Button → Rename to "Zoom Out Button"
   - Right-click Canvas → UI → Button → Rename to "Reset Scale Button"

3. **Position Buttons:**
   - **Zoom In**: Top-right corner, below any other UI
   - **Zoom Out**: Below Zoom In button
   - **Reset**: Bottom-right corner

4. **Change Button Text:**
   - Expand each button in Hierarchy
   - Select **Text (TMP)** child
   - Change text to: "+" (Zoom In), "-" (Zoom Out), "Reset"

---

### Step 2: Add AR Scale UI Script

1. **Create Controller GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Rename to "AR Scale UI Controller"

2. **Add Script:**
   - Select "AR Scale UI Controller"
   - Add Component → **AR Scale UI**

3. **Configure in Inspector:**
   - **AR Manager**: Drag your "AR Manager" GameObject (the one with ARSolarSystemManager script)
   - **Zoom In Button**: Drag your "Zoom In Button" GameObject
   - **Zoom Out Button**: Drag your "Zoom Out Button" GameObject
   - **Reset Button**: Drag your "Reset Scale Button" GameObject
   - **Min Scale**: `100`
   - **Max Scale**: `10000`
   - **Scale Step**: `1.2` (20% change per click)

4. **Optional - Add Text Display:**
   - Create UI → Text - TextMeshPro
   - Position it near buttons
   - Drag it to **Scale Text** field in AR Scale UI
   - This will show current scale (e.g., "Scale: 1000x")

---

## 🎨 Button Styling (Optional)

Make buttons look better:

1. **Select Button** → Inspector → **Image** component
2. Change **Color** to something visible (white, cyan, etc.)
3. Adjust **Width** and **Height** (e.g., 80x80 for +/- buttons)

For Text:
1. Select **Text (TMP)** child
2. Font Size: **36** or larger
3. Color: **White** or contrasting color
4. Style: **Bold**

---

## 🎮 How to Use in AR

Once set up:

1. **Launch AR scene** on device
2. Point camera at floor/table
3. Solar system appears
4. **Tap "+" button** → Zooms IN (gets closer)
5. **Tap "-" button** → Zooms OUT (gets farther)
6. **Tap "Reset"** → Returns to 1000x scale

---

## 📊 Scale Guide

| Button | Action | Scale Change | Result |
|--------|--------|--------------|--------|
| **+** (Zoom In) | Divide by 1.2 | 1000 → 833 → 694 | Get closer to planets |
| **-** (Zoom Out) | Multiply by 1.2 | 1000 → 1200 → 1440 | See more of system |
| **Reset** | Set to 1000 | Any → 1000 | Default view |

**Example Scales:**
- **100x** = Very close (single planet fills view)
- **500x** = Close (inner planets visible)
- **1000x** = Default (good balance)
- **2000x** = Far (most planets visible)
- **5000x** = Very far (entire system)

---

## 🔧 Alternative: Simple Setup Without UI Buttons

If you want controls without creating buttons:

### Option 1: Keyboard Shortcuts (Testing in Editor)

Add this to `ARScaleUI.cs` in `Update()` method:
```csharp
void Update()
{
    // For testing in editor
    if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
    {
        ZoomIn();
    }
    if (Input.GetKeyDown(KeyCode.Minus))
    {
        ZoomOut();
    }
    if (Input.GetKeyDown(KeyCode.R))
    {
        ResetScale();
    }
}
```

### Option 2: Touch Gestures

Two-finger pinch (already handled by XR Origin for camera zoom, but you can add custom):
- Pinch together = Zoom In
- Pinch apart = Zoom Out

---

## 🎯 What You Should See

### In Unity Editor:
- Buttons visible in Scene view
- Click buttons → Console shows scale changes
- OnGUI text shows current scale in top-left

### On Device:
- AR camera shows real world
- Solar system visible in AR space
- Buttons work to adjust scale
- Scale text updates in real-time

---

## 🐛 Troubleshooting

### Problem: Buttons don't do anything

**Solution:**
1. Check AR Scale UI script has AR Manager reference
2. Check buttons are dragged to correct fields
3. Check Console for "ARScaleUI: Zoom In/Out" messages
4. Make sure EventSystem exists (Canvas should auto-create it)

### Problem: Scale changes but nothing happens

**Solution:**
1. Check ARSolarSystemManager has XR Origin reference
2. Check Console for "Updated XR Origin scale" messages
3. Make sure XR Origin GameObject exists in scene

### Problem: Can't see buttons in AR

**Solution:**
1. Canvas → Render Mode: Screen Space - Overlay (default)
2. Buttons should appear on top of AR camera view
3. Check button colors are visible (not transparent)

### Problem: Scale text not showing

**Solution:**
- OnGUI text always shows (top-left, white text)
- If using TextMeshPro, make sure it's assigned in Inspector
- Check text color is not same as background

---

## ✅ Quick Checklist

- [ ] Canvas created
- [ ] 3 buttons created (+, -, Reset)
- [ ] AR Scale UI Controller created
- [ ] AR Scale UI script added
- [ ] AR Manager reference assigned
- [ ] Buttons dragged to script fields
- [ ] Tested in scene (click buttons, check console)
- [ ] Built to device and tested

---

## 🎨 Button Layout Suggestion

```
┌─────────────────────────────┐
│  AR Scale: 1000x            │  ← OnGUI text (auto)
│  (Tap +/- to adjust)        │
│                             │
│                        [+]  │  ← Zoom In (top-right)
│                        [-]  │  ← Zoom Out
│                             │
│        AR View              │
│     (Solar System)          │
│                             │
│                             │
│                   [Reset]   │  ← Reset (bottom-right)
└─────────────────────────────┘
```

---

## 🚀 Next Steps

Once working:
- Adjust `scaleStep` in Inspector (1.2 = 20% change, 1.5 = 50% change)
- Add haptic feedback when pressing buttons
- Add visual feedback (button glow/pulse)
- Add slider for precise control

---

**Need help?** Check Console for "ARScaleUI:" messages.

