# Current Date/Time Usage Guide

Both the **PlanetPositionInitializer** and **SimulationTimeTracker** scripts now use **current system date and time by default**.

---

## ✅ What Changed

### **PlanetPositionInitializer.cs**
- Now has a checkbox: **"Use Current DateTime"** (enabled by default)
- When checked: Uses your computer's current date/time to fetch planet positions from API
- When unchecked: Uses manual date/time fields

### **SimulationTimeTracker.cs**
- Now has a checkbox: **"Use Current DateTime"** (enabled by default)
- When checked: Starts time tracking from your computer's current date/time
- When unchecked: Uses manual start date/time fields

---

## 🎮 How to Use in Unity

### **Default Behavior (Current DateTime)**

1. **Do nothing!** By default, both scripts will use the current date/time
2. When you press Play:
   - Planets will be positioned based on **today's date and current time**
   - Time tracking will start from **today's date and current time**
   - Both scripts automatically sync to use the same date/time

### **Example:**
If you press Play on **November 5, 2025 at 3:45:30 PM**:
- Planets spawn at their real positions for that exact moment
- Time tracker shows: `Date: 05-11-2025, Time: 15:45:30`

---

## 🔧 Using Manual Date/Time

If you want to simulate a specific historical or future date:

### **In PlanetPositionInitializer Inspector:**
1. Uncheck **"Use Current DateTime"**
2. Set **Manual Date**: e.g., `25-12-2024` (Christmas 2024)
3. Set **Manual Time**: e.g., `12:00:00` (Noon)

### **In SimulationTimeTracker Inspector:**
1. Uncheck **"Use Current DateTime"**
2. Set **Manual Start Date**: e.g., `25-12-2024`
3. Set **Manual Start Time**: e.g., `12:00:00`

**Important:** Make sure both scripts use the same date/time!

---

## 📊 Inspector Layout

### **PlanetPositionInitializer**

```
┌─ API Configuration ────────────────────────┐
│ API Base URL: http://localhost:8000        │
└────────────────────────────────────────────┘

┌─ Date/Time Settings ───────────────────────┐
│ ☑ Use Current DateTime                     │
│ Manual Date: 05-11-2025                    │
│ Manual Time: 20:00:00                      │
└────────────────────────────────────────────┘

┌─ Scale Configuration ──────────────────────┐
│ Scale To Unity Units: 1E-09                │
└────────────────────────────────────────────┘

┌─ Status ───────────────────────────────────┐
│ Positions Loaded: false                    │
│ Actual Date Used: (shows at runtime)       │
│ Actual Time Used: (shows at runtime)       │
└────────────────────────────────────────────┘
```

### **SimulationTimeTracker**

```
┌─ References ───────────────────────────────┐
│ Earth Transform: (auto-detected)           │
│ Sun Transform: (auto-detected)             │
└────────────────────────────────────────────┘

┌─ Starting Date/Time ───────────────────────┐
│ ☑ Use Current DateTime                     │
│ Manual Start Date: 05-11-2025              │
│ Manual Start Time: 20:00:00                │
└────────────────────────────────────────────┘

┌─ Current Simulation Time ──────────────────┐
│ Days Elapsed: 0.0                          │
│ Years Elapsed: 0.00                        │
│ Orbital Progress: 0.0                      │
│ Current Date: (updates during play)        │
│ Current DateTime: (updates during play)    │
└────────────────────────────────────────────┘
```

---

## 🔍 Status Fields

### **PlanetPositionInitializer Status**
- **Actual Date Used**: Shows which date was actually used (visible at runtime)
- **Actual Time Used**: Shows which time was actually used (visible at runtime)

These fields help you verify what date/time is being sent to the API.

---

## 📝 Console Messages

### When Using Current DateTime:
```
Using current date/time: 05-11-2025 15:45:30
Fetching planet positions from API...
Time Tracker: Using current date/time: 05-11-2025 15:45:30
```

