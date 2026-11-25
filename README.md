# 🚀 MBR++ - Unity + Android AR Project

An integrated Unity AR and Android native app project for visualizing a space mission to asteroid 269 Justitia, featuring real-time planetary position calculations, AI-driven trajectory optimization using genetic algorithms, and immersive AR visualization.

## 📁 Project Structure

```
Planet X - AR/
├── Assets/
│   ├── PlanetXNative/              # Android native app (Kotlin + Compose) - COPY OUTSIDE BEFORE USE
│   ├── Scenes/                     # Unity scenes
│   ├── Scripts/
│   │   ├── python/                 # Python API server for planetary positions
│   │   │   ├── api_server.py       # FastAPI server
│   │   │   └── requirements.txt    # Python dependencies
│   │   ├── PathOptimization/GA/    # C# Genetic Algorithm for trajectory optimization
│   │   │   ├── ProgramJustitia.cs  # Main trajectory optimizer
│   │   │   └── TrajectoryJustitia.csproj  # .NET 9.0 project
│   │   └── ...                     # Other Unity C# scripts
│   └── Real Stars Skybox/          # Large asset files (tracked via Git LFS)
├── UnityExports/
│   └── PlanetXAR/                  # Unity Android library export
│       └── unityLibrary/           # Used by Android app
├── ProjectSettings/
└── Packages/
```

---

## 🚀 Complete Setup Guide (From GitHub Clone)

Follow these steps exactly to set up the project from scratch.

### Prerequisites

#### Required Software
1. **Git with Git LFS** (for large asset files)
   - Download: https://git-scm.com/
   - Install Git LFS: `git lfs install`

2. **Unity 6** (6000.2.10f1 or newer)
   - Download Unity Hub: https://unity.com/download
   - Install Unity 6 with **Android Build Support** module
   - Include **Android SDK & NDK Tools**, **OpenJDK**

3. **Android Studio** (Latest version - Ladybug or newer)
   - Download: https://developer.android.com/studio
   - During installation, ensure **Android SDK**, **Android NDK**, and **Android SDK Platform-Tools** are installed

4. **Python 3.9+** (for planetary position API)
   - Download: https://www.python.org/downloads/
   - Make sure to check "Add Python to PATH" during installation

5. **.NET 9.0 SDK** (for trajectory optimization)
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0

6. **ARCore-compatible Android Device**
   - Check compatibility: https://developers.google.com/ar/devices
   - USB debugging enabled
   - Android 7.0 (API level 24) or higher

---

### Step 1: Clone the Repository

```bash
# Clone the repository with Git LFS support
git clone https://github.com/yourusername/your-repo.git
cd "Planet X - AR"

# Fetch LFS files (large assets like cubemaps)
git lfs pull
```

**Important**: This project uses **Git LFS** for large files (192 MB cubemap files). If you see small `.cubemap` files instead of large ones, run `git lfs pull`.

---

### Step 2: Set Up Python API Server

The Python API server provides real-time planetary position calculations.

```bash
# Navigate to Python directory
cd "Assets/Scripts/python"

# Create virtual environment (recommended)
python -m venv .myenv

# Activate virtual environment
# On Windows:
.myenv\Scripts\activate
# On macOS/Linux:
source .myenv/bin/activate

# Install dependencies
pip install -r requirements.txt

# Start the API server
python api_server.py
```

The server will start on `http://localhost:8000`

**Test the API:**
```bash
curl "http://localhost:8000/get_coords?date=05-11-2025&time=14:30:00"
```

**Keep this terminal running** while developing. The Unity app will connect to this API.

📖 **Full API Documentation**: See `Assets/Scripts/python/API_README.md`

---

### Step 3: Set Up C# Trajectory Optimizer (Optional)

The genetic algorithm trajectory optimizer is optional but recommended for advanced trajectory planning.

```bash
# Navigate to GA directory
cd "Assets/Scripts/PathOptimization/GA"

# Build the project
dotnet build TrajectoryJustitia.csproj

# Run trajectory optimization (optional)
dotnet run --project TrajectoryJustitia.csproj
```

This will generate optimal trajectory paths to asteroid Justitia, saved as CSV files for visualization.

