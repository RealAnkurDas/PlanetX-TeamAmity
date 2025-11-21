using UnityEngine;
using TMPro;

public class MBRPartInfoDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MBRPartSelector partSelector;
    [SerializeField] private TextMeshProUGUI infoText;
    
    [Header("Display Settings")]
    [SerializeField] private bool showPanel = true;
    [SerializeField] private bool useOnGUI = true;
    
    [Header("Panel Colors")]
    [SerializeField] private Color panelBackgroundColor = new Color(0, 0, 0, 0.7f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color titleColor = Color.cyan;
    
    private string currentPartName = "Overview";
    private string currentPartDescription = "Welcome to MBR Explorer spacecraft viewer.\n\nSelect a part from the dropdown to learn more.";
    
    void Start()
    {
        if (partSelector == null)
        {
            partSelector = FindFirstObjectByType<MBRPartSelector>();
            if (partSelector != null)
            {
                Debug.Log("MBRPartInfoDisplay: Found MBRPartSelector");
            }
        }
        
        if (partSelector != null)
        {
            partSelector.OnPartSelected += OnPartChanged;
        }
    }
    
    void OnDestroy()
    {
        if (partSelector != null)
        {
            partSelector.OnPartSelected -= OnPartChanged;
        }
    }
    
    void OnPartChanged(SpacecraftPartSetting part)
    {
        if (part == null)
        {
            currentPartName = "Overview";
            currentPartDescription = "Welcome to MBR Explorer spacecraft viewer.\n\nSelect a part from the dropdown to learn more about the spacecraft components.";
        }
        else
        {
            currentPartName = part.partName;
            currentPartDescription = part.partDescription;
        }
        
        if (infoText != null)
        {
            infoText.text = $"<b><size=120%>{currentPartName}</size></b>\n\n{currentPartDescription}";
        }
        
        Debug.Log($"MBRPartInfoDisplay: Updated info for '{currentPartName}'");
    }
    
    void OnGUI()
    {
        if (!showPanel || !useOnGUI) return;
        
        float scale = Screen.height / 800f;
        
        float panelHeight = Screen.height * 0.3f;
        float panelY = Screen.height * 0.7f;
        
        Texture2D panelTexture = MakeTex(2, 2, panelBackgroundColor);
        GUI.DrawTexture(new Rect(0, panelY, Screen.width, panelHeight), panelTexture);
        
        float padding = 20f * scale;
        
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = Mathf.RoundToInt(24 * scale);
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = titleColor;
        titleStyle.alignment = TextAnchor.UpperLeft;
        titleStyle.wordWrap = true;
        
        GUIStyle descStyle = new GUIStyle(GUI.skin.label);
        descStyle.fontSize = Mathf.RoundToInt(16 * scale);
        descStyle.fontStyle = FontStyle.Normal;
        descStyle.normal.textColor = textColor;
        descStyle.alignment = TextAnchor.UpperLeft;
        descStyle.wordWrap = true;
        
        float titleHeight = 40f * scale;
        float descriptionY = panelY + titleHeight + padding;
        float descriptionHeight = panelHeight - titleHeight - (padding * 2);
        
        GUI.Label(
            new Rect(padding, panelY + padding, Screen.width - (padding * 2), titleHeight),
            currentPartName,
            titleStyle
        );
        
        GUI.Label(
            new Rect(padding, descriptionY, Screen.width - (padding * 2), descriptionHeight),
            currentPartDescription,
            descStyle
        );
    }
    
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