### When Using Manual DateTime:
```
Using manual date/time: 25-12-2024 12:00:00
Fetching planet positions from API...
Time Tracker: Using manual date/time: 25-12-2024 12:00:00
```

---

## 🎯 Common Use Cases

### **1. Real-Time Solar System**
**Goal:** Show planets as they are RIGHT NOW

**Setup:**
- ✅ Keep "Use Current DateTime" checked (both scripts)
- Press Play anytime to see current planetary positions

### **2. Historical Event**
**Goal:** See where planets were during a specific event

**Example:** Apollo 11 Moon Landing (July 20, 1969)

**Setup:**
- ❌ Uncheck "Use Current DateTime" (both scripts)
- Set Manual Date: `20-07-1969`
- Set Manual Time: `20:17:00` (UTC)

### **3. Future Prediction**
**Goal:** See where planets will be in the future

**Example:** New Year 2030

**Setup:**
- ❌ Uncheck "Use Current DateTime" (both scripts)
- Set Manual Date: `01-01-2030`
- Set Manual Time: `00:00:00`

---

## 🚨 Important Notes

### **Date Range Limitation**
The JPL DE421 ephemeris only covers:
- **Start:** 1900-07-28
- **End:** 2053-10-08

Dates outside this range will cause API errors!

### **Time Zone**
- All times are in **your system's local time zone**
- The API expects UTC, but for most purposes, local time is fine
- For precise historical astronomy, consider UTC conversion

### **Synchronization**
Both scripts will use the same current time when:
- They both have "Use Current DateTime" checked
- They both run in the same frame (they do - PlanetPositionInitializer runs in Awake)

---

## 🔄 Runtime Behavior

### **What Happens on Play:**

1. **PlanetPositionInitializer (Awake):**
   - Gets current date/time (if enabled)
   - Calls API with that date/time
   - Positions planets

2. **SimulationTimeTracker (Start):**
   - Gets current date/time (if enabled)
   - Records as starting time
   - Begins tracking Earth's orbit

3. **Time Advances:**
   - SimulationTimeTracker updates based on Earth's movement
   - Shows how much simulation time has passed since start

---

## 💡 Pro Tips

### **Tip 1: Quick Testing**
- Keep current date/time enabled during development
- See real planet positions instantly
- Easy to verify against real astronomical data

### **Tip 2: Reproducible Simulations**
- Use manual date/time for demonstrations
- Same date/time = same starting positions every time
- Good for screenshots, videos, presentations

### **Tip 3: Time-Lapse**
- Start with a manual date
- Let simulation run
- Watch months/years pass in minutes

### **Tip 4: Validation**
- Use current date/time
- Compare with real astronomy websites
- Verify your simulation accuracy

---

## 🐛 Troubleshooting

### Issue: "Actual Date Used" is empty
**Solution:** This only shows at runtime. Press Play to see it populate.

### Issue: Date seems wrong
**Solution:** 
- Check if "Use Current DateTime" is actually checked
- Verify your system date/time is correct
- Check Console for the debug message showing which date is used

### Issue: API returns error about date
**Solution:**
- Verify date is within 1900-2053 range
- Check date format is dd-mm-yyyy (not mm-dd-yyyy)
- Ensure API server is running

### Issue: Scripts use different times
**Solution:**
- Both scripts should have same "Use Current DateTime" setting
- If using manual, ensure both have same manual date/time values

---

## 📚 Summary

| Feature | Default | Can Change? | Purpose |
|---------|---------|-------------|---------|
| Use Current DateTime | ✅ Enabled | Yes | Automatic real-time positions |
| Manual Date | 05-11-2025 | Yes | Backup/specific date |
| Manual Time | 20:00:00 | Yes | Backup/specific time |
| Actual Date Used | (runtime) | Read-only | Shows what was used |
| Actual Time Used | (runtime) | Read-only | Shows what was used |

---

**Your simulation now always starts with real current positions by default!** 🌍🪐✨