---

### Step 4: Open Unity Project

1. **Launch Unity Hub**

2. **Add Project**
   - Click **"Add"** → **"Add project from disk"**
   - Navigate to the cloned `Planet X - AR` folder
   - Select it (this is the Unity project root)

3. **Open Project**
   - Unity will import all packages and assets (may take 5-10 minutes on first open)
   - Wait for all imports to complete

4. **Configure Android Build Settings**
   - Go to **File > Build Settings**
   - Select **Android** platform
   - Click **"Switch Platform"** if not already on Android
   - Set **Texture Compression**: ASTC
   - Set **API Level**: Android 7.0 (API level 24) or higher

5. **Export Unity Android Library**
   - In Build Settings, check **"Export Project"**
   - Set export path: `UnityExports/PlanetXAR`
   - Click **"Export"** (NOT "Build")
   - Wait for export to complete

**Note**: You must export the Unity project before building the Android app. Re-export whenever you make Unity changes.

---

### Step 5: Set Up Android Native App

⚠️ **IMPORTANT**: The `PlanetXNative` folder is included in this repository for reference, but **you should copy it outside the Unity project** before working on it in Android Studio. Unity will automatically generate `.meta` files for Android resources, which will cause build errors.

**Recommended Setup:**

1. **Copy PlanetXNative Outside Unity Project**
   ```bash
   # From the repository root, copy PlanetXNative to a location outside
   # Example: Copy to parent directory or separate location
   cp -r "Planet X - AR/PlanetXNative" "../PlanetXNative"
   # Or on Windows:
   xcopy "Planet X - AR\PlanetXNative" "..\PlanetXNative\" /E /I
   ```

2. **Update Unity Export Path Reference**
   - After copying, you'll need to update the `settings.gradle.kts` file in the copied `PlanetXNative` folder
   - Update the relative path to `unityLibrary` to point to: `../../Planet X - AR/UnityExports/PlanetXAR/unityLibrary`
   - Or use an absolute path if preferred

3. **Open Android Project**
   - Launch **Android Studio**
   - Click **"Open"**
   - Navigate to the **copied** `PlanetXNative` folder (outside Unity project)
   - Click **OK**

2. **Gradle Sync**
   - Android Studio will automatically sync Gradle
   - The `unityLibrary` will be found via relative path: `../../UnityExports/PlanetXAR/unityLibrary`
   - Wait for sync to complete (may take 5-10 minutes on first run)

3. **Resolve Dependencies**
   - If Gradle sync fails, check:
     - Unity library was exported to correct location
     - Relative paths in `settings.gradle.kts` are correct
     - Android SDK and NDK are installed

4. **Clean Build** (if you copied project from another location)
   ```bash
   cd ../PlanetXNative  # Or wherever you copied it
   ./gradlew clean
   
   # Remove any .meta files from Android resources (if Unity accessed the folder)
   find app/src/main/res -name "*.meta" -delete
   
   # Build again
   ./gradlew assembleDebug
   ```

**Alternative: If You Must Work Inside Unity Project**

If you need to keep `PlanetXNative` inside the Unity project (not recommended), you'll need to:
- Delete `.meta` files regularly: `find PlanetXNative/app/src/main/res -name "*.meta" -delete`
- Add `PlanetXNative/` to Unity's **Version Control** → **Visible Meta Files** exclusion list
- Be prepared for Unity to regenerate `.meta` files whenever you access the folder in Unity Editor

---

### Step 6: Build and Run on Device

1. **Connect Android Device**
   - Enable **Developer Options** on your device
   - Enable **USB Debugging**
   - Connect via USB
   - Accept USB debugging prompt on device

2. **Select Device in Android Studio**
   - Click the device dropdown in toolbar
   - Select your connected device

3. **Run the App**
   - Click the **Run** button (green play icon)
   - Or press **Shift+F10**
   - App will install and launch on your device

4. **Grant Permissions**
   - Camera permission (required for AR)
   - Storage permission (if needed)

---

## 🔧 How It Works

### System Architecture

