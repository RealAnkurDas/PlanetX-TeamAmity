# Adjustable Planet Zoom - Inspector Guide

Each planet's zoom distance is now fully customizable in the Unity Inspector!

---

## 📊 Default Zoom Settings

| Planet | Zoom Distance |
|--------|---------------|
| Sun | 1000 units |
| Mercury | 500 units |
| Venus | 500 units |
| Earth | 500 units |
| Mars | 500 units |
| Jupiter | 1000 units |
| Saturn | 1000 units |
| Uranus | 1000 units |
| Neptune | 1000 units |

---

## ⚙️ How to Adjust in Inspector

### **Step 1: Select PlanetSelector GameObject**

In Hierarchy, click on `PlanetSelector`

### **Step 2: Find Planet Zoom Settings**

In Inspector, you'll see:

```
┌─ Planet Zoom Settings ──────────────────────┐
│ Planet Zoom Settings                         │
│   Size: 9                                    │
│   ▼ Element 0                                │
│      Planet Name: Sun                        │
│      Zoom Distance: 1000                     │
│   ▼ Element 1                                │
│      Planet Name: Mercury                    │
│      Zoom Distance: 500                      │
│   ▼ Element 2                                │
│      Planet Name: Venus                      │
│      Zoom Distance: 500                      │
│   ... (and so on)                            │
└──────────────────────────────────────────────┘
```

### **Step 3: Adjust Any Planet's Zoom**

1. **Expand** the element you want to change (click the ▼)
2. **Change** the **Zoom Distance** value
3. **Test** in Play mode - click that planet to see the new zoom

---

## 🎯 Examples

### **Make Mercury Closer:**
```
Element 1:
  Planet Name: Mercury
  Zoom Distance: 500 → 300  ← Change this
```

### **Make Jupiter Farther:**
```
Element 5:
  Planet Name: Jupiter
  Zoom Distance: 1000 → 1500  ← Change this
```

### **Make Sun Very Far:**
```
Element 0:
  Planet Name: Sun
  Zoom Distance: 1000 → 2000  ← Change this
```

---

## 🔧 Adding New Planets

Want to add the Moon or other bodies?

1. **Increase Size**: Change from `9` to `10` (or more)
2. **New Element Appears**: Element 9
3. **Fill in**:
   - Planet Name: `Moon`
   - Zoom Distance: `400`
4. The Moon will now appear in the dropdown!

---

## 🎨 Recommended Zoom Ranges

### **Small Objects (Inner Planets):**
- Range: 300 - 600 units
- Examples: Mercury, Venus, Earth, Mars
- Closer zoom to see details

### **Large Objects (Outer Planets):**
- Range: 800 - 1500 units
- Examples: Jupiter, Saturn, Uranus, Neptune
- Farther zoom to see the whole planet

### **Very Large (Sun):**
- Range: 1000 - 2000 units
- Far enough to see the whole Sun

---

## 💡 Tips

### **Tip 1: Test While Playing**
- Adjust values in Inspector
- Click on that planet in dropdown
- See the zoom in real-time
- **Note**: Changes during Play mode don't save! Adjust in Edit mode.

### **Tip 2: Match Object Sizes**
- Bigger planets → larger zoom distance
- Smaller planets → smaller zoom distance

### **Tip 3: Consider Trail Visibility**
- Too close: Hard to see orbital trail
- Too far: Planet appears tiny
- Sweet spot: 3-5x the planet's visual size

### **Tip 4: Consistency**
- Group similar planets with similar zooms
- Inner planets: 400-600
- Outer planets: 800-1200
- Sun: 1000-2000

---

## 📐 Finding the Perfect Zoom

### **Method 1: Visual Testing**
1. Set initial guess (e.g., 500)
2. Press Play
3. Click on planet
4. Too close? Increase value
5. Too far? Decrease value
6. Stop Play, adjust, repeat

### **Method 2: Math Formula**
```
Zoom Distance = Planet Visual Radius × 4
```

Measure planet in Scene view, multiply by 4.

---

## 🔄 Resetting to Defaults

If you want to reset all zoom values:

1. Select PlanetSelector
2. Right-click on PlanetSelector script component
3. Click **"Reset"**
4. Default values will be restored

Or manually set them back to:
- Inner planets: 500
- Outer planets: 1000
- Sun: 1000

---

## 🎮 In-Game Behavior

**Dropdown Closed:**
```
Top-Left:
┌────────────┐
│ ▶ Sun      │  ← Shows current anchor
└────────────┘
```

**Dropdown Open:**
```
┌────────────┐
│ ▼ Sun      │  ← Click to close
├────────────┤
│ Mercury    │  ← Zoom: 500
│ Venus      │  ← Zoom: 500
│ Earth      │  ← Zoom: 500
│ Mars       │  ← Zoom: 500
│ Jupiter    │  ← Zoom: 1000
│ Saturn     │  ← Zoom: 1000
│ Uranus     │  ← Zoom: 1000
│ Neptune    │  ← Zoom: 1000
└────────────┘
```

Each planet uses its custom zoom distance when selected!

---

## ✅ Summary

**Benefits:**
- ✓ Adjust each planet's zoom individually
- ✓ No code editing required
- ✓ See values clearly in Inspector
- ✓ Add/remove planets easily
- ✓ Test and tweak in real-time

**Your zoom distances are now fully customizable!** 🎯

