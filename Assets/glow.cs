using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PulsingGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    public Color glowColor = Color.yellow;  // Base emission color (can be HDR)
    public float minIntensity = 1f;         // Minimum glow
    public float maxIntensity = 10f;         // Maximum glow
    public float pulseSpeed = 2f;           // Speed of pulsing
    public float extraMultiplier = 2f;      // Extra multiplier to make emission really bright

    private Material mat;

    void Start()
    {
        // Use sharedMaterial to avoid creating copies
        mat = GetComponent<Renderer>().sharedMaterial;

        // Enable emission keyword for the shader
        mat.EnableKeyword("_EMISSION");

        // Optional: start with minimum intensity
        mat.SetColor("_EmissionColor", glowColor * minIntensity * extraMultiplier);
    }

    void Update()
    {
        // Smooth pulsing between min and max intensity using sine wave
        float lerpFactor = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // 0 -> 1
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, lerpFactor);

        // Apply emission color with extra multiplier for HDR effect
        mat.SetColor("_EmissionColor", glowColor * intensity * extraMultiplier);
    }
}
