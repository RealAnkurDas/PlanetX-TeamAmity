# 🚀 Make MBR Spacecraft Interactable in AR

Step-by-step guide to make "MBR FINAL - Copy" work like the other spawnable objects (Cube, Cylinder, etc.) in your MBR Part View AR scene.

---

## 🎯 What You'll Get

Your MBR spacecraft will be able to:
- ✅ **Tap to place** on AR surfaces (floor, table)
- ✅ **Drag to move** with one finger
- ✅ **Pinch to scale** (make bigger/smaller)
- ✅ **Twist to rotate** (two-finger rotation)
- ✅ **Delete** with delete button
- ✅ **Spawn from button** (like other shapes)

---

## 📋 Setup Steps

### Step 1: Prepare MBR Spacecraft GameObject

1. **Find** "MBR FINAL - Copy" in Hierarchy of **MBR Part View AR** scene

2. **Add Required Components:**

   **A. Add Rigidbody:**
   - Select "MBR FINAL - Copy"
   - Add Component → **Rigidbody**
   - Configure:
     - Mass: `1`
     - Drag: `0`
     - Angular Drag: `0.05`
     - **Use Gravity**: ❌ **OFF**
     - **Is Kinematic**: ✅ **ON** (important!)
     - Interpolate: None
     - Collision Detection: Discrete

   **B. Add XR Grab Interactable:**
   - Add Component → Search **"XR Grab Interactable"**
   - This is the KEY component that makes it interactable!
   - Leave all settings as default

   **C. Add Collider (if not present):**
   - If no collider exists, add one:
     - Add Component → **Box Collider** (or Mesh Collider)
     - Adjust size to encompass the entire spacecraft

---

### Step 2: Create Prefab from MBR Spacecraft

1. **In Project window**, create folder for your prefabs:
   - Navigate to `Assets/`
   - Right-click → Create → **Folder**
   - Name it: `AR Prefabs`

2. **Create Prefab:**
   - In Hierarchy, **drag "MBR FINAL - Copy"** into the `Assets/AR Prefabs/` folder
   - This creates a prefab
   - Rename the prefab to: **"MBR Spacecraft"**

3. **The original stays in scene** - that's fine, you can delete it or disable it later

---

### Step 3: Add MBR Spacecraft to Object Spawner

1. **Find "Object Spawner"** GameObject in Hierarchy

2. **In Inspector**, find **Object Spawner (Script)** component

3. **Add to Object Prefabs list:**
   - Find "**Object Prefabs**" (it shows 7 elements currently)
   - Click the **+** button to add slot 8
   - **Drag your "MBR Spacecraft" prefab** from Project window into the new slot

4. ✅ **Done!** MBR is now in the spawner list

---

### Step 4: Create Spawn Button for MBR (Optional)

If you want a button to spawn MBR (like the Cube, Cylinder buttons):

1. **Duplicate existing button:**
   - Find "Button (Cube)" in Hierarchy
   - Duplicate it (Ctrl+D)
   - Rename to: "Button (MBR Spacecraft)"

2. **Change Button Icon:**
   - Expand button → Find "Icon" child
   - Change sprite/image to represent spacecraft (or use same)

3. **Wire up to spawner:**
   - Select your button
   - Find **Button (Script)** component
   - In **OnClick()** event:
     - Should already have "Object Spawner" → `SetSpawnOptionIndex`
     - Change the index to **7** (since MBR is the 8th prefab, index 7)

4. **Position button** in the object menu with other buttons

---

## 🎮 How to Use

### Spawning MBR Spacecraft:

