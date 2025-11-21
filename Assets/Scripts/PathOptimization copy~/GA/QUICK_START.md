# 🚀 Quick Start - Trajectory GA

## What You Have Now

All files are in: `Assets/Scripts/PathOptimization/GA/`

### C# Standalone (Terminal - Fastest!)
- ✅ `Program.cs` - Main application
- ✅ `TrajectoryGA.csproj` - .NET project file
- ✅ `run_ga.sh` - Bash runner script
- ✅ `RUN_GA.bat` - Windows batch runner script

### Unity Version (Editor)
- ✅ `TrajectoryGA.cs` - MonoBehaviour version
- ✅ `TrajectoryGAController.cs` - UI controller

### Python Version (Original)
- ✅ `gravity_ga.py` - Python implementation

---

## 🏃 How to Run (3 Options)

### Option 1: Terminal - C# Standalone (Recommended - Fastest!)

**First Time Setup:**
1. Install .NET SDK: https://dotnet.microsoft.com/download
2. Restart your terminal

**Run:**
```bash
cd "c:/Users/ankur/Planet X - AR/Assets/Scripts/PathOptimization/GA"

# Easy way (using script)
./run_ga.sh

# Or direct way
dotnet run

# With custom parameters
dotnet run 50 100  # 50 population, 100 generations
```

**Speed: ~20-40 seconds for 75 generations** ⚡

---

### Option 2: Unity (In-Editor)

1. Open Unity
2. Create empty GameObject
3. Add Component → `Trajectory GA`
4. Press Play
5. In Console, find the component and call `StartOptimization()`

Or use the controller:
1. Add Component → `Trajectory GA Controller`
2. Press Play
3. Press **'G'** key to start

**Speed: ~60-90 seconds (Debug mode)**

---

### Option 3: Python (Original)

```bash
cd "c:/Users/ankur/Planet X - AR/Assets/Scripts/PathOptimization/GA"
python gravity_ga.py
```

**Speed: ~15-30 minutes**

---

## 📊 What You'll See

```
=============================================================
Trajectory Genetic Algorithm - Standalone C#
=============================================================
Mission Timeline: 2028-03-03 → 2035-12-31
Population: 40, Generations: 75
=============================================================

Gen   0 | Fitness: 523.45 | Dist: 4.234 AU | ΔV: 15234 m/s | Time:  892d
Gen   5 | Fitness:  89.23 | Dist: 2.145 AU | ΔV: 14523 m/s | Time:  756d
...
Gen  74 | Fitness:   2.15 | Dist: 0.087 AU | ΔV: 12234 m/s | Time:  567d

⏱️  Total execution time: 23.45 seconds

============================================================
🎯 OPTIMIZATION COMPLETE!
============================================================

Best Solution Found:
  • Minimum Distance to Jupiter: 0.0874 AU (13074 km)
  • Total Delta-V: 12234 m/s
  • Mission Time: 567 days (1.55 years)

Mission Parameters:
  • Launch Velocity: [3955, 9234] m/s
  • Maneuver 1 (Day 234): ΔV = [-1234, 567] m/s
  • Maneuver 2 (Day 489): ΔV = [890, -2134] m/s
============================================================
```

---

## 🎯 Next Steps

1. **Install .NET SDK** (if you haven't)
2. **Run the script:** `./run_ga.sh` or `RUN_GA.bat`
3. **Watch it optimize!** 🎉

---

## 💡 Tips

- **Faster results?** Use Release mode: `dotnet run -c Release`
- **Different parameters?** `dotnet run 30 50` (smaller/faster test)
- **Bigger search?** `dotnet run 100 150` (better solutions, slower)

---

## 📖 More Info

See `README_CSHARP.md` for detailed documentation.

---

**Ready to launch! 🚀**

