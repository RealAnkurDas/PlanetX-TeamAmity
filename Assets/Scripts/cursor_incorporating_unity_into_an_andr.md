# Incorporating Unity into an Android app
_Exported on 11/7/2025 at 21:17:23 GMT+4 from Cursor (2.0.60)_

---

**User**

https://docs.unity3d.com/6000.2/Documentation/Manual/UnityasaLibrary-Android.html REfer this and tell me if i want to incoroporate unity in an anderoid app how would i do it?

---

**Cursor**

Based on the Unity documentation you've provided, here's how you can incorporate Unity into an Android app using the **Unity as a Library** feature:

## Overview

Unity as a Library allows you to embed Unity-powered features (like 3D/2D rendering, AR experiences, model interactions) into an existing Android application. Unity runs as a module within your Android app rather than as a standalone application.

## How It Works

When Unity builds an Android Gradle project, it creates:
1. **unityLibrary module** - The core Unity runtime and Player data (this is what you'll integrate)
2. **launcher module** - A simple launcher app (you can replace this with your own app)

## Integration Steps

### 1. **Build Your Unity Project for Android**
   - In Unity, build your project as a Gradle project (not APK)
   - This generates the Android Gradle project structure with the `unityLibrary` module

### 2. **Include unityLibrary in Your Android Project**
   - In your Android app's `settings.gradle` file, include the Unity library module:
   ```gradle
   include ':unityLibrary'
   project(':unityLibrary').projectDir = new File('path/to/your/unity/build/unityLibrary')
   ```

### 3. **Add Dependency**
   - In your app's `build.gradle`, add the dependency:
   ```gradle
   dependencies {
       implementation project(':unityLibrary')
   }
   ```

### 4. **Control Unity from Your Android Code**
   - Use **Intents** to launch Unity activities
   - Use the **UnityPlayer Java API** for more direct control
   - Implement **IUnityPlayerLifecycleEvents** to handle Unity lifecycle events:
     - `onUnityPlayerUnloaded()` - Called when Unity pauses/unloads
     - `onUnityPlayerQuitted()` - Called when Unity quits completely

## Important Limitations

⚠️ Be aware of these constraints:
- **Only full-screen rendering** is supported (unless you're a Unity Industry customer)
- **Only one Unity runtime instance** can run at a time
- **Not compatible with Xamarin**
- Third-party plugins may need adaptation
- Cannot use Play Feature Delivery for dynamic modules

## Additional Resources

The documentation mentions there's a **repository with example projects** that demonstrates integration (though the specific link isn't in the excerpt you provided). I'd recommend searching for "Unity as a Library Android example" on Unity's GitHub.

## For Your AR Project

Given that your project is "Planet X - AR" with AR functionality, this approach would let you:
- Embed your solar system AR experience into a larger Android app
- Control when Unity loads/unloads
- Combine Unity's AR features with native Android UI and functionality

Would you like help with any specific aspect of this integration, such as setting up the Gradle configuration or handling the Unity lifecycle in your Android code?

```plaintext
   include ':unityLibrary'
   project(':unityLibrary').projectDir = new File('path/to/your/unity/build/unityLibrary')
```

```plaintext
   dependencies {
       implementation project(':unityLibrary')
   }
```

---

**User**

will it work with react native?

---

**Cursor**

**Yes, it can work with React Native, but with additional complexity!**

While the Unity documentation mentions it's **not compatible with Xamarin**, React Native integration **is possible** but requires extra work. Here's what you need to know:

## Integration Approaches

### 1. **Using React Native Unity Libraries** (Easier)
There are community-maintained packages like `react-native-unity-view` that bridge Unity and React Native:
- Provides a React Native component to display Unity content
- Handles the native module bridging for you
- Supports both iOS and Android

```jsx
// Example usage (conceptual)
import UnityView from 'react-native-unity-view';

<UnityView style={{flex: 1}} />
```

### 2. **Manual Integration** (More Control)
You can manually create a native module bridge:
- Export Unity as a library (following the Unity as a Library process)
- Create a React Native Native Module that wraps the Unity integration
- Write native code (Java/Kotlin for Android, Objective-C/Swift for iOS) to bridge communication between React Native and Unity

## Key Challenges

⚠️ **Additional Complexity:**
- **Native Bridge Required**: React Native apps need a native module to communicate with Unity
- **Message Passing**: You'll need to set up bidirectional communication between React Native JS and Unity C#
- **Performance**: Adding Unity to React Native adds overhead - your AR experience might impact app performance
- **Maintenance**: You're adding another layer (React Native → Native Bridge → Unity) which increases complexity

## For Your AR Project

Since your "Planet X - AR" project is Unity-based with AR features, consider:

1. **Full Unity App** - Keep it as a standalone Unity app (simpler, better performance)
2. **React Native + Unity** - Only if you need React Native for other parts of your app (like user authentication, social features, etc.)

## Recommendation

If you're building a primarily AR experience (which your solar system project appears to be), staying with a **native Unity Android app** would be simpler and more performant. Only integrate with React Native if you have compelling reasons to use React Native for other app features.

Would you like more details on any specific integration approach?

```javascriptreact
// Example usage (conceptual)
import UnityView from 'react-native-unity-view';

<UnityView style={{flex: 1}} />
```

---

**User**

but then what is the normal way to do it according to the docs? what to use for making the app

---

**Cursor**

Good question! Let me clarify - **Unity as a Library is NOT the normal way**. It's a special feature for a specific use case. Here's the breakdown:

## The Normal Way (Standard Unity Android Development)

### **Direct Unity Build to Android APK/AAB**

For most Unity projects (including AR apps like yours), you simply:

1. **Build directly from Unity Editor**:
   - File → Build Settings → Android
   - Click "Build" or "Build And Run"
   - Unity creates a standalone Android APK or AAB (Android App Bundle)

2. **What you get**:
   - A complete, self-contained Android application
   - No need for Android Studio or native code
   - Everything runs in Unity

3. **Publishing**:
   - Upload the APK/AAB directly to Google Play Store
   - Users install it like any other Android app

### **When to Use This (Normal Way)**
✅ Your entire app is made in Unity  
✅ You're making a game or AR experience (like your Planet X project)  
✅ You don't need to integrate with existing native Android UI  
✅ **This is what 99% of Unity Android apps do**

---

## Unity as a Library (Special Use Case)

### **When to Use This**
Only use Unity as a Library when:
- ❌ You have an **existing native Android app** (written in Java/Kotlin)
- ❌ You want to add Unity features to **part** of that existing app
- ❌ You need native Android UI alongside Unity content

### **Example Scenarios**
- An e-commerce app that wants to add a 3D product viewer
- A real estate app that wants to add AR room visualization
- A social media app that wants to embed 3D mini-games

---

## For Your "Planet X - AR" Project

**You should use the NORMAL way:**

```
Unity Editor 
    ↓
Build Settings → Android
    ↓
Build APK/AAB
    ↓
Upload to Play Store
```

**Don't use Unity as a Library** unless you specifically need to embed your solar system AR experience into an existing Android app written in native code.

### Building Your App (The Normal Process)

1. **In Unity Editor**:
   - Open Build Settings (Ctrl+Shift+B)
   - Select Android platform
   - Configure Player Settings (app name, icon, package name)
   - Click "Build" → Choose output location
   - Get your `.apk` or `.aab` file

2. **Testing**:
   - Connect Android device via USB
   - Click "Build and Run" to install directly

3. **Distribution**:
   - Upload to Google Play Console
   - Done!

---

Does this clarify things? Your AR solar system project should follow the **normal Unity Android build process**, not Unity as a Library! 🚀

```plaintext
Unity Editor 
    ↓
Build Settings → Android
    ↓
Build APK/AAB
    ↓
Upload to Play Store
```

---

**User**

native android app means java or kotlin?

---

**Cursor**

**Yes! "Native Android app" means an app written in Java and/or Kotlin.**

## Native Android Development

**Native Android** refers to apps built using:
- **Java** (the original Android language, still widely used)
- **Kotlin** (Google's preferred modern language for Android since 2019)
- Both are compiled to run natively on Android devices
- Developed using **Android Studio** (the official IDE)
- Uses Android SDK directly

### Example:
```kotlin
// Native Android code (Kotlin)
class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }
}
```

```java
// Native Android code (Java)
public class MainActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }
}
```

---

## Comparison Chart

| **Type** | **Languages** | **IDE** | **Use Case** |
|----------|---------------|---------|--------------|
| **Native Android** | Java, Kotlin | Android Studio | Traditional Android apps, business apps, UI-heavy apps |
| **Unity** | C# | Unity Editor | Games, 3D apps, AR/VR experiences |
| **React Native** | JavaScript/TypeScript | VS Code, etc. | Cross-platform apps (iOS + Android) |
| **Flutter** | Dart | VS Code, Android Studio | Cross-platform apps (iOS + Android) |

---

## So to Recap

When Unity docs say **"Unity as a Library"** lets you integrate into a **native Android app**, they mean:
- An app written in **Java** or **Kotlin**
- Built with **Android Studio**
- Using standard Android UI components (Activities, Fragments, Views)

Your Unity project (written in C#) would be **embedded as a module** into that Java/Kotlin app.

---

**For your Planet X AR project**: You're building everything in Unity (C#), so you're NOT building a native Android app - you're building a **Unity app that exports to Android**. No Java/Kotlin needed! 🎯

```plaintext
// Native Android code (Kotlin)
class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }
}
```

```java
// Native Android code (Java)
public class MainActivity extends AppCompatActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }
}
```

---

**User**

okay I wanna try with the native kotlin method. help me do everything from stractch assuming i haave unirtyscene ready and build is for android apk

---

**Cursor**

Great! Let me walk you through integrating your Unity AR project into a native Kotlin Android app from scratch. This is a comprehensive process, so follow along carefully.

## Part 1: Export Unity Project as a Library

### Step 1: Prepare Unity Project for Android

1. **Open your Unity project** (Planet X - AR)

2. **Go to Build Settings**:
   - File → Build Settings
   - Select **Android** platform
   - Click **"Switch Platform"** if not already on Android

3. **Configure Player Settings**:
   - Click **"Player Settings"** button
   - Set these important settings:
     - **Company Name**: `com.yourcompany`
     - **Product Name**: `PlanetXAR`
     - **Package Name**: `com.yourcompany.planetxar`
     - **Minimum API Level**: API Level 24 (Android 7.0) or higher
     - **Target API Level**: Latest available (API 33+)

4. **Configure Build Settings**:
   - In Build Settings window, check **"Export Project"** ✅
   - This is crucial - it exports as Gradle project instead of APK
   - Click **"Export"**
   - Choose a location (e.g., `C:\UnityExports\PlanetXAR`)
   - Click **"Save"** and wait for export to complete

### Step 2: Verify Export Structure

After export, you should have this structure:
```
PlanetXAR/
├── launcher/           (simple launcher - you'll replace this)
├── unityLibrary/       (THIS IS WHAT YOU NEED)
│   ├── src/
│   ├── libs/
│   ├── build.gradle
│   └── ...
├── build.gradle
├── settings.gradle
└── gradle/
```

---

## Part 2: Create Native Kotlin Android App

### Step 1: Create New Android Studio Project

1. **Open Android Studio**
2. **Create New Project**:
   - Select **"Empty Activity"**
   - Click **"Next"**

3. **Configure Project**:
   - **Name**: `PlanetX Native`
   - **Package name**: `com.yourcompany.planetxnative`
   - **Save location**: `C:\AndroidProjects\PlanetXNative`
   - **Language**: **Kotlin** ✅
   - **Minimum SDK**: **API 24** (same as Unity)
   - Click **"Finish"**

### Step 2: Project Structure

Your new project structure:
```
PlanetXNative/
├── app/
│   ├── src/
│   │   └── main/
│   │       ├── java/com/yourcompany/planetxnative/
│   │       │   └── MainActivity.kt
│   │       ├── AndroidManifest.xml
│   │       └── res/
│   └── build.gradle
├── build.gradle
└── settings.gradle
```

---

## Part 3: Integrate Unity Library into Kotlin App

### Step 1: Include Unity Library Module

**Edit `settings.gradle` (Project level)**:

```gradle
pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
        flatDir {
            dirs "${project(':unityLibrary').projectDir}/libs"
        }
    }
}

rootProject.name = "PlanetX Native"
include ':app'

// ADD THESE LINES TO INCLUDE UNITY LIBRARY
include ':unityLibrary'
project(':unityLibrary').projectDir = file('C:\\UnityExports\\PlanetXAR\\unityLibrary')
```

**Important**: Update the path to match where you exported Unity project!

### Step 2: Add Unity Dependency

**Edit `app/build.gradle` (Module level)**:

```gradle
plugins {
    id 'com.android.application'
    id 'org.jetbrains.kotlin.android'
}

android {
    namespace 'com.yourcompany.planetxnative'
    compileSdk 34

    defaultConfig {
        applicationId "com.yourcompany.planetxnative"
        minSdk 24
        targetSdk 34
        versionCode 1
        versionName "1.0"

        // ADD THIS - Required for Unity
        ndk {
            abiFilters 'armeabi-v7a', 'arm64-v8a'
        }
    }

    buildTypes {
        release {
            minifyEnabled false
            proguardFiles getDefaultProguardFile('proguard-android-optimize.txt'), 'proguard-rules.pro'
        }
    }
    compileOptions {
        sourceCompatibility JavaVersion.VERSION_1_8
        targetCompatibility JavaVersion.VERSION_1_8
    }
    kotlinOptions {
        jvmTarget = '1.8'
    }
}

dependencies {
    implementation 'androidx.core:core-ktx:1.12.0'
    implementation 'androidx.appcompat:appcompat:1.6.1'
    implementation 'com.google.android.material:material:1.11.0'
    implementation 'androidx.constraintlayout:constraintlayout:2.1.4'
    
    // ADD THIS - Unity Library dependency
    implementation project(':unityLibrary')
}
```

### Step 3: Sync Gradle

Click **"Sync Now"** in Android Studio. If successful, you'll see `unityLibrary` in your project structure.

---

## Part 4: Create Kotlin Code to Launch Unity

### Step 1: Create Unity Activity

Create new file: `app/src/main/java/com/yourcompany/planetxnative/UnityHolderActivity.kt`

```kotlin
package com.yourcompany.planetxnative

import android.os.Bundle
import android.widget.FrameLayout
import androidx.appcompat.app.AppCompatActivity
import com.unity3d.player.UnityPlayer

class UnityHolderActivity : AppCompatActivity() {
    
    private var unityPlayer: UnityPlayer? = null
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        
        // Create UnityPlayer instance
        unityPlayer = UnityPlayer(this)
        
        // Add Unity view to activity
        val container = FrameLayout(this)
        container.addView(
            unityPlayer,
            FrameLayout.LayoutParams.MATCH_PARENT,
            FrameLayout.LayoutParams.MATCH_PARENT
        )
        setContentView(container)
    }
    
    override fun onResume() {
        super.onResume()
        unityPlayer?.resume()
    }
    
    override fun onPause() {
        super.onPause()
        unityPlayer?.pause()
    }
    
    override fun onDestroy() {
        unityPlayer?.destroy()
        super.onDestroy()
    }
    
    override fun onLowMemory() {
        super.onLowMemory()
        unityPlayer?.lowMemory()
    }
    
    override fun onBackPressed() {
        unityPlayer?.quit()
        finish()
    }
}
```

### Step 2: Create Main Activity with Button

**Edit `app/src/main/java/com/yourcompany/planetxnative/MainActivity.kt`**:

```kotlin
package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity

class MainActivity : AppCompatActivity() {
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        val launchUnityButton = findViewById<Button>(R.id.launchUnityButton)
        launchUnityButton.setOnClickListener {
            // Launch Unity AR Experience
            val intent = Intent(this, UnityHolderActivity::class.java)
            startActivity(intent)
        }
    }
}
```

### Step 3: Create Layout

**Edit `app/src/main/res/layout/activity_main.xml`**:

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.constraintlayout.widget.ConstraintLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent">

    <TextView
        android:id="@+id/titleText"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Planet X - AR Experience"
        android:textSize="24sp"
        android:textStyle="bold"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toTopOf="@id/launchUnityButton"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

    <Button
        android:id="@+id/launchUnityButton"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Launch AR Solar System"
        android:textSize="18sp"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toBottomOf="parent"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

</androidx.constraintlayout.widget.ConstraintLayout>
```

---

## Part 5: Configure Android Manifest

**Edit `app/src/main/AndroidManifest.xml`**:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:tools="http://schemas.android.com/tools">

    <!-- Unity Required Permissions -->
    <uses-permission android:name="android.permission.INTERNET"/>
    <uses-permission android:name="android.permission.CAMERA"/>
    <uses-feature android:name="android.hardware.camera.ar" android:required="true"/>
    
    <application
        android:allowBackup="true"
        android:icon="@mipmap/ic_launcher"
        android:label="Planet X Native"
        android:supportsRtl="true"
        android:theme="@style/Theme.PlanetXNative"
        tools:targetApi="31">
        
        <!-- Main Activity -->
        <activity
            android:name=".MainActivity"
            android:exported="true">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>
        
        <!-- Unity Holder Activity -->
        <activity
            android:name=".UnityHolderActivity"
            android:label="Planet X AR"
            android:configChanges="mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density"
            android:hardwareAccelerated="true"
            android:screenOrientation="fullSensor"/>
        
    </application>

</manifest>
```

---

## Part 6: Build and Run

### Step 1: Clean and Rebuild
```
Build → Clean Project
Build → Rebuild Project
```

### Step 2: Connect Android Device
- Enable Developer Options and USB Debugging
- Connect via USB
- Select device in Android Studio

### Step 3: Run
- Click **Run** button (green play icon)
- App should install and launch
- Click "Launch AR Solar System" button
- Unity scene should load!

---

## Troubleshooting Common Issues

### Issue 1: "Unable to find unityLibrary"
- **Fix**: Verify path in `settings.gradle` is correct
- Use absolute path with double backslashes on Windows

### Issue 2: "Duplicate class" errors
- **Fix**: Add to `app/build.gradle`:
```gradle
configurations {
    all*.exclude group: 'com.google.android.gms', module: 'play-services-basement'
}
```

### Issue 3: Unity doesn't load
- **Fix**: Check `AndroidManifest.xml` has all Unity permissions
- Verify NDK ABI filters match in both Unity and app build.gradle

### Issue 4: Black screen
- **Fix**: Ensure Unity Player Settings has correct graphics API
- Try adding `android:hardwareAccelerated="true"` to activity

---

## Next Steps (Optional Advanced Features)

### Communicate from Kotlin to Unity:
```kotlin
// Send message to Unity GameObject
UnityPlayer.UnitySendMessage("GameObjectName", "MethodName", "parameter")
```

### Communicate from Unity to Kotlin:
Create a plugin in Unity that calls back to Android code.

---

Does this help? Let me know which step you're on and if you hit any issues! 🚀

```plaintext
PlanetXAR/
├── launcher/           (simple launcher - you'll replace this)
├── unityLibrary/       (THIS IS WHAT YOU NEED)
│   ├── src/
│   ├── libs/
│   ├── build.gradle
│   └── ...
├── build.gradle
├── settings.gradle
└── gradle/
```

```plaintext
PlanetXNative/
├── app/
│   ├── src/
│   │   └── main/
│   │       ├── java/com/yourcompany/planetxnative/
│   │       │   └── MainActivity.kt
│   │       ├── AndroidManifest.xml
│   │       └── res/
│   └── build.gradle
├── build.gradle
└── settings.gradle
```

```plaintext
pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
        flatDir {
            dirs "${project(':unityLibrary').projectDir}/libs"
        }
    }
}

rootProject.name = "PlanetX Native"
include ':app'

// ADD THESE LINES TO INCLUDE UNITY LIBRARY
include ':unityLibrary'
project(':unityLibrary').projectDir = file('C:\\UnityExports\\PlanetXAR\\unityLibrary')
```

```plaintext
plugins {
    id 'com.android.application'
    id 'org.jetbrains.kotlin.android'
}

android {
    namespace 'com.yourcompany.planetxnative'
    compileSdk 34

    defaultConfig {
        applicationId "com.yourcompany.planetxnative"
        minSdk 24
        targetSdk 34
        versionCode 1
        versionName "1.0"

        // ADD THIS - Required for Unity
        ndk {
            abiFilters 'armeabi-v7a', 'arm64-v8a'
        }
    }

    buildTypes {
        release {
            minifyEnabled false
            proguardFiles getDefaultProguardFile('proguard-android-optimize.txt'), 'proguard-rules.pro'
        }
    }
    compileOptions {
        sourceCompatibility JavaVersion.VERSION_1_8
        targetCompatibility JavaVersion.VERSION_1_8
    }
    kotlinOptions {
        jvmTarget = '1.8'
    }
}

dependencies {
    implementation 'androidx.core:core-ktx:1.12.0'
    implementation 'androidx.appcompat:appcompat:1.6.1'
    implementation 'com.google.android.material:material:1.11.0'
    implementation 'androidx.constraintlayout:constraintlayout:2.1.4'
    
    // ADD THIS - Unity Library dependency
    implementation project(':unityLibrary')
}
```

```plaintext
package com.yourcompany.planetxnative

import android.os.Bundle
import android.widget.FrameLayout
import androidx.appcompat.app.AppCompatActivity
import com.unity3d.player.UnityPlayer

class UnityHolderActivity : AppCompatActivity() {
    
    private var unityPlayer: UnityPlayer? = null
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        
        // Create UnityPlayer instance
        unityPlayer = UnityPlayer(this)
        
        // Add Unity view to activity
        val container = FrameLayout(this)
        container.addView(
            unityPlayer,
            FrameLayout.LayoutParams.MATCH_PARENT,
            FrameLayout.LayoutParams.MATCH_PARENT
        )
        setContentView(container)
    }
    
    override fun onResume() {
        super.onResume()
        unityPlayer?.resume()
    }
    
    override fun onPause() {
        super.onPause()
        unityPlayer?.pause()
    }
    
    override fun onDestroy() {
        unityPlayer?.destroy()
        super.onDestroy()
    }
    
    override fun onLowMemory() {
        super.onLowMemory()
        unityPlayer?.lowMemory()
    }
    
    override fun onBackPressed() {
        unityPlayer?.quit()
        finish()
    }
}
```

```plaintext
package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity

class MainActivity : AppCompatActivity() {
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        val launchUnityButton = findViewById<Button>(R.id.launchUnityButton)
        launchUnityButton.setOnClickListener {
            // Launch Unity AR Experience
            val intent = Intent(this, UnityHolderActivity::class.java)
            startActivity(intent)
        }
    }
}
```

```xml
<?xml version="1.0" encoding="utf-8"?>
<androidx.constraintlayout.widget.ConstraintLayout 
    xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:app="http://schemas.android.com/apk/res-auto"
    android:layout_width="match_parent"
    android:layout_height="match_parent">

    <TextView
        android:id="@+id/titleText"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Planet X - AR Experience"
        android:textSize="24sp"
        android:textStyle="bold"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toTopOf="@id/launchUnityButton"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

    <Button
        android:id="@+id/launchUnityButton"
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Launch AR Solar System"
        android:textSize="18sp"
        app:layout_constraintTop_toTopOf="parent"
        app:layout_constraintBottom_toBottomOf="parent"
        app:layout_constraintStart_toStartOf="parent"
        app:layout_constraintEnd_toEndOf="parent"/>

</androidx.constraintlayout.widget.ConstraintLayout>
```

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android"
    xmlns:tools="http://schemas.android.com/tools">

    <!-- Unity Required Permissions -->
    <uses-permission android:name="android.permission.INTERNET"/>
    <uses-permission android:name="android.permission.CAMERA"/>
    <uses-feature android:name="android.hardware.camera.ar" android:required="true"/>
    
    <application
        android:allowBackup="true"
        android:icon="@mipmap/ic_launcher"
        android:label="Planet X Native"
        android:supportsRtl="true"
        android:theme="@style/Theme.PlanetXNative"
        tools:targetApi="31">
        
        <!-- Main Activity -->
        <activity
            android:name=".MainActivity"
            android:exported="true">
            <intent-filter>
                <action android:name="android.intent.action.MAIN" />
                <category android:name="android.intent.category.LAUNCHER" />
            </intent-filter>
        </activity>
        
        <!-- Unity Holder Activity -->
        <activity
            android:name=".UnityHolderActivity"
            android:label="Planet X AR"
            android:configChanges="mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density"
            android:hardwareAccelerated="true"
            android:screenOrientation="fullSensor"/>
        
    </application>

</manifest>
```

```plaintext
Build → Clean Project
Build → Rebuild Project
```

```plaintext
configurations {
    all*.exclude group: 'com.google.android.gms', module: 'play-services-basement'
}
```

```plaintext
// Send message to Unity GameObject
UnityPlayer.UnitySendMessage("GameObjectName", "MethodName", "parameter")
```

