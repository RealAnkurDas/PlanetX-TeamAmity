using UnityEngine;

public class CustomInitialVelocity : MonoBehaviour
{
    public Vector3 initialVelocity = new Vector3(5, 0, 0);
    public bool useLocalSpace = true; // If true, velocity is relative to object's rotation
    
    void Start()
    {
        // Wait for the Orbit script's SetInitialVelocity to run, then override it
        Invoke(nameof(SetCustomVelocity), 0.01f);
    }
    
    void SetCustomVelocity()
    {
        Vector3 velocity = initialVelocity;
        
        // Convert to world space if using local space
        if (useLocalSpace)
        {
            velocity = transform.TransformDirection(initialVelocity);
        }
        
        GetComponent<Rigidbody>().linearVelocity = velocity;
    }
}