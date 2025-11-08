# 🚀 Mission to Justitia - Unity + Android AR Project

An integrated Unity AR and Android native app project for visualizing a space mission to asteroid 269 Justitia.

## 📁 Project Structure

```
Planet X - AR/
├── Assets/
│   ├── PlanetXNative/          # Android native app (Kotlin + Compose)
│   ├── Scenes/                 # Unity scenes
│   ├── Scripts/                # Unity C# scripts
│   └── ...                     # Other Unity assets
├── UnityExports/
│   └── PlanetXAR/              # Unity Android library export
│       └── unityLibrary/       # Used by Android app
├── ProjectSettings/
└── Packages/
```

## 🚀 Quick Setup

### Prerequisites
- Unity 6 (6000.2.10f1+)
- Android Studio (latest)
- ARCore-compatible Android device

### 1. Unity Setup
1. Open this folder in Unity Hub
2. Let Unity import packages
3. File > Build Settings > Android
4. Export Project to: `UnityExports/PlanetXAR`

### 2. Android App Setup
1. Open `Assets/PlanetXNative` in Android Studio
2. Gradle sync will automatically find Unity library
3. Connect device and run

## 🔧 How It Works

- **Unity Project**: This root folder
- **Android App**: Lives in `Assets/PlanetXNative`
- **Integration**: Android app uses relative paths to find Unity exports
  ```kotlin
  // In settings.gradle.kts
  project(":unityLibrary").projectDir = 
    file("${rootDir}/../../UnityExports/PlanetXAR/unityLibrary")
  ```

## 📝 Development Workflow

1. **Make Unity changes**: Edit in Unity, export when ready
2. **Make Android changes**: Edit in Android Studio at `Assets/PlanetXNative`
3. **Test**: Build and run from Android Studio

## 🐛 Troubleshooting

**Unity library not found:**
- Ensure Unity is exported to `UnityExports/PlanetXAR`
- Check paths in `Assets/PlanetXNative/settings.gradle.kts`

**AR not working:**
- Test on physical device (not emulator)
- Verify device supports ARCore
- Grant camera permissions

## 🤝 Git Setup

```bash
# In this directory
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/yourusername/your-repo.git
git push -u origin main
```

## 🎯 Features

### Android App (Compose UI)
- 📊 Dashboard - Real-time spacecraft telemetry
- 📚 Education - Interactive 3D spacecraft components
- 🎯 Visualizer - AI trajectory paths + AR launcher

### Unity AR
- Immersive AR visualization of space mission
- 3D trajectory rendering
- ARCore integration

---

**Ready to explore space! 🌌**

