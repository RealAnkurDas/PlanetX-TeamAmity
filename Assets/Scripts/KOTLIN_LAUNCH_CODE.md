# 📱 Kotlin Launch Code (Updated for Angle & Speed Mode)

## 🎯 **Intuitive Launch System**

The launch now works like a real rocket:
- **Launch Speed**: How fast you launch (single number, easier!)
- **Horizontal Angle**: Direction in the plane perpendicular to Earth's motion (0-360°)
- **Vertical Angle**: Up/down tilt (-90° to +90°)
- **Base velocity**: Earth's orbital velocity is automatically added

---

## 📱 **Kotlin Code (MainActivity.kt)**

```kotlin
package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.unity3d.player.UnityPlayerActivity

class MainActivity : AppCompatActivity() {
    
    // UI elements
    private lateinit var editLaunchSpeed: EditText
    private lateinit var editHorizontalAngle: EditText
    private lateinit var editVerticalAngle: EditText
    private lateinit var btnLaunchSimulation: Button
    private lateinit var txtInfo: TextView
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        // Initialize UI
        editLaunchSpeed = findViewById(R.id.editLaunchSpeed)
        editHorizontalAngle = findViewById(R.id.editHorizontalAngle)
        editVerticalAngle = findViewById(R.id.editVerticalAngle)
        btnLaunchSimulation = findViewById(R.id.btnLaunchSimulation)
        txtInfo = findViewById(R.id.txtInfo)
        
        // Set default values
        editLaunchSpeed.setText("0.005")    // 5 km/s relative to Earth
        editHorizontalAngle.setText("0")     // Forward (along Earth's motion)
        editVerticalAngle.setText("0")       // Horizontal launch
        
        // Launch button click
        btnLaunchSimulation.setOnClickListener {
            launchUnitySimulation()
        }
        
        // Preset buttons
        findViewById<Button>(R.id.btnPresetEarthOrbit).setOnClickListener {
            setPreset(0.002f, 0f, 0f, "Circular Earth Orbit")
        }
        
        findViewById<Button>(R.id.btnPresetMarsTransfer).setOnClickListener {
            setPreset(0.005f, 0f, 0f, "Mars Transfer Orbit")
        }
        
        findViewById<Button>(R.id.btnPresetEscape).setOnClickListener {
            setPreset(0.015f, 0f, 0f, "Escape Trajectory")
        }
        
        findViewById<Button>(R.id.btnPresetVertical).setOnClickListener {
            setPreset(0.01f, 0f, 90f, "Straight Up")
        }
    }
    
    private fun setPreset(speed: Float, hAngle: Float, vAngle: Float, name: String) {
        editLaunchSpeed.setText(speed.toString())
        editHorizontalAngle.setText(hAngle.toString())
        editVerticalAngle.setText(vAngle.toString())
        txtInfo.text = "Preset: $name"
    }
    
    private fun launchUnitySimulation() {
        // Read values from UI (with fallbacks)
        val speed = editLaunchSpeed.text.toString().toFloatOrNull() ?: 0.005f
        val hAngle = editHorizontalAngle.text.toString().toFloatOrNull() ?: 0f
        val vAngle = editVerticalAngle.text.toString().toFloatOrNull() ?: 0f
        
        // Validate inputs
        if (speed < 0) {
            android.widget.Toast.makeText(
                this,
                "Speed must be positive!",
                android.widget.Toast.LENGTH_SHORT
            ).show()
            return
        }
        
        if (speed > 0.05f) {
            android.widget.Toast.makeText(
                this,
                "Warning: Very high speed! (>${speed * 1000} km/s)",
                android.widget.Toast.LENGTH_LONG
            ).show()
        }
        
        // Create Intent for Unity
        val intent = Intent(this, UnityPlayerActivity::class.java)
        
        // Pass parameters via Intent extras (ANGLE & SPEED MODE)
        intent.putExtra("launchMode", "angleAndSpeed")  // Important!
        intent.putExtra("launchSpeed", speed)
        intent.putExtra("horizontalAngle", hAngle)
        intent.putExtra("verticalAngle", vAngle)
        
        // Optional: Custom launch position (near Earth by default)
        // intent.putExtra("launchPosX", 150f)
        // intent.putExtra("launchPosY", 0f)
        // intent.putExtra("launchPosZ", 0f)
        
        startActivity(intent)
    }
}
```

---

## 🎨 **XML Layout (activity_main.xml)**

```xml
<?xml version="1.0" encoding="utf-8"?>
<ScrollView xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent">

    <LinearLayout
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:orientation="vertical"
        android:padding="16dp">

        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="🚀 Spacecraft Launch Control"
            android:textSize="24sp"
            android:textStyle="bold"
            android:layout_marginBottom="20dp"/>

        <!-- Launch Speed -->
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Launch Speed (m/s relative to Earth):"
            android:textSize="16sp"
            android:textStyle="bold"/>
        
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Examples: 0.002 (slow), 0.005 (Mars), 0.015 (escape)"
            android:textSize="12sp"
            android:textColor="#666"
            android:layout_marginBottom="4dp"/>
        
        <EditText
            android:id="@+id/editLaunchSpeed"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:inputType="numberDecimal|numberSigned"
            android:hint="0.005"/>

        <!-- Horizontal Angle -->
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Horizontal Angle (degrees):"
            android:textSize="16sp"
            android:textStyle="bold"
            android:layout_marginTop="16dp"/>
        
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="0°=Forward, 90°=Right, 180°=Back, 270°=Left"
            android:textSize="12sp"
            android:textColor="#666"
            android:layout_marginBottom="4dp"/>
        
        <EditText
            android:id="@+id/editHorizontalAngle"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:inputType="numberDecimal|numberSigned"
            android:hint="0"/>

        <!-- Vertical Angle -->
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Vertical Angle (degrees):"
            android:textSize="16sp"
            android:textStyle="bold"
            android:layout_marginTop="16dp"/>
        
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="0°=Horizontal, +90°=Straight up, -90°=Straight down"
            android:textSize="12sp"
            android:textColor="#666"
            android:layout_marginBottom="4dp"/>
        
        <EditText
            android:id="@+id/editVerticalAngle"
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:inputType="numberDecimal|numberSigned"
            android:hint="0"/>

        <!-- Launch Button -->
        <Button
            android:id="@+id/btnLaunchSimulation"
            android:layout_width="match_parent"
            android:layout_height="60dp"
            android:text="🚀 LAUNCH SIMULATION"
            android:textSize="18sp"
            android:textStyle="bold"
            android:layout_marginTop="30dp"
            android:backgroundTint="#4CAF50"/>

        <!-- Preset Buttons -->
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Quick Presets:"
            android:textSize="16sp"
            android:textStyle="bold"
            android:layout_marginTop="30dp"/>

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal"
            android:layout_marginTop="10dp">

            <Button
                android:id="@+id/btnPresetEarthOrbit"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="Earth\nOrbit"
                android:textSize="12sp"
                android:layout_margin="4dp"/>

            <Button
                android:id="@+id/btnPresetMarsTransfer"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="Mars\nTransfer"
                android:textSize="12sp"
                android:layout_margin="4dp"/>
        </LinearLayout>

        <LinearLayout
            android:layout_width="match_parent"
            android:layout_height="wrap_content"
            android:orientation="horizontal">

            <Button
                android:id="@+id/btnPresetEscape"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="Escape\nVelocity"
                android:textSize="12sp"
                android:layout_margin="4dp"/>

            <Button
                android:id="@+id/btnPresetVertical"
                android:layout_width="0dp"
                android:layout_height="wrap_content"
                android:layout_weight="1"
                android:text="Straight\nUp"
                android:textSize="12sp"
                android:layout_margin="4dp"/>
        </LinearLayout>

        <!-- Info Text -->
        <TextView
            android:id="@+id/txtInfo"
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="Ready to launch!"
            android:textSize="14sp"
            android:textStyle="italic"
            android:layout_marginTop="20dp"
            android:textColor="#2196F3"/>

        <!-- Help Text -->
        <TextView
            android:layout_width="wrap_content"
            android:layout_height="wrap_content"
            android:text="ℹ️ How it works:\n• Speed is added to Earth's motion (~30 km/s)\n• Horizontal angle rotates in plane ⊥ to Earth\n• Vertical angle tilts up/down from plane\n• Gravity simulation shows real trajectory!"
            android:textSize="12sp"
            android:layout_marginTop="30dp"
            android:padding="12dp"
            android:background="#F5F5F5"
            android:lineSpacingMultiplier="1.3"/>
    </LinearLayout>
</ScrollView>
```

---

## 📊 **Launch Speed Guide**

| Speed (m/s) | Real Speed | Description |
|-------------|------------|-------------|
| 0.001 | ~1 km/s | Very slow, orbit decays |
| 0.002 | ~2 km/s | Stable Earth orbit |
| 0.005 | ~5 km/s | Mars transfer |
| 0.010 | ~10 km/s | High elliptical orbit |
| 0.015 | ~15 km/s | Near escape |
| 0.020+ | ~20+ km/s | Definitely escapes |

---

## 🧭 **Angle Examples**

### **Horizontal Angle (in plane ⊥ Earth's motion):**
- **0°** = Forward (along Earth's direction)
- **90°** = Right (perpendicular to Earth)
- **180°** = Backward (opposite Earth's direction)
- **270°** = Left

### **Vertical Angle (up/down from plane):**
- **0°** = Horizontal launch (in the plane)
- **+45°** = Angled upward
- **+90°** = Straight up (perpendicular to plane)
- **-45°** = Angled downward
- **-90°** = Straight down

---

## 🎯 **Example Scenarios**

### **Scenario 1: Earth Orbit**
```
Speed: 0.002
H-Angle: 0°
V-Angle: 0°
→ Gentle launch forward, stays near Earth
```

### **Scenario 2: Mars Transfer**
```
Speed: 0.005
H-Angle: 0°
V-Angle: 0°
→ Faster launch forward, reaches Mars orbit
```

### **Scenario 3: Polar Orbit**
```
Speed: 0.005
H-Angle: 0°
V-Angle: +45°
→ Angled up, creates inclined orbit
```

### **Scenario 4: Sideways Launch**
```
Speed: 0.01
H-Angle: 90°
V-Angle: 0°
→ Launches perpendicular to Earth's motion
```

### **Scenario 5: Escape Upward**
```
Speed: 0.015
H-Angle: 0°
V-Angle: +90°
→ Launches straight "up" from plane, escapes
```

---

## ✅ **Key Points**

1. **Speed is relative to Earth** - Earth's 30 km/s orbital velocity is added automatically
2. **Launch plane is perpendicular to Earth's motion** - Like launching from Earth's surface
3. **Angles are intuitive** - Horizontal (compass) + Vertical (pitch)
4. **Spacecraft points in velocity direction** - Visual feedback!

---

**Copy this code into your Kotlin app!** 🚀

