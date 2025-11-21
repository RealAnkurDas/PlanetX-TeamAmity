# 🚀 MBR Spacecraft AR Viewer Setup

Setup guide for viewing the MBR Explorer spacecraft in AR at human scale.

---

## 🎯 Goal

View the detailed MBR Explorer spacecraft model in AR at comfortable viewing size (like examining a physical model on a table).

---

## 📋 Quick Setup

### Step 1: Open Your MBR AR Scene

Open `Assets/Scenes/MBR Part View AR.unity`

---

### Step 2: Add AR Spacecraft Viewer

1. **Create Manager GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Rename to **"AR Spacecraft Manager"**

2. **Add Component:**
   - Select "AR Spacecraft Manager"
   - Add Component → **AR Spacecraft Viewer**

3. **Configure in Inspector:**

   **Spacecraft:**
   - **Spacecraft Model**: Drag **"MBR FINAL - Copy"** GameObject here
   - **Spacecraft Scale**: `0.5` (50cm size - adjust to taste)
   
   **AR Setup:**
   - **XR Origin**: Should auto-find (or drag XR Origin)
   - **AR Camera**: Should auto-find (or drag Main Camera from XR Origin)
   - **Placement Distance**: `1.5` (1.5 meters from camera)
   - **Height Above Floor**: `0.5` (hover height)
   
   **Camera Settings:**
   - **Near Clip Plane**: `0.01`
   - **Far Clip Plane**: `100`

---

### Step 3: Verify XR Origin Setup

1. **Find XR Origin** in Hierarchy

2. **XR Origin Settings:**
   - Transform → Scale: `1, 1, 1` (no scaling needed!)
   - Position: `0, 0, 0`

**Note:** Unlike solar system AR, we DON'T scale XR Origin here because:
- Spacecraft is at human scale (meters)
- No need for 1000x scaling
- View it like a physical model

---

### Step 4: Verify AR Session

1. Make sure **AR Session** GameObject exists
2. Default settings are fine

---

## 🎮 How It Works

### When You Launch:

1. Point device at floor/table
2. AR detects surface
3. Spacecraft appears **1.5 meters in front of you**
4. Hovers **0.5 meters above floor**
5. Faces toward you

### Walk Around:

- Walk closer → See more detail
- Walk around → See from all sides
- Look up/down → See top/bottom
- Like examining a physical model!

---

## 📏 Size Guide

The **Spacecraft Scale** parameter controls how big the model appears:

| Scale | Approximate Size | Best For |
|-------|------------------|----------|
| 0.2 | 20cm (8 inches) | Desktop viewing |
| 0.5 | 50cm (20 inches) | Table-top model |
| 1.0 | 1 meter (3 feet) | Large room |
| 2.0 | 2 meters (6 feet) | Very large space |

**Recommended:** Start with `0.5` and adjust in Inspector.

---

## 🎨 Optional: Add Interaction

### Rotate Spacecraft:

Create a script to rotate the spacecraft when user swipes:

```csharp
void Update()
{
    if (Input.touchCount == 1)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Moved)
        {
            float rotationSpeed = 0.5f;
            spacecraftModel.transform.Rotate(
                Vector3.up,
                touch.deltaPosition.x * rotationSpeed,
                Space.World
            );
        }
    }
}
```

### Scale with Pinch Gesture:

Detect pinch to make spacecraft bigger/smaller dynamically.

---

## 🔧 Adjusting Placement

### Closer to Camera:
- **Placement Distance**: `1.0` (1 meter)

### Farther from Camera:
- **Placement Distance**: `2.5` (2.5 meters)

### Higher Above Floor:
- **Height Above Floor**: `1.0` (chest height)

### At Table Height:
- **Height Above Floor**: `0.3` (on table)

---

## 🌟 Expected Scene Setup

```
Hierarchy (MBR Part View AR scene):
├── AR Session
├── XR Origin
│   ├── Camera Offset
│   │   └── Main Camera
├── AR Spacecraft Manager
│   └── AR Spacecraft Viewer (script)
├── MBR FINAL - Copy (spacecraft model)
│   └── (all the spacecraft parts)
└── EventSystem (for UI if needed)
```

---

## ✅ Checklist

Setup:
- [ ] MBR Part View AR scene open
- [ ] AR Spacecraft Manager created
- [ ] AR Spacecraft Viewer component added
- [ ] Spacecraft model assigned
- [ ] XR Origin exists (scale 1,1,1)
- [ ] AR Session exists
- [ ] Camera settings configured

Test:
- [ ] Press Play in editor (will show warnings about AR)
- [ ] Check Console: "ARSpacecraftViewer: Found spacecraft model"
- [ ] Check Console: "Placed spacecraft at..."
- [ ] Build to device for real AR test

---

## 📱 On Device

### What You'll See:

1. **Point at floor** → AR detects surface
2. **Spacecraft appears** floating in front of you
3. **Walk around** → Inspect from all angles
4. **Get close** → See fine details
5. **Look from below** → See underside

### Perfect For:

- Educational demonstrations
- Design review
- Part inspection
- Showing off your spacecraft! 🚀

---

## 🐛 Troubleshooting

### Can't see spacecraft

**Solution:**
1. Check "MBR FINAL - Copy" is assigned
2. Check spacecraft scale (try 0.5 to 1.0)
3. Check placement distance (1.5m default)
4. Check Console for "Placed spacecraft" message

### Spacecraft too small

**Solution:**
- Increase **Spacecraft Scale** to `1.0` or higher

### Spacecraft too big

**Solution:**
- Decrease **Spacecraft Scale** to `0.3` or lower

### Spacecraft on floor (not floating)

**Solution:**
- Increase **Height Above Floor** to `0.8` or `1.0`

### Spacecraft too far away

**Solution:**
- Decrease **Placement Distance** to `1.0` or `0.8`

---

## 🎯 Recommended Settings

For inspecting spacecraft details:

```
Spacecraft Scale: 0.5
Placement Distance: 1.5
Height Above Floor: 0.5
```

This places a 50cm model 1.5m in front of you at waist height - perfect for detailed inspection!

---

**Ready!** Build to device and view your spacecraft in AR! 🛸✨

