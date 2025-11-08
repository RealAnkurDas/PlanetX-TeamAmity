# Asteroid Mass Reference

## 🪨 **All Asteroid Masses**

| Asteroid | Diameter | Mass (kg) | Earth Masses | Scientific Notation |
|----------|----------|-----------|--------------|---------------------|
| **Justitia** | ~58 km | 140,000,000,000,000,000 | 2.3 × 10⁻⁸ | 1.4E17 |
| **Chimaera** | ~8 km | 210,000,000,000,000 | 3.5 × 10⁻¹¹ | 2.1E14 |
| **Rockox** | 5.24 km | 150,000,000,000,000 | 2.5 × 10⁻¹¹ | 1.5E14 |
| **Westerwald** | 2.27 km | 13,000,000,000,000 | 2.2 × 10⁻¹² | 1.3E13 |
| **Ousha** | ~3 km | 34,000,000,000,000 | 5.7 × 10⁻¹² | 3.4E13 |
| **Moza** | ~2.5 km | 16,000,000,000,000 | 2.7 × 10⁻¹² | 1.6E13 |
| **Ghaf** | ~2 km | 8,400,000,000,000 | 1.4 × 10⁻¹² | 8.4E12 |

---

## 📊 **Context**

- **Earth Mass**: 5.972 × 10²⁴ kg
- **Spacecraft (typical)**: ~10,000 kg (1.0 × 10⁴ kg)
- **ISS**: ~420,000 kg (4.2 × 10⁵ kg)

### **Mass Comparison:**
- **Justitia** is the largest at 1.4 × 10¹⁷ kg
- **Ghaf** is the smallest at 8.4 × 10¹² kg
- **Justitia** is ~16,000 times more massive than Ghaf

---

## 🔧 **How to Use**

### **In Code:**

The masses are automatically loaded from the database in `AsteroidEphemerisReader.cs`:

```csharp
// Get mass in kilograms
double massKg = asteroidReader.GetMassKg();

// Get mass in Earth masses
double earthMasses = asteroidReader.GetMassInEarthMasses();

// Static lookup by name
double justitiaMass = AsteroidEphemerisReader.GetAsteroidMass("Justitia");
```

### **Mass Database Location:**

File: `AsteroidEphemerisReader.cs`

```csharp
private static readonly Dictionary<string, double> asteroidMassDatabase = new Dictionary<string, double>
{
    { "Justitia", 1.4e17 },
    { "Chimaera", 2.1e14 },
    { "Rockox", 1.5e14 },
    { "Westerwald", 1.3e13 },
    { "Ousha", 3.4e13 },
    { "Moza", 1.6e13 },
    { "Ghaf", 8.4e12 }
};
```

---

## 📐 **Mass Calculation**

Masses estimated using:

```
Mass = (4/3) × π × r³ × density
```

Where:
- **r** = radius (half of diameter)
- **density** = 2,000-2,700 kg/m³ (typical for stony asteroids)

---

## 🚀 **For Future Spacecraft Integration**

When you add spacecraft to your simulation, you can compare:
- A spacecraft (10,000 kg) is negligible compared to even the smallest asteroid (8.4 × 10¹² kg)
- Asteroids will have gravitational influence on spacecraft
- Spacecraft will have essentially zero influence on asteroids

---

## ✅ **Automatic Setup**

Each `AsteroidEphemerisReader` component will automatically:
1. Look up the mass based on the `asteroidName` field
2. Set the `massKg` value
3. Log the mass in both kg and Earth masses on startup

**Example Console Output:**
```
AsteroidEphemeris (Justitia): Mass set to 1.40E+17 kg (2.34E-08 Earth masses)
```