1. **Python API Server** (`Assets/Scripts/python/`)
   - FastAPI server providing real-time planetary positions
   - Uses JPL DE421 ephemeris for accurate calculations
   - Returns heliocentric coordinates in meters
   - Date range: 1900-07-28 to 2053-10-08

2. **C# Trajectory Optimizer** (`Assets/Scripts/PathOptimization/GA/`)
   - Genetic algorithm for optimal trajectory planning
   - Calculates mission paths to asteroid Justitia
   - Outputs CSV files for visualization
   - Written in C# (.NET 9.0)

3. **Unity AR Engine** (Unity 6)
   - 3D visualization of solar system
   - AR rendering using ARCore
   - Reads trajectory data and planetary positions
   - Exports as Android library

4. **Android Native App** (`Assets/PlanetXNative/`)
   - Modern Kotlin + Jetpack Compose UI
   - Integrates Unity AR view
   - Dashboard, education, and visualizer screens
   - Communicates with Python API

### Integration Flow

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│   Python    │──────│     Unity    │──────│   Android   │
│  API Server │ HTTP │   AR Engine  │ JNI  │  Native App │
└─────────────┘      └──────────────┘      └─────────────┘
        │                    │                     │
        │                    │                     │
   Port 8000            unityLibrary        Kotlin + Compose
