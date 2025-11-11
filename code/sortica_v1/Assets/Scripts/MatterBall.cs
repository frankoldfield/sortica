using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MatterBall : MonoBehaviour
{
    [Header("Matter Properties")]
    public MaterialType materialType;

    private XRGrabInteractable grabInteractable;
    private float pickupTime;

    // Event to notify when picked up
    public event Action OnMatterPickedUp;

    void Awake()
    {
        // Search in children if not found on this GameObject
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        if (grabInteractable == null)
        {
            Debug.LogError("MatterBall: XRGrabInteractable not found on this GameObject or its children!");
            return;
        }

        //Debug.Log($"MatterBall {materialType} initialized with XRGrabInteractable");

        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnPickup);
        grabInteractable.selectExited.AddListener(OnDrop);
    }

    public void LogGeneration()
    {
        AnalyticsLogger.Instance.LogEvent("matterGenerated", new MatterGeneratedData
        {
            materialType = materialType.ToString(),
            position = transform.position
        });
    }

    void OnPickup(SelectEnterEventArgs args)
    {
        pickupTime = Time.time;

        //Debug.Log($"Matter ball {materialType} picked up!");

        AnalyticsLogger.Instance.LogEvent("matterPicked", new MatterPickedData
        {
            materialType = materialType.ToString(),
            position = transform.position
        });

        // Notify generator that this was picked up
        OnMatterPickedUp?.Invoke();
    }

    void OnDrop(SelectExitEventArgs args)
    {
        float holdDuration = Time.time - pickupTime;

        //Debug.Log($"Matter ball {materialType} dropped!");

        AnalyticsLogger.Instance.LogEvent("matterDropped", new MatterDroppedData
        {
            materialType = materialType.ToString(),
            position = transform.position,
            holdDuration = holdDuration
        });
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnPickup);
            grabInteractable.selectExited.RemoveListener(OnDrop);
        }
    }
}

// Enum for material types
public enum MaterialType
{
    Terrain,
    Cement,
    Metal,
    Bricks,
    Wood,
    Paint
}