# 🔧 AR Troubleshooting Guide

Fixes for common AR issues: jittering, trails, and zoom not working.

---

## Issue 1: Sun/Text Jittering 📳

### Cause:
`ConstantSizeText` script is fighting with AR camera/scale changes.

### Fix A: Disable ConstantSizeText on Sun (Quick Fix)

1. **Find Sun GameObject** in Hierarchy
2. **Expand it** to find text children (like "Sun" text label)
3. **For each text child:**
   - Add Component → **Disable In AR**
   - Check "Disable GameObject" is ✅ checked
4. ✅ Text will be hidden in AR mode, visible in regular mode

### Fix B: Keep Text But Stop Jitter (Better)

**Option 1: Disable ConstantSizeText Only**
1. Find Sun's text object
2. Add Component → **Disable In AR**
3. Uncheck "Disable GameObject"
4. **Components To Disable**: Drag `ConstantSizeText` script here

**Option 2: Remove ConstantSizeText Altogether**
1. Find all text objects with ConstantSizeText
2. In Inspector, **Remove Component** → ConstantSizeText
3. Set text to fixed size instead

---

## Issue 2: Planet Selection Doesn't Zoom 🎯

### Check Console Messages:

When you select a planet, you SHOULD see:

```
✓ PlanetSelector (AR Mode): Selected 'Mars' - ARPlanetZoom will handle zoom
✓ ARPlanetZoom Update: Current='Mars', Last='Sun', AutoZoom=True
✓ ARPlanetZoom: ★★★ Planet selection changed to 'Mars' - auto-zooming ★★★
✓ ARPlanetZoom: Attempting to zoom to 'Mars'
✓ ARPlanetZoom: Found planet 'Mars' at position (...)
✓ ARPlanetZoom: Zooming to Mars at distance 0.020
```

### If You DON'T See These:

#### Missing: "PlanetSelector (AR Mode): Selected..."
**Problem:** PlanetSelector not updating in AR mode
**Fix:** Make sure you accepted the latest PlanetSelector.cs changes

#### Missing: "ARPlanetZoom Update: Current=..."
**Problem:** ARPlanetZoom component not in scene
**Fix:**
1. Find/Create "AR Manager" GameObject
2. Add Component → AR Planet Zoom
3. Check "Auto Zoom On Select" is ✅ checked

#### Missing: "Found planet..."
**Problem:** Planet GameObjects not found by name
**Fix:** Console will show exact error, e.g. "Could not find planet 'Mars'"

### Diagnostic Commands:

**In Console filter, search for:**
- "ARPlanetZoom" - Shows all zoom activity
- "PlanetSelector" - Shows selection changes
- "Found planet" - Shows if planets are being found

---

## Issue 3: MBR Explorer Trail Shooting Out 🚀

### Cause:
Trail Renderer uses world-space positions, and with XR Origin scaled 1000x, trails become HUGE.

### Fix: Disable Trails in AR

1. **Find MBR Explorer GameObject** in Hierarchy
2. **Find child with Trail Renderer** component
3. **Add Component** → **Disable In AR**
4. **Settings:**
   - Disable GameObject: ✅ **Checked**
5. ✅ Trail will be hidden in AR, visible in regular scene

**OR disable all trails at once:**

1. Search Hierarchy for "Trail" or "trail"
2. For each trail object:
   - Add **Disable In AR** component
   - Check "Disable GameObject"

---

## Quick Fix Checklist

### Fix Jittering:
- [ ] Find Sun's text objects
- [ ] Add "Disable In AR" component
- [ ] Set to disable GameObject or just ConstantSizeText

### Fix Zoom Not Working:
- [ ] Check AR Manager has ARPlanetZoom component
- [ ] Check "Auto Zoom On Select" is checked
- [ ] Press Play and select planet
- [ ] Check console for debug messages
- [ ] Paste console output if still not working

### Fix Trail:
- [ ] Find MBR Explorer's trail object
- [ ] Add "Disable In AR" component
- [ ] Set to disable GameObject

---

## Console Debug Output You Should See

### On Scene Start:
```
ARSolarSystemManager: Configured AR camera - Near: 0.01, Far: 10000000
ARSolarSystemManager: Scaled XR Origin by 1000x
ARPlanetZoom: Found ARSolarSystemManager
ARPlanetZoom: Found PlanetSelector, current anchor: 'Sun'
ARPlanetZoom: Initialized - Auto-zoom: True
ARPlanetZoom: Will auto-zoom to initial planet on first Update()
ARSolarSystemManager: Placed solar system at (...), height: 1500
ARPlanetZoom: Stored original position: (...)
```

### Every Second (Update Loop):
```
ARPlanetZoom Update: Current='Sun', Last='', AutoZoom=True
```

### When Selecting Mars:
```
PlanetSelector (AR Mode): Selected 'Mars' - ARPlanetZoom will handle zoom
ARPlanetZoom: ★★★ Planet selection changed to 'Mars' - auto-zooming ★★★
ARPlanetZoom: Attempting to zoom to 'Mars'
ARPlanetZoom: Found planet 'Mars' at position (...)
ARPlanetZoom: Zooming to Mars at distance 0.020
  Planet world pos: (...)
  Camera pos: (...)
  Direction: (...)
  (more debug info...)
```

### A Few Frames Later:
```
ARPlanetZoom: Zoom complete
```

---

## If Still Not Working

**Do this:**

1. Press Play
2. Wait 2 seconds
3. Open dropdown, select Mars
4. **Copy ALL console messages**
5. Paste them here

The detailed logging will show exactly where it's failing!

---

## Expected Behavior (When Working)

1. ✅ Scene starts → Solar system floats above floor
2. ✅ No jittering text
3. ✅ Select "Mars" → Solar system smoothly moves so Mars is in view
4. ✅ Select "Jupiter" → Solar system moves to show Jupiter
5. ✅ No weird trails shooting out
6. ✅ Reset button returns to original view

---

## Alternative Quick Test

**Use the Test Buttons:**

I added test buttons to ARPlanetZoom debugger. Press Play and you'll see:
- Top-left: "Test Zoom Mars" button
- Below it: "Test Reset" button

Click these to manually trigger zoom (bypasses dropdown).

If test buttons work but dropdown doesn't:
- Problem is in dropdown → ARPlanetZoom connection
- Check that PlanetSelector is updating currentAnchor

---

**Press Play and check console!** The debug messages will tell us what's failing. 🔍

