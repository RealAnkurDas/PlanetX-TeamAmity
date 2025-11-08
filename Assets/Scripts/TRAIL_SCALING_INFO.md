# Trail Renderer Camera Scaling

The trail renderer now automatically scales with camera distance to maintain consistent visual thickness.

---

## ✅ How It Works

The trail width dynamically adjusts based on the camera's distance from the planet:
- **Close up**: Trail appears thinner
- **Far away**: Trail appears thicker
- **Result**: Trail looks the same visual thickness from any distance!

---

## 📊 Formula

```
Scaled Width = Base Width × (Current Distance / Reference Distance)
```

**Example:**
- Base Width: 0.5
- Reference Distance: 100 units
- Current Distance: 200 units
- Result: Trail width = 0.5 × (200/100) = **1.0 units**

---

## ⚙️ Settings (in Inspector)

Select a planet with **DelayedTrailRenderer** script:

### **Trail Settings:**

1. **Trail Start Delay**: `2` seconds
   - How long to wait before showing trail

2. **Scale With Distance**: ✓ **Enabled**
   - Turn on/off the scaling feature

3. **Base Width**: `0.5`
   - The trail width at the reference distance
   - Adjust this to make trails thicker or thinner overall

4. **Reference Distance**: `100`
   - The distance where trail width = base width
   - Should match your typical viewing distance

---

## 🎯 Recommended Settings

### **For Your Solar System (100 unit zoom):**

```
✓ Scale With Distance: Enabled
Base Width: 0.5
Reference Distance: 100
```

This ensures trails look consistent when you:
- Zoom in close to a planet
- Zoom out to see the whole orbit
- Switch between different planets

---

## 🔧 Adjusting Trail Appearance

### **Make ALL Trails Thicker:**
- Increase **Base Width**: `0.5` → `1.0` or `2.0`

### **Make ALL Trails Thinner:**
- Decrease **Base Width**: `0.5` → `0.25` or `0.1`

### **Different Reference Distance:**
If you're viewing from different distances:
- Set **Reference Distance** to your typical zoom level
- Example: If you usually view at 50 units, set to `50`

### **Disable Scaling:**
If you want fixed-width trails:
- Uncheck **"Scale With Distance"**
- Trail will use Unity's default width settings

---

## 📐 Technical Details

### **Start vs End Width:**
- **Start Width**: Full calculated width
- **End Width**: Half of start width (creates nice taper effect)

### **Update Frequency:**
- Trail width updates every frame
- No performance impact (simple distance calculation)

### **Camera Detection:**
- Uses `Camera.main` automatically
- No manual camera assignment needed

---

## 💡 Tips

### **Tip 1: Test Different Zoom Levels**
1. Focus on a planet
2. Zoom in close
3. Zoom out far
4. Trail should look the same thickness!

### **Tip 2: Adjust Per Planet (Optional)**
You can have different settings for each planet:
- Mercury: Thinner trail (Base Width: 0.3)
- Jupiter: Thicker trail (Base Width: 1.0)

### **Tip 3: Match Your Scale**
If your scene uses different distance units:
- Adjust **Reference Distance** to match
- Example: Scene in kilometers? Set to typical viewing km

---

## 🎨 Visual Comparison

### **Without Scaling:**
```
Far:  ---- (barely visible)
Near: ████████ (way too thick)
```

### **With Scaling:**
```
Far:  ████ (consistent)
Near: ████ (consistent)
```

---

## ✅ Summary

**Default settings work great for your setup:**
- ✓ Enabled by default
- ✓ Base Width: 0.5
- ✓ Reference: 100 units
- ✓ Auto-scales every frame

Just add the **DelayedTrailRenderer** script to each planet and you're done! 🚀