```

---

## 📝 Development Workflow

### Daily Development

1. **Start Python API Server** (keep running)
   ```bash
   cd Assets/Scripts/python
   python api_server.py
   ```

2. **Make Changes in Unity**
   - Edit scenes, scripts, assets in Unity Editor
   - Test in Play mode

3. **Export Unity Changes**
   - File > Build Settings > Export Project
   - Export to `UnityExports/PlanetXAR`

4. **Test in Android Studio**
   - Open `Assets/PlanetXNative`
   - Run on device

### Making Changes

#### Unity Changes (C# Scripts, Scenes, Assets)
- Edit in Unity Editor
- Always **Export** after changes (not Build)
- Export path: `UnityExports/PlanetXAR`

#### Android UI Changes (Kotlin, Compose)
- Edit in Android Studio (using the **copied** PlanetXNative folder outside Unity project)
- No Unity export needed for UI-only changes
- Just build and run
- **Note**: If you make changes to the PlanetXNative folder inside Unity project, Unity will generate `.meta` files that need to be deleted before building

#### Python API Changes
- Edit `api_server.py`
- Restart server: `python api_server.py`

#### Trajectory Optimizer Changes
- Edit `ProgramJustitia.cs`
- Rebuild: `dotnet build TrajectoryJustitia.csproj`

---

## 🐛 Troubleshooting

### Git LFS Issues

**Problem**: Large `.cubemap` files are only a few KB
```bash
# Solution: Pull LFS files
git lfs pull
```

**Problem**: Push rejected due to large files
```bash
# Solution: Already configured! Files > 100MB use Git LFS
# Check .gitattributes: *.cubemap is tracked by LFS
```

### Unity Issues

**Problem**: Unity library not found in Android Studio
- ✅ Export Unity project to `UnityExports/PlanetXAR`
- ✅ Check `Assets/PlanetXNative/settings.gradle.kts` has correct relative path
- ✅ Verify `unityLibrary` folder exists at: `UnityExports/PlanetXAR/unityLibrary`

**Problem**: Assets not loading
- ✅ Run `git lfs pull` to download large assets
- ✅ Reimport assets: Assets > Reimport All

### Android Build Issues

**Problem**: `.meta` files causing build errors
- **Solution 1 (Recommended)**: Copy `PlanetXNative` outside Unity project and work from there
- **Solution 2**: Delete `.meta` files before building:
```bash
cd PlanetXNative  # Or wherever your Android project is
find app/src/main/res -name "*.meta" -delete
./gradlew clean
./gradlew assembleDebug
```

**Problem**: `TrajectoryGA.csproj` errors
```bash
# If you only want Justitia optimizer, the old TrajectoryGA files
# have been removed. Only TrajectoryJustitia.csproj should exist.
```

**Problem**: Gradle sync fails
- ✅ Check Android SDK/NDK installed
- ✅ Update Gradle: `./gradlew wrapper --gradle-version=8.13`
- ✅ Invalidate Caches: File > Invalidate Caches > Invalidate and Restart

### Python API Issues

**Problem**: Server won't start - port already in use
```bash
# Use different port
python api_server.py --port 8001
```

**Problem**: Ephemeris file (de421.bsp) not downloading
```bash
# Manually download from:
# https://naif.jpl.nasa.gov/pub/naif/generic_kernels/spk/planets/de421.bsp
# Place in Python script directory
```

### AR Issues

**Problem**: AR not working
- ✅ Test on **physical device** (not emulator - AR won't work)
- ✅ Verify device supports ARCore: https://developers.google.com/ar/devices
- ✅ Grant camera permissions
- ✅ Ensure good lighting conditions

**Problem**: Tracking lost
- ✅ Point camera at textured surface
- ✅ Move device slowly
- ✅ Avoid reflective/plain surfaces

### C# Trajectory Optimizer Issues

**Problem**: C# 9.0 language version error
```bash
# Already fixed! TrajectoryJustitia.csproj has:
# <LangVersion>latest</LangVersion>
```

**Problem**: .NET 9.0 not found
- ✅ Install .NET 9.0 SDK: https://dotnet.microsoft.com/download/dotnet/9.0
- ✅ Verify: `dotnet --version`

---

## 🎯 Features

### 📱 Android App (Kotlin + Compose)

1. **Dashboard Screen**
   - Real-time spacecraft telemetry
   - Mission progress tracking
   - System status monitoring

2. **Education Screen**
   - Interactive 3D spacecraft components
   - Mission information
   - Technical specifications

3. **Visualizer Screen**
   - AI-generated trajectory paths
   - Launch AR experience button
   - Mission planning tools

### 🌌 Unity AR Experience

- Immersive AR visualization of space mission
- Real-time planetary positions
- 3D trajectory rendering
- Solar system scale models
- ARCore integration for mobile

### 🐍 Python API Server

- RESTful API for planetary positions
- JPL DE421 ephemeris calculations
- Heliocentric coordinate system (ICRF)
- Date range: 1900-2053
- Interactive docs: http://localhost:8000/docs

### 🧬 C# Trajectory Optimizer

- Genetic algorithm for optimal paths
- Multi-objective optimization
- Hohmann transfer calculations
- CSV output for visualization
- .NET 9.0 performance

---

## 📦 Important Files & Directories

### Don't Commit
- `Assets/Scripts/python/.myenv/` - Python virtual environment (in .gitignore)
- `PlanetXNative/app/build/` - Android build artifacts (in .gitignore)
- `PlanetXNative/**/*.meta` - Unity meta files for Android project (in .gitignore)
- `PlanetXNative/.gradle/` - Gradle cache (in .gitignore)
- `PlanetXNative/.idea/` - Android Studio IDE files (in .gitignore)
- `PlanetXNative/local.properties` - Local Android SDK paths (in .gitignore)
- `Library/` - Unity cache

### Tracked by Git LFS
- `Assets/Real Stars Skybox/**/*.cubemap` - Large skybox files (192 MB each)
- `Assets/Scripts/python/*.bsp` - Ephemeris data files

### Key Configuration Files
- `.gitignore` - Git ignore patterns
- `.gitattributes` - Git LFS tracking
- `Assets/PlanetXNative/settings.gradle.kts` - Android project settings
- `Assets/Scripts/PathOptimization/GA/TrajectoryJustitia.csproj` - C# project

---

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

---

## 📄 License

This project is part of an educational initiative for space mission visualization.

---

## 🙏 Acknowledgments

- **NASA JPL** - DE421 Ephemeris data
- **Unity Technologies** - Unity Engine & ARCore
- **Google ARCore** - AR Framework
- **Skyfield** - Astronomical calculations
- **FastAPI** - Python API framework

---

## 📞 Support

If you encounter issues:
1. Check the **Troubleshooting** section above
2. Review API documentation: `Assets/Scripts/python/API_README.md`
3. Check Unity console for errors
4. Review Android Logcat in Android Studio

---

**Ready to explore space! 🌌**

*Built with 💙 for space exploration and education*

