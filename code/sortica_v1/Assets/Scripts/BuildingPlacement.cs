using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BuildingPlacement : MonoBehaviour
{
    [Header("Placement Settings")]
    public Transform placementZone;
    public float placementRadius = 2f;
    
    [Header("Visual Feedback")]
    public GameObject placementIndicator;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    public bool isCompleted = false;
    private bool isPlaced = false;
    private string currentLevel;
    private float grabTime;
    
    
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
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
        if (isPlaced || placementZone == null)
            return;
        
        float holdDuration = Time.time - grabTime;
        
        // Check if building is close enough to placement zone
        float distance = Vector3.Distance(transform.position, placementZone.position);
        
        //if (distance <= placementRadius)
        if (isCompleted)
            {
            // Snap to placement zone
            transform.position = placementZone.position;
            transform.rotation = placementZone.rotation;
            
            // Disable grabbing
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }
            
            isPlaced = true;
            
            AnalyticsLogger.Instance.LogEvent("buildingPlaced", new 
            { 
                level = currentLevel,
                distanceFromTarget = distance,
                placementAccurate = distance <= placementRadius / 2,
                holdDuration = holdDuration
            });
            
            Debug.Log($"✓ Building placed in street! Level complete!");
            
            // Notify MasterScript that building was placed
            NotifyMasterScript();
        }
        //else
        //{
        //    AnalyticsLogger.Instance.LogEvent("buildingPlacementAttempt", new 
        //    { 
        //        level = currentLevel,
        //        distanceFromTarget = distance,
        //        tooFar = true,
        //        holdDuration = holdDuration
        //    });
            
        //    Debug.Log($"Building too far from placement zone. Distance: {distance:F2}m (needs {placementRadius}m)");
        //}
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
    
    // Optional: Show distance to placement zone while holding
    void Update()
    {
        if (grabInteractable != null && grabInteractable.isSelected && placementZone != null)
        {
            float distance = Vector3.Distance(transform.position, placementZone.position);
            
            // You could update a UI element here showing distance
            // Or change color of placement indicator based on distance
        }
    }
}