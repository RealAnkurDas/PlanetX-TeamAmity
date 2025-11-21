# 🚀 Simple Velocity Launch (Direct vX, vY, vZ)

## ✅ **Simple Mode - Just 3 Numbers!**

No angles, no calculations. Just set the velocity components directly.

---

## 📱 **Kotlin Code (Super Simple)**

```kotlin
package com.yourcompany.planetxnative

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AppCompatActivity
import com.unity3d.player.UnityPlayerActivity

class MainActivity : AppCompatActivity() {
    
    private lateinit var editVelocityX: EditText
    private lateinit var editVelocityY: EditText
    private lateinit var editVelocityZ: EditText
    private lateinit var btnLaunch: Button
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        editVelocityX = findViewById(R.id.editVelocityX)
        editVelocityY = findViewById(R.id.editVelocityY)
        editVelocityZ = findViewById(R.id.editVelocityZ)
        btnLaunch = findViewById(R.id.btnLaunch)
        
        // Default values for Earth orbit
        editVelocityX.setText("0.01")
        editVelocityY.setText("0.0")
        editVelocityZ.setText("0.03")
        
        btnLaunch.setOnClickListener {
            launchUnitySimulation()
        }
    }
    
    private fun launchUnitySimulation() {
        // Read velocity values
        val vx = editVelocityX.text.toString().toFloatOrNull() ?: 0.01f
        val vy = editVelocityY.text.toString().toFloatOrNull() ?: 0.0f
        val vz = editVelocityZ.text.toString().toFloatOrNull() ?: 0.03f
        
        // Create Intent for Unity
        val intent = Intent(this, UnityPlayerActivity::class.java)
        
        // Send velocity components (NO launchMode needed, defaults to velocity)
        intent.putExtra("velocityX", vx)
        intent.putExtra("velocityY", vy)
        intent.putExtra("velocityZ", vz)
        
        startActivity(intent)
    }
}
```

---

## 🎨 **XML Layout (Super Simple)**

```xml
<?xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:orientation="vertical"
    android:padding="16dp">

    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="🚀 Launch Velocity"
        android:textSize="24sp"
        android:textStyle="bold"
        android:layout_marginBottom="20dp"/>

    <!-- Velocity X -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity X (m/s):"
        android:textSize="16sp"/>
    
    <EditText
        android:id="@+id/editVelocityX"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.01"/>

    <!-- Velocity Y -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity Y (m/s):"
        android:textSize="16sp"
        android:layout_marginTop="12dp"/>
    
    <EditText
        android:id="@+id/editVelocityY"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.0"/>

    <!-- Velocity Z -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Velocity Z (m/s):"
        android:textSize="16sp"
        android:layout_marginTop="12dp"/>
    
    <EditText
        android:id="@+id/editVelocityZ"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:inputType="numberDecimal|numberSigned"
        android:hint="0.03"/>

    <!-- Launch Button -->
    <Button
        android:id="@+id/btnLaunch"
        android:layout_width="match_parent"
        android:layout_height="60dp"
        android:text="🚀 LAUNCH"
        android:textSize="18sp"
        android:layout_marginTop="30dp"/>

    <!-- Helper Text -->
    <TextView
        android:layout_width="wrap_content"
        android:layout_height="wrap_content"
        android:text="Examples:\n• Earth orbit: (0.01, 0, 0.03)\n• Mars transfer: (0.01, 0, 0.035)\n• Escape: (0.02, 0, 0.05)"
        android:textSize="14sp"
        android:layout_marginTop="20dp"
        android:padding="12dp"
        android:background="#F5F5F5"/>
</LinearLayout>
```

---

## 📐 **Coordinate System**

- **X-axis**: Radial (toward/away from Sun)
- **Y-axis**: Perpendicular to orbital plane (north/south)
- **Z-axis**: Tangential (along orbital direction)

---

## 🎯 **Common Velocities**

### **Earth Orbit (Circular)**
```
vX = 0.01
vY = 0.0
vZ = 0.03
→ Stays near Earth, circular orbit
```

### **Mars Transfer**
```
vX = 0.01
vY = 0.0
vZ = 0.035
→ Reaches Mars orbit
```

### **High Elliptical**
```
vX = 0.02
vY = 0.0
vZ = 0.03
→ Very elongated orbit
```

### **Escape Trajectory**
```
vX = 0.02
vY = 0.0
vZ = 0.05
→ Leaves solar system
```

### **Polar Orbit**
```
vX = 0.01
vY = 0.02
vZ = 0.03
→ Inclined to orbital plane
```

---

## ✅ **That's It!**

**3 numbers, that's all you need:**
1. **vX** - How fast toward/away from Sun
2. **vY** - How fast up/down from orbital plane
3. **vZ** - How fast along orbital direction

**No angles, no calculations, just works!** 🚀✨

---

## 🎮 **Unity Setup**

1. Open **"Solar System Animated View 3D User"** scene
2. Select **"LaunchParameterReceiver"** in Hierarchy
3. In Inspector → **KotlinLaunchReceiver** component:
   - **Launch Mode**: Set to **"Direct Velocity"** ✅
   - **Launch From Earth**: Checked ✅
   - Done!

---

## 📊 **Velocity Range Guide**

| Component | Min | Max | Typical |
|-----------|-----|-----|---------|
| **vX** | -0.05 | 0.05 | 0.01 |
| **vY** | -0.03 | 0.03 | 0.0 |
| **vZ** | 0.0 | 0.08 | 0.03 |

---

**Copy-paste the Kotlin code and you're done!** 🎉

