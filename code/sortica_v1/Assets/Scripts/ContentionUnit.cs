using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ContentionUnit : MonoBehaviour
{
    [Header("Expected Sequences")]
    public MaterialType[] level1Sequence; // FIFO
    public MaterialType[] level2Sequence; // LIFO
    
    [Header("Visual Feedback")]
    public Renderer unitRenderer;
    public Color correctColor = Color.green;
    public Color errorColor = Color.red;
    public Color neutralColor = Color.white;
    public float feedbackDuration = 1f;
    
    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip errorSound;
    
    [Header("Building Progression")]
    public GameObject[] buildingStages;
    public GameObject completedBuilding;
    
    [Header("Progress UI - World Space")]
    public Canvas worldSpaceCanvas;
    public Image progressFillBar;
    public TextMeshProUGUI progressText;
    
    private MaterialType[] currentExpectedSequence;
    private int currentStep = 0;
    private string currentLevel;
    private Color originalColor;
    private bool isBuildingComplete = false;
    
    void Start()
    {
        if (unitRenderer != null)
        {
            originalColor = unitRenderer.material.color;
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Ensure canvas is set to world space
        if (worldSpaceCanvas != null)
        {
            worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
        }
    }
    
    public void InitializeForLevel(string level)
    {
        currentLevel = level;
        currentStep = 0;
        isBuildingComplete = false;
        
        // Set the expected sequence based on level
        currentExpectedSequence = level == "level1" ? level1Sequence : level2Sequence;
        
        // Reset visual state
        if (unitRenderer != null)
        {
            unitRenderer.material.color = neutralColor;
        }
        
        // Hide all building stages
        foreach (GameObject stage in buildingStages)
        {
            if (stage != null)
                stage.SetActive(false);
        }
        
        // Hide completed building at start
        if (completedBuilding != null)
        {
            completedBuilding.SetActive(true);
        }
        
        // Reset progress bar
        UpdateProgressBar();
        
        // Convert sequence to string array for logging
        string[] sequenceStrings = System.Array.ConvertAll(currentExpectedSequence, x => x.ToString());
        
        AnalyticsLogger.Instance.LogEvent("contentionInitialized", new 
        { 
            level = level,
            expectedSequence = sequenceStrings,
            algorithm = level == "level1" ? "FIFO" : "LIFO"
        });
    }
    
    void OnTriggerEnter(Collider other)
    {
        
        MatterBall matterBall = other.GetComponent<MatterBall>();
        if (matterBall == null || isBuildingComplete)
            return;
        
        MaterialType droppedType = matterBall.materialType;
        MaterialType expectedType = currentStep < currentExpectedSequence.Length 
            ? currentExpectedSequence[currentStep] 
            : MaterialType.Terrain;
        
        if (droppedType == expectedType)
        {
            // Correct matter dropped
            OnCorrectMatter(droppedType, currentStep);
            
            // Update building progression BEFORE incrementing step
            UpdateBuildingStage(currentStep);
            
            currentStep++;
            
            // Show correct feedback
            StartCoroutine(ShowFeedback(correctColor, correctSound));
            
            // Update progress bar
            UpdateProgressBar();
            
            // Absorb/destroy the matter ball
            Destroy(matterBall.gameObject);
            
            // Check if building is complete
            if (currentStep >= currentExpectedSequence.Length)
            {
                OnBuildingComplete();
            }
        }
        else
        {
            // Wrong matter dropped - stays in world
            OnWrongMatter(droppedType, expectedType, currentStep);
            
            // Show error feedback
            StartCoroutine(ShowFeedback(errorColor, errorSound));
        }
    }
    
    void UpdateBuildingStage(int stageIndex)
    {
        // Show the building stage corresponding to this step
        // All previous stages remain visible (cumulative)
        
        if (stageIndex >= 0 && stageIndex < buildingStages.Length)
        {
            if (buildingStages[stageIndex] != null)
            {
                buildingStages[stageIndex].SetActive(true);
                
                AnalyticsLogger.Instance.LogEvent("buildingStageRevealed", new 
                { 
                    level = currentLevel,
                    stage = stageIndex,
                    stageName = currentExpectedSequence[stageIndex].ToString(),
                    visibleStages = stageIndex + 1
                });
                
                Debug.Log($"Building stage {stageIndex + 1}/{buildingStages.Length} revealed: {currentExpectedSequence[stageIndex]}");
            }
        }
    }
    
    void UpdateProgressBar()
    {
        float progress = currentStep / (float)currentExpectedSequence.Length;
        
        if (progressFillBar != null)
        {
            progressFillBar.fillAmount = progress;
            
            // Optional: Change color based on progress
            progressFillBar.color = Color.Lerp(Color.yellow, correctColor, progress);
        }
        
        if (progressText != null)
        {
            progressText.text = $"{currentStep}/{currentExpectedSequence.Length}";
        }
    }
    
    IEnumerator ShowFeedback(Color color, AudioClip sound)
    {
        // Change color
        if (unitRenderer != null)
        {
            unitRenderer.material.color = color;
        }
        
        // Play sound
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
        
        // Wait
        yield return new WaitForSeconds(feedbackDuration);
        
        // Return to progress color
        if (unitRenderer != null)
        {
            unitRenderer.material.color = currentStep > 0 ? correctColor : neutralColor;
        }
    }
    
    void OnCorrectMatter(MaterialType droppedType, int step)
    {
        AnalyticsLogger.Instance.LogEvent("contentionCorrect", new 
        { 
            level = currentLevel,
            materialType = droppedType.ToString(),
            expectedMaterial = droppedType.ToString(),
            buildingStep = step,
            totalSteps = currentExpectedSequence.Length,
            algorithm = currentLevel == "level1" ? "FIFO" : "LIFO",
            progressPercentage = ((step + 1) / (float)currentExpectedSequence.Length) * 100
        });
        
        Debug.Log($"✓ Correct! Step {step + 1}/{currentExpectedSequence.Length} - {droppedType} placed");
    }
    
    void OnWrongMatter(MaterialType droppedType, MaterialType expectedType, int step)
    {
        AnalyticsLogger.Instance.LogEvent("contentionError", new 
        { 
            level = currentLevel,
            droppedMaterial = droppedType.ToString(),
            expectedMaterial = expectedType.ToString(),
            buildingStep = step,
            totalSteps = currentExpectedSequence.Length,
            algorithm = currentLevel == "level1" ? "FIFO" : "LIFO"
        });
        
        Debug.Log($"✗ Wrong! Expected {expectedType} but got {droppedType}");
    }
    
    void OnBuildingComplete()
    {
        isBuildingComplete = true;
        
        AnalyticsLogger.Instance.LogEvent("buildingComplete", new 
        { 
            level = currentLevel,
            totalSteps = currentExpectedSequence.Length,
            algorithm = currentLevel == "level1" ? "FIFO" : "LIFO"
        });
        
        // Keep all building stages visible, but now show the complete grabbable version
        if (completedBuilding != null)
        {
            completedBuilding.SetActive(true);
            
            // Make building grabbable
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = completedBuilding.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnBuildingGrabbed);
            }
        }
        
        // Fill bar to 100%
        UpdateProgressBar();
        
        Debug.Log($"🎉 Building complete! Player can now grab and place it in the street.");
    }
    
    void OnBuildingGrabbed(SelectEnterEventArgs args)
    {
        AnalyticsLogger.Instance.LogEvent("buildingGrabbed", new 
        { 
            level = currentLevel
        });
    }
    
    public int GetCurrentStep()
    {
        return currentStep;
    }
    
    public int GetTotalSteps()
    {
        return currentExpectedSequence.Length;
    }
    
    public float GetProgressPercentage()
    {
        return (currentStep / (float)currentExpectedSequence.Length) * 100f;
    }
    
    public bool IsBuildingComplete()
    {
        return isBuildingComplete;
    }
}