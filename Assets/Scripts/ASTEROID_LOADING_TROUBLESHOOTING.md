# Troubleshooting: Asteroid Data Not Loading

## 🔍 **Check These Steps:**

### **Step 1: Verify File Import**

**In Unity Project window:**
1. Navigate to `Assets/Scripts/asteroid_ephemeris/`
2. Find `justitia.txt`
3. **Click on it** to select
4. In Inspector, check:
   - **Type**: Should show as "Text Asset" or "TextAsset"
   - If it says "Default" or something else, Unity didn't recognize it

**Fix if needed:**
1. Right-click on `justitia.txt`
2. **Reimport**
3. Or rename it to `justitia.txt.txt` if it's missing extension

---

### **Step 2: Check Console for Debug Messages**

When you press Play, you should see:
```
AsteroidEphemeris (Justitia): Parsing ephemeris file...
  File has XXXXX characters
  File has XXXXX lines
  Found $$SOE at line XX
  Found $$EOE at line XX
  Processed XXXX date lines
AsteroidEphemeris (Justitia): Loaded XXXX data points
  Date range: 2025-11-05 to 2037-12-05
```

**If you DON'T see these messages:**
- File isn't assigned in Inspector
- Script isn't enabled
- GameObject isn't active

---

### **Step 3: Check Inspector Assignment**

**Select JustitiaEphemeris GameObject:**

```
AsteroidEphemerisReader (Script)
├─ Ephemeris File
│  └─ [Should show: justitia (TextAsset)]  ← NOT empty!
└─ Asteroid Info
   └─ Asteroid Name: Justitia
```

**If Ephemeris File is "None":**
- Drag `justitia.txt` from Project window to this slot

---

### **Step 4: Check File is Actually a TextAsset**

**To import .txt as TextAsset:**
1. Select `justitia.txt` in Project
2. Inspector should show it's a TextAsset
3. If not, check file extension (should be `.txt`)

---

## 🐛 **Common Issues:**

### Issue: "No ephemeris file assigned!"
**Solution:**
- Make sure file is dragged to Ephemeris File slot
- Check file exists in Assets folder

### Issue: "No data points found in file!"
**Solutions:**
- File might not have $$SOE and $$EOE markers
- Check Console for "Found $$SOE" message
- Verify file isn't empty or corrupted

### Issue: File has data but "Data not loaded!"
**Solution:**
- Changed to Awake() now - should fix timing issue
- Check isLoaded becomes true in Inspector while playing

### Issue: Parse errors in Console
**Solution:**
- Date format might be different than expected
- Check the debug output showing line content
- Report the error message

---

## ✅ **Verification:**

**After pressing Play, check Inspector:**

Select `JustitiaEphemeris`:
```
Status section should show:
├─ Data Points Loaded: 4383  (or similar number)
├─ Date Range Start: 2025-11-05
└─ Date Range End: 2037-12-05
```

**If these are empty or 0:**
- Parsing failed
- Check Console for error messages
- Verify file format

---

## 🔧 **Quick Test:**

Try this minimal test:

1. Create new scene
2. Create Empty GameObject → Add AsteroidEphemerisReader
3. Assign justitia.txt
4. Press Play
5. Check Console - should see parsing messages

If this works, the issue is with integration. If not, the file/parsing has issues.

---

## 📝 **Report These if Still Not Working:**

1. Console messages when you press Play (copy all)
2. Inspector screenshot of JustitiaEphemeris
3. Does Inspector show "Data Points Loaded" > 0?
4. What does file type show in Project window?

This will help identify the exact issue! 🔍

