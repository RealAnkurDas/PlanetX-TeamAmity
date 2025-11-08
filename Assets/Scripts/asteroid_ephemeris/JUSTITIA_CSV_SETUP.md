# Justitia CSV Setup - Quick Guide

## ✅ **Files:**

- ✅ **justitia_clean.csv** - Clean CSV format (USE THIS!)
- ❌ ~~justitia.csv~~ - Deleted
- ⚠️ **Justitia.txt** - Original JPL format (can keep for reference)

---

## 🎯 **Setup in Unity:**

### **Step 1: Import CSV**
1. The file `justitia_clean.csv` is already in `Assets/Scripts/asteroid_ephemeris/`
2. Unity should recognize it as a TextAsset
3. If not, right-click → Reimport

### **Step 2: Assign to Reader**
1. Select `JustitiaEphemeris` GameObject in Hierarchy
2. In Inspector, **AsteroidEphemerisReader** component:
   - **Ephemeris File**: Drag `justitia_clean.csv` here
   - **Asteroid Name**: `Justitia`

### **Step 3: Press Play!**

**Console should show:**
```
AsteroidEphemeris (Justitia): Parsing ephemeris file...
  File has XXXXX characters
  File has 12477 lines
  Found CSV header at line 0
  Successfully parsed 12476 data lines
AsteroidEphemeris (Justitia): Loaded 12476 data points
  Date range: 2025-11-05 to 2060-01-01
```

**Justitia should now move!** 🪐

---

## 📊 **CSV Format:**

```csv
JDTDB,Date,X_km,Y_km,Z_km,VX_km_s,VY_km_s,VZ_km_s,LT,RG,RR
2460984.500000000, A.D. 2025-Nov-05 00:00:00.0000, -3.833228E+08, 7.241479E+06, 1.402014E+07, ...
```

**Columns used:**
- Column 1: Date (parsed for timestamp)
- Column 2: X in kilometers
- Column 3: Y in kilometers  
- Column 4: Z in kilometers

---

## 🔧 **If It Doesn't Work:**

### Check Console for:
- "Found CSV header" message
- "Successfully parsed XXXX data lines"
- Any error messages

### Common Issues:
1. **File not assigned**: Drag justitia_clean.csv to Ephemeris File slot
2. **Wrong asteroid name**: Must be exactly "Justitia" (case-sensitive)
3. **GameObject missing**: Create GameObject named "Justitia"

---

## ✅ **Verification:**

**In Inspector while playing:**
```
JustitiaEphemeris → AsteroidEphemerisReader:
  Status:
    Data Points Loaded: 12476
    Date Range Start: 2025-11-05
    Date Range End: 2060-01-01
```

**If these show values, it's working!** ✓

---

**Your asteroid is now ready to move based on JPL data!** 🎉

