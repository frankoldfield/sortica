using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class VRMovementTracker : MonoBehaviour
{
    [Header("XR References")]
    public XROrigin xrOrigin;
    
    [Header("Performance Settings")]
    [Tooltip("Sample movement every N frames (e.g., 10 = check every 10th frame)")]
    public int sampleInterval = 10;
    
    private Transform headTransform;
    private Transform leftHandTransform;
    private Transform rightHandTransform;
    
    private Vector3 lastHeadPosition;
    private Vector3 lastLeftHandPosition;
    private Vector3 lastRightHandPosition;
    
    private Quaternion lastHeadRotation;
    private Quaternion lastLeftHandRotation;
    private Quaternion lastRightHandRotation;
    
    private float totalHeadDistance = 0f;
    private float totalLeftHandDistance = 0f;
    private float totalRightHandDistance = 0f;
    
    private float totalHeadRotation = 0f;
    private float totalLeftHandRotation = 0f;
    private float totalRightHandRotation = 0f;
    
    private int frameCount = 0;
    
    void Start()
    {
        if (xrOrigin == null)
            xrOrigin = FindFirstObjectByType<XROrigin>();
        
        headTransform = xrOrigin.Camera.transform;
        
        // Adjust these paths based on your actual XR Rig hierarchy
        leftHandTransform = xrOrigin.transform.Find("Camera Offset/LeftHand");
        rightHandTransform = xrOrigin.transform.Find("Camera Offset/RightHand");
        
        if (leftHandTransform == null || rightHandTransform == null)
        {
            Debug.LogWarning("Could not find hand controllers. Please check the hierarchy paths.");
        }
        
        InitializeTracking();
    }
    
    void InitializeTracking()
    {
        lastHeadPosition = headTransform.position;
        lastLeftHandPosition = leftHandTransform.position;
        lastRightHandPosition = rightHandTransform.position;
        
        lastHeadRotation = headTransform.rotation;
        lastLeftHandRotation = leftHandTransform.rotation;
        lastRightHandRotation = rightHandTransform.rotation;
    }
    
    void Update()
    {
        frameCount++;
        
        // Only sample every N frames
        if (frameCount % sampleInterval != 0)
            return;
        
        // Track positional movement
        totalHeadDistance += Vector3.Distance(headTransform.position, lastHeadPosition);
        totalLeftHandDistance += Vector3.Distance(leftHandTransform.position, lastLeftHandPosition);
        totalRightHandDistance += Vector3.Distance(rightHandTransform.position, lastRightHandPosition);
        
        // Track rotational movement
        totalHeadRotation += Quaternion.Angle(lastHeadRotation, headTransform.rotation);
        totalLeftHandRotation += Quaternion.Angle(lastLeftHandRotation, leftHandTransform.rotation);
        totalRightHandRotation += Quaternion.Angle(lastRightHandRotation, rightHandTransform.rotation);
        
        // Update last values
        lastHeadPosition = headTransform.position;
        lastLeftHandPosition = leftHandTransform.position;
        lastRightHandPosition = rightHandTransform.position;
        
        lastHeadRotation = headTransform.rotation;
        lastLeftHandRotation = leftHandTransform.rotation;
        lastRightHandRotation = rightHandTransform.rotation;
    }
    
    public void ResetTracking()
    {
        totalHeadDistance = 0f;
        totalLeftHandDistance = 0f;
        totalRightHandDistance = 0f;
        
        totalHeadRotation = 0f;
        totalLeftHandRotation = 0f;
        totalRightHandRotation = 0f;
        
        frameCount = 0;
        
        InitializeTracking();
    }
    
    public MovementData GetMovementData()
    {
        return new MovementData
        {
            headDistance = totalHeadDistance,
            leftHandDistance = totalLeftHandDistance,
            rightHandDistance = totalRightHandDistance,
            headRotation = totalHeadRotation,
            leftHandRotation = totalLeftHandRotation,
            rightHandRotation = totalRightHandRotation
        };
    }
}

[System.Serializable]
public struct MovementData
{
    public float headDistance;
    public float leftHandDistance;
    public float rightHandDistance;
    public float headRotation;
    public float leftHandRotation;
    public float rightHandRotation;
}