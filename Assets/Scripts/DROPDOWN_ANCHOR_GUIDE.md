# Dropdown Anchor Selector - Quick Guide

A collapsible dropdown menu at the top-left for selecting camera anchor.

---

## 🎯 How It Works

### **Default State (Collapsed):**
```
Top-Left Corner:
┌──────────────┐
│ ▶ Sun        │  ← Click to expand
└──────────────┘
```

### **Expanded State:**
```
Top-Left Corner:
┌──────────────┐
│ ▼ Sun        │  ← Click to collapse
├──────────────┤
│ Mercury      │  ← Click to select
│ Venus        │
│ Earth        │
│ Mars         │
│ Jupiter      │
│ Saturn       │
│ Uranus       │
│ Neptune      │
└──────────────┘
```

---

## 🎮 Usage

1. **Click the dropdown** (shows current anchor with ▶ or ▼)
2. **Menu expands** showing all planets
3. **Click a planet name** to:
   - Change camera anchor to that planet
   - Auto-zoom to 200 units
   - Dropdown automatically closes

---

## ⚙️ Setup (Simple!)

### **Step 1: Create GameObject**
1. Right-click in Hierarchy → Create Empty
2. Rename to: `PlanetSelector`
3. Add Component → `PlanetSelector` script

### **Step 2: Assign Camera**
1. In Inspector, drag **Main Camera** to **"Camera Controller"** field

**Done!** ✅

---

## 📊 Settings

### **Display Settings:**
- **Menu Left Offset**: `20` (default, at left edge)
- **Menu Top Offset**: `20` (default, at top edge)

### **Planet Names:**
Default list includes all planets. You can add/remove:
- Click `+` to add more
- Click `-` to remove
- Edit names to match your GameObjects

---

## 🎨 Visual States

### **Collapsed (Default):**
- Shows: `▶ Sun` (or current anchor)
- Cyan color
- Small and compact

### **Expanded:**
- Shows: `▼ Sun` at top
- Lists all other planets below
- White text (yellow on hover)
- Auto-closes after selection

---

## 🔍 Zoom Behavior

When you select any planet:
- **Automatic zoom to 200 units** from the planet
- Consistent distance for all celestial bodies
- Good balance between seeing the object and its orbital path

---

## 💡 Features

### **Auto-Close:**
- Dropdown closes automatically after selecting a planet
- Keeps UI clean and unobtrusive

### **Visual Feedback:**
- **▶** = Closed
- **▼** = Open
- **Cyan** = Current selection
- **Yellow** = Hover over option

### **Smart Positioning:**
- Top-left corner (customizable)
- Doesn't overlap with other UI elements
- Compact when closed

---

## 🐛 Troubleshooting

### Dropdown doesn't appear
- Make sure PlanetSelector GameObject exists
- Press Play to see it (doesn't show in Edit mode)

### Clicking doesn't open dropdown
- Check that script is enabled
- Make sure you're clicking in Game view, not Scene view

### Planet selection doesn't work
- Camera Controller must be assigned
- Check planet names match GameObjects
- Look in Console for warning messages

### Camera doesn't zoom correctly
- Default zoom is 200 units
- Adjust in script if needed
- Check TouchCameraController min/max zoom settings

---

## 📝 Summary

**Setup:**
1. Create PlanetSelector GameObject + script
2. Assign Camera Controller

**Usage:**
- Click dropdown to expand/collapse
- Click planet name to change anchor
- Auto-zooms to 200 units
- Dropdown auto-closes

**Position:** Top-left corner, compact and clean! ✨