1. **Launch AR app** on device
2. **Point at floor** → AR detects surface
3. **Tap "Create" button** (bottom-right)
4. **Tap your MBR button** (or it auto-selects if it's the first)
5. **Tap on floor** where you want to place it
6. 🚀 **MBR Spacecraft appears!**

### Interacting with MBR:

Once spawned:

| Gesture | Action |
|---------|--------|
| **Tap object** | Select it (shows selection box) |
| **Drag (1 finger)** | Move on surface |
| **Pinch (2 fingers)** | Scale up/down |
| **Twist (2 fingers)** | Rotate |
| **Tap Delete button** | Delete selected object |

---

## 🔧 Adjusting MBR Size

The MBR spacecraft might be too big or small when spawned. To adjust:

### Option 1: Scale the Prefab

1. **In Project window**, find **"MBR Spacecraft"** prefab
2. **Double-click** to open in Prefab mode
3. **Select root** of prefab
4. **Transform → Scale:**
   - Current scale × 0.1 = **10% size** (smaller)
   - Current scale × 0.5 = **50% size**
   - Current scale × 2.0 = **2x size** (bigger)
5. **Save prefab** (Ctrl+S)

### Option 2: Adjust in Scene Before Creating Prefab

1. **Delete the prefab** you created
2. **In Hierarchy**, select "MBR FINAL - Copy"
3. **Change Transform → Scale** to desired size
4. **Create prefab again** (drag to Assets/AR Prefabs/)

**Recommended Size for AR:**
- If spacecraft is currently 1:1 scale (actual size), try `Scale: 0.01, 0.01, 0.01` (1% size)
- This makes it table-top model size

---

## ✅ Checklist

Preparation:
- [ ] MBR FINAL - Copy found in scene
- [ ] Rigidbody added (kinematic, no gravity)
- [ ] XR Grab Interactable added
- [ ] Collider present
- [ ] Scale adjusted for AR viewing

Prefab:
- [ ] AR Prefabs folder created
- [ ] MBR Spacecraft prefab created
- [ ] Prefab has all components

Integration:
- [ ] Object Spawner found
- [ ] MBR prefab added to Object Prefabs list (slot 8)
- [ ] Button created (optional)
- [ ] Button wired to spawn index 7

Testing:
- [ ] Build to device
- [ ] Tap Create button
- [ ] Tap to place MBR
- [ ] Test drag (moves) ✓
- [ ] Test pinch (scales) ✓
- [ ] Test twist (rotates) ✓
- [ ] Test delete ✓

---

## 🐛 Troubleshooting

### Can't place MBR (nothing happens when tapping)

**Solution:**
1. Check XR Grab Interactable is on prefab
2. Check Rigidbody is kinematic
3. Check Object Spawner has prefab in list
4. Check AR planes are being detected (green grid on floor)

### MBR is too big/small

**Solution:**
- Edit prefab scale (see "Adjusting MBR Size" above)
- Recommended: Try 0.1, 0.1, 0.1 first

### Can't drag/move MBR after placing

**Solution:**
1. Check XR Grab Interactable component is enabled
2. Check Rigidbody is kinematic (not using gravity)
3. Tap object first to select it, then drag

### MBR doesn't have selection box

**Solution:**
- The selection box is part of the AR system
- Make sure your scene has the AR UI elements
- Check that XR Grab Interactable is properly configured

### Can't scale with pinch

**Solution:**
- Make sure you're using 2 fingers
- Select object first (tap it)
- Then pinch in/out

---

## 🎨 Expected Behavior

### In AR:

```
1. Launch app → Point at floor
2. Tap "Create" → Menu appears
3. Tap "MBR Spacecraft" button → Reticle appears
4. Tap on floor → MBR appears there
5. Tap MBR → Selection box appears
6. Drag → MBR moves along floor
7. Pinch → MBR scales bigger/smaller
8. Twist → MBR rotates
9. Tap Delete → MBR disappears
10. Spawn another → Can have multiple!
```

---

## 📊 Component Comparison

| Component | Cube Prefab | MBR Spacecraft Prefab |
|-----------|-------------|----------------------|
| Transform | ✓ | ✓ |
| Rigidbody (Kinematic) | ✓ | ✓ (add this) |
| XR Grab Interactable | ✓ | ✓ (add this) |
| Collider | ✓ | ✓ (should have) |
| Visual Model | Cube mesh | MBR FBX model |

---

## 🚀 Advanced: Custom Spawn Scale

If you want MBR to always spawn at a specific size:

1. **Select Object Spawner** in Hierarchy
2. **Add script** (create new C#):

```csharp
using UnityEngine;

public class MBRSpawnScaler : MonoBehaviour
{
    void OnEnable()
    {
        // Listen for object spawn events
        var spawner = GetComponent<ObjectSpawner>();
        if (spawner != null)
        {
            // Hook into spawn callback (you'd need to modify ObjectSpawner)
            // Or use Update() to check for new MBR objects
        }
    }
    
    void Update()
    {
        // Find newly spawned MBR objects and scale them
        GameObject[] mbrs = GameObject.FindGameObjectsWithTag("Spacecraft");
        foreach (var mbr in mbrs)
        {
            if (!mbr.GetComponent<ScaledMarker>())
            {
                mbr.transform.localScale = Vector3.one * 0.1f; // 10% size
                mbr.AddComponent<ScaledMarker>(); // Mark as scaled
            }
        }
    }
}

// Marker component
public class ScaledMarker : MonoBehaviour { }
```

---

## ✅ Summary

1. Add Rigidbody + XR Grab Interactable to MBR
2. Create prefab
3. Add prefab to Object Spawner list
4. Spawn and interact!

**That's it!** Your MBR spacecraft will work exactly like the Cube, Cylinder, and other shapes. 🛸✨

