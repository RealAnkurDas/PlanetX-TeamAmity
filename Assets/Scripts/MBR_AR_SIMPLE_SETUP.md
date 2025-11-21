# 🚀 MBR AR Simple Setup

Ultra-simple AR setup: Just tap to spawn MBR, automatically selected and ready to manipulate.

---

## 🎯 What You Get

1. **Tap anywhere** → MBR spacecraft spawns
2. **Automatically selected** → Ready to manipulate
3. **Drag** → Moves (stays upright)
4. **Pinch** → Scales up/down
5. **Twist** → Rotates
6. **Delete button** → Removes it
7. **Only MBR** → No other objects

---

## 🚀 Quick Setup

### Step 1: Remove Other Prefabs from Spawner

1. **Find "Object Spawner"** in Hierarchy

2. **In Inspector → Object Spawner component:**
   - Find **"Object Prefabs"** list (currently has 7+ items)
   - Click on each prefab slot EXCEPT your MBR one
   - Click **"-"** to remove them
   - **Keep ONLY the MBR Spacecraft prefab**

3. Result: Should show "Size: 1" with only MBR

---

### Step 2: Add Auto-Spawner

1. **Create GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Rename to **"MBR Auto Spawner"**

2. **Add Component:**
   - Add Component → **ARMBR Auto Spawner**

3. **Configure:**
   - **Spacecraft Prefab**: Drag your MBR prefab here
   - **Spawn Scale**: `0.1` (10% size - adjust as needed)
   - **Only One Spacecraft**: ✅ **Checked** (prevents duplicates)
   - **AR Raycast Manager**: Should auto-find
   - **AR Plane Manager**: Should auto-find

---

### Step 3: Fix Rotation Issue on Grab

1. **Edit MBR Prefab:**
   - In Project window, find your MBR prefab
   - Double-click to open in Prefab mode

2. **Select root GameObject**

3. **Find XR Grab Interactable component:**
   - **Match Attach Position**: ✅ Checked
   - **Match Attach Rotation**: ❌ **UNCHECK THIS!**
   - **Movement Type**: Change to **Instantaneous**

4. **Save prefab** (Ctrl+S)

---

### Step 4: Setup Delete Button

1. **Find "Delete Button"** in Hierarchy (should already exist)

2. **In Inspector → Button component:**
   - Find **OnClick()** event
   - Click **+** to add event
   - Drag **"MBR Auto Spawner"** GameObject
   - Select Function: **ARMBRAutoSpawner → DeleteSpacecraft()**

3. ✅ Delete button now removes MBR!

---

### Step 5: Remove UI Menus (Optional)

If you don't want the object selection menu:

1. **Find "Object Menu"** or similar in Hierarchy
2. **Disable** it (uncheck in Inspector)

The Create button and shape buttons are now unnecessary!

---

## 🎮 How It Works

### User Experience:

```
1. Launch AR app
2. Point at floor → Green grid appears (AR planes)
3. Tap anywhere on floor → MBR spacecraft spawns
4. Spacecraft is already selected (selection box visible)
5. Drag → Moves along surface (stays upright)
6. Pinch → Makes bigger/smaller
7. Twist → Rotates around vertical axis
8. Tap Delete → Spacecraft disappears
9. Tap again → New spacecraft spawns
```

---

## 🔧 Adjust Size

### If Spacecraft Still Too Big:

**Change Spawn Scale:**
- MBR Auto Spawner → **Spawn Scale**: `0.05` (5% size)

### If Too Small:

**Change Spawn Scale:**
- MBR Auto Spawner → **Spawn Scale**: `0.2` (20% size)

### Perfect Table-Top Size:

Try these scales:
- Small model: `0.05` (5cm if spacecraft is 1m)
- Medium model: `0.1` (10cm)
- Large model: `0.2` (20cm)

---

## ✅ XR Grab Interactable Settings

To prevent rotation issues, set these on your MBR prefab:

### Important Settings:

```
XR Grab Interactable:
├── Match Attach Position: ✓ ON
├── Match Attach Rotation: ✗ OFF ← Important!
├── Movement Type: Instantaneous
├── Track Position: ✓ ON
├── Track Rotation: ✓ ON
└── Smooth Position: ✗ OFF
```

**Match Attach Rotation OFF** = Spacecraft keeps its orientation when grabbed!

---

## 🎯 Result

### Before Fix:
- Tap → Select object type
- Tap again → Place it
- Grab → Orientation changes ❌
- Need to navigate menus
- Multiple object types

### After Fix:
- Tap → MBR spawns instantly ✓
- Already selected ✓
- Grab → Stays upright ✓
- No menus needed ✓
- Only MBR ✓

---

## 📱 Clean AR Experience

```
AR View:
┌─────────────────────────────┐
│                             │  ← No menus
│         [Delete]            │  ← Just delete button
│                             │
│          🛸                 │  ← MBR spacecraft
│      (selected)             │     with selection box
│      [□ □ □ □]             │  ← Scale/rotate handles
│                             │
│  _____Floor_____            │  ← AR plane
└─────────────────────────────┘

Tap anywhere → Spawns MBR
Drag → Move
Pinch → Scale
Twist → Rotate
Delete → Remove
```

---

## ✅ Checklist

Setup:
- [ ] Other prefabs removed from Object Spawner (only MBR remains)
- [ ] MBR Auto Spawner component added
- [ ] MBR prefab assigned
- [ ] Spawn Scale set (0.1 recommended)
- [ ] XR Grab Interactable → Match Attach Rotation: OFF
- [ ] XR Grab Interactable → Movement Type: Instantaneous
- [ ] Delete button wired to DeleteSpacecraft()

Test:
- [ ] Tap on floor → MBR spawns
- [ ] Grab and move → Stays upright ✓
- [ ] Pinch → Scales ✓
- [ ] Twist → Rotates ✓
- [ ] Delete button → Removes it ✓
- [ ] Tap again → New one spawns ✓

---

## 🐛 Troubleshooting

### Spacecraft flips/tilts when grabbed

**Solution:**
- Edit MBR prefab
- XR Grab Interactable → **Match Attach Rotation**: ❌ OFF
- Movement Type: **Instantaneous**

### Spacecraft too big/small

**Solution:**
- MBR Auto Spawner → **Spawn Scale**: Adjust 0.05 to 0.2

### Tap doesn't spawn

**Solution:**
- Check MBR prefab is assigned
- Check AR planes are detected (green grid visible)
- Check console for errors

### Can't delete

**Solution:**
- Delete button → OnClick → ARMBRAutoSpawner.DeleteSpacecraft()
- Make sure button is wired correctly

---

**Much simpler!** Just tap, spawn, manipulate, delete. No menus! 🎯

