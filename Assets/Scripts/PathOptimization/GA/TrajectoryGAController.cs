using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple UI controller for the Trajectory GA
/// Attach to a GameObject in your scene
/// </summary>
public class TrajectoryGAController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TrajectoryGA trajectoryGA;
    
    [Header("UI (Optional)")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Text fitnessText;
    [SerializeField] private Text generationText;
    
    void Start()
    {
        // If no TrajectoryGA assigned, find or create one
        if (trajectoryGA == null)
        {
            trajectoryGA = FindFirstObjectByType<TrajectoryGA>();
            if (trajectoryGA == null)
            {
                GameObject gaObject = new GameObject("TrajectoryGA");
                trajectoryGA = gaObject.AddComponent<TrajectoryGA>();
            }
        }
        
        // Setup button callbacks if assigned
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClicked);
        }
        
        if (stopButton != null)
        {
            stopButton.onClick.AddListener(OnStopClicked);
        }
    }
    
    void Update()
    {
        // Update UI if assigned
        if (trajectoryGA != null)
        {
            if (statusText != null)
            {
                statusText.text = $"Status: {(trajectoryGA.isRunning ? "Running" : "Idle")}";
            }
            
            if (fitnessText != null)
            {
                fitnessText.text = $"Best Fitness: {trajectoryGA.bestFitness:F2}";
            }
            
            if (generationText != null)
            {
                generationText.text = $"Generation: {trajectoryGA.currentGeneration}";
            }
        }
    }
    
    public void OnStartClicked()
    {
        if (trajectoryGA != null)
        {
            Debug.Log("Starting Trajectory GA optimization...");
            trajectoryGA.StartOptimization();
        }
    }
    
    public void OnStopClicked()
    {
        if (trajectoryGA != null)
        {
            Debug.Log("Stopping Trajectory GA optimization...");
            trajectoryGA.StopOptimization();
        }
    }
    
    // Keyboard shortcuts
    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.G)
            {
                OnStartClicked();
            }
            else if (Event.current.keyCode == KeyCode.Escape)
            {
                OnStopClicked();
            }
        }
    }
}

