using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BuildingPlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    public float placementRadius = 2f;
    
    [Header("Visual Feedback")]
    public GameObject placementIndicator;
    
    private XRGrabInteractable grabInteractable;
    public bool isCompleted = false;
    private bool isPlaced = false;
    private string currentLevel;
    private float grabTime;
    
    
    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnBuildingGrabbed);
            grabInteractable.selectExited.AddListener(OnBuildingReleased);
        }
        
        // Determine current level
        MasterScript master = FindFirstObjectByType<MasterScript>();
        if (master != null)
        {
            currentLevel = master.game_state == GameStates.First_Level || master.game_state == GameStates.First_Finished 
                ? "level1" 
                : "level2";
        }
    }
    
    void OnBuildingGrabbed(SelectEnterEventArgs args)
    {
        grabTime = Time.time;
    }
    
    void OnBuildingReleased(SelectExitEventArgs args)
    {
        if (isPlaced)
            return;
        
        float holdDuration = Time.time - grabTime;
        
        if (isCompleted)
            {
            Animator animator = GetComponent<Animator>();
            animator.SetBool("final", true);
            // Disable grabbing
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }
            
            isPlaced = true;

            AnalyticsLogger.Instance.LogEvent("buildingPlaced", new BuildingPlacedData
            {
                level = currentLevel,
                holdDuration = holdDuration
            });

            //Debug.Log($"✓ Building placed in street! Level complete!");
            
            // Notify MasterScript that building was placed
            NotifyMasterScript();
        }
    }
    
    void NotifyMasterScript()
    {
        MasterScript master = FindFirstObjectByType<MasterScript>();
        if (master != null)
        {
            master.OnBuildingPlaced(currentLevel);
        }
    }
    
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnBuildingGrabbed);
            grabInteractable.selectExited.RemoveListener(OnBuildingReleased);
        }
    }
    
}