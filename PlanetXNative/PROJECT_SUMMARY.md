# Mission to Justitia - Android App

## Overview
A comprehensive space mission simulation app featuring Unity AR integration for visualizing a journey from Earth to asteroid 269 Justitia.

## Features

### 📊 Dashboard Page
- **Real-time Spacecraft Telemetry**: Velocity, distance, altitude, attitude (pitch/roll/yaw), fuel reserves
- **Mission Status**: Current phase, progress tracking, next event countdown, ETA
- **Journey Minimap**: Visual representation of the spacecraft's position from Earth to Justitia
- **AI Insights & Threat Detection**: Real-time analysis and warnings
- **Spacecraft Health Monitoring**: Component-by-component health status with visual indicators

### 📚 Education Page
- **Interactive 3D Spacecraft Model**: Clickable components with detailed descriptions
  - Solar Panels
  - Main Engine
  - Antenna
  - Sensors
- **Mission Information**:
  - About the Program
  - Mission Objectives (Primary & Secondary)
  - Scientific Background on Asteroid Justitia
  - Technical Specifications
- **Educational Content**: Real scientific data about the asteroid and mission planning

### 🚀 Visualizer Page
- **AI Trajectory Optimization**: Top 5 pre-computed optimal paths using RL algorithms
  - Fuel-Efficient Path
  - Fastest Path
  - Balanced Path
  - Safe Path (Low Solar Activity)
  - Gravity-Assist Path
- **Launch Parameters**: Interactive sliders for launch velocity and angle
- **Mission Simulation Info**: Explanation of how the AI navigation works
- **AR Launch Button**: Opens Unity AR scene for immersive 3D/AR experience

## Navigation
- Bottom navigation bar for easy switching between Dashboard, Education, and Visualizer
- Top app bar with "Mission to Justitia" branding
- Material 3 design with modern, space-themed UI

## Unity Integration
- Unity 6 AR scene integrated via `UnityPlayerGameActivity`
- Launches from Visualizer page button
- Seamless transition between native Kotlin UI and Unity AR experience
- Configured for ARCore support

## Technical Stack
- **Language**: Kotlin
- **UI Framework**: Jetpack Compose
- **Navigation**: Navigation Compose
- **Unity**: Unity 6 (6000.2.10f1)
- **AR**: ARCore
- **Minimum SDK**: 30 (Android 11)
- **Target SDK**: 36

## Project Structure
```
app/src/main/java/com/yourcompany/planetxnative/
├── MainActivity.kt                 # Main entry point with navigation setup
├── UnityHolderActivity.kt         # Unity AR activity
├── data/
│   └── SpacecraftData.kt          # Data models and sample data
└── screens/
    ├── DashboardScreen.kt         # Mission control dashboard
    ├── EduScreen.kt               # Educational content
    └── VisualizerScreen.kt        # Path planning and AR launcher
```

## Build Instructions
1. Ensure Unity project is exported to: `C:\Users\ankur\Planet X - AR\UnityExports\PlanetXAR`
2. Sync Gradle files
3. Build and run on device with ARCore support

## Notes
- Uses placeholder/sample data for telemetry and mission status
- Educational content includes real scientific information about asteroid Justitia
- AI path optimization displays pre-calculated results (simulates real-time computation)
- Designed for tablet/phone in portrait orientation
- Requires camera permissions for AR functionality

