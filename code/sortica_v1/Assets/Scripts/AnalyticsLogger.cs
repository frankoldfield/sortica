using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class AnalyticsLogger : MonoBehaviour
{
    public static AnalyticsLogger Instance { get; private set; }

    private string userId;
    private string sessionId;
    private float sessionStartTime;
    private StreamWriter writer;
    private string filePath;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sessionId = Guid.NewGuid().ToString();
        sessionStartTime = Time.time;
    }

    public void SetUserId(string username)
    {
        userId = string.IsNullOrEmpty(username) ? SystemInfo.deviceUniqueIdentifier : username;

        // Initialize file after userId is set
        string fileName = $"analytics_{userId}_{sessionId}_{DateTime.Now:yyyyMMdd_HHmmss}.jsonl";
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        // Open with AutoFlush enabled to ensure immediate writes
        writer = new StreamWriter(filePath, append: true);
        writer.AutoFlush = true; // CRITICAL: Flush after every write

        Debug.Log($"Analytics file: {filePath}");

        // Log session start
        LogEvent("sessionStart", new SessionStartData { username = userId });
    }

    public void LogEvent(string eventType, object data)
    {
        if (writer == null)
        {
            Debug.LogWarning("AnalyticsLogger: Writer not initialized. Call SetUserId first.");
            return;
        }

        // Manual JSON construction to avoid JsonUtility issues with anonymous objects
        string dataJson = JsonUtility.ToJson(data);

        string json = $"{{\"userId\":\"{userId}\",\"sessionId\":\"{sessionId}\",\"timestamp\":{Time.time - sessionStartTime},\"eventType\":\"{eventType}\",\"data\":{dataJson}}}";

        // Write immediately to disk (no queuing)
        try
        {
            writer.WriteLine(json);
            writer.Flush(); // Force immediate flush to disk
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write analytics event '{eventType}': {e.Message}");
        }
    }

    public void LogLevelComplete(string level, MovementData movementData)
    {
        LogEvent("levelComplete", new LevelCompleteData { level = level });

        LogEvent("movement", new MovementEventData
        {
            level = level,
            headDistance = movementData.headDistance,
            headRotation = movementData.headRotation,
            leftHandDistance = movementData.leftHandDistance,
            leftHandRotation = movementData.leftHandRotation,
            rightHandDistance = movementData.rightHandDistance,
            rightHandRotation = movementData.rightHandRotation
        });
    }

    public void LogSessionEnd(string reason)
    {
        LogEvent("sessionEnd", new SessionEndData { reason = reason });

        // Ensure everything is written
        if (writer != null)
        {
            writer.Flush();
        }
    }

    /// <summary>
    /// Forces an immediate flush of all data and logs current movement.
    /// Call this before application exit.
    /// </summary>
    public void ForceFlushAll()
    {
        Debug.Log("Force flushing all analytics data...");

        // Get current movement data
        MasterScript master = FindObjectOfType<MasterScript>();
        if (master != null)
        {
            VRMovementTracker tracker = master.GetComponent<VRMovementTracker>();
            if (tracker != null)
            {
                MovementData currentMovement = tracker.GetMovementData();

                // Determine which level we're in
                string currentLevel = "unknown";
                if (master.game_state == GameStates.First_Level || master.game_state == GameStates.First_Finished)
                {
                    currentLevel = "level1";
                }
                else if (master.game_state == GameStates.Second_Level || master.game_state == GameStates.Second_Finished)
                {
                    currentLevel = "level2";
                }

                // Log current movement state
                LogEvent("movementSnapshot", new MovementEventData
                {
                    level = currentLevel,
                    headDistance = currentMovement.headDistance,
                    headRotation = currentMovement.headRotation,
                    leftHandDistance = currentMovement.leftHandDistance,
                    leftHandRotation = currentMovement.leftHandRotation,
                    rightHandDistance = currentMovement.rightHandDistance,
                    rightHandRotation = currentMovement.rightHandRotation
                });
            }
        }

        // Final flush
        if (writer != null)
        {
            writer.Flush();
        }

        Debug.Log($"All analytics data flushed to: {filePath}");
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application quitting - flushing analytics...");

        // Log that we're quitting
        if (writer != null)
        {
            LogEvent("applicationQuit", new ApplicationQuitData { reason = "exit" });
            writer.Flush();
            writer.Close();
        }
    }

    void OnDestroy()
    {
        if (Instance == this && writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }
}

// Serializable data classes for logging
[System.Serializable]
public class SessionStartData
{
    public string username;
}

[System.Serializable]
public class SessionEndData
{
    public string reason;
}

[System.Serializable]
public class LevelCompleteData
{
    public string level;
}

[System.Serializable]
public class MovementEventData
{
    public string level;
    public float headDistance;
    public float headRotation;
    public float leftHandDistance;
    public float leftHandRotation;
    public float rightHandDistance;
    public float rightHandRotation;
}

[System.Serializable]
public class StateChangedData
{
    public string fromState;
    public string toState;
}

[System.Serializable]
public class LevelStartData
{
    public string level;
    public string algorithm;
}

[System.Serializable]
public class LevelRestartStartData
{
    public string level;
}

[System.Serializable]
public class TotalMovementData
{
    public float headDistance;
    public float headRotation;
    public float leftHandDistance;
    public float leftHandRotation;
    public float rightHandDistance;
    public float rightHandRotation;
}

[System.Serializable]
public class MatterGeneratedData
{
    public string materialType;
    public Vector3 position;
}

[System.Serializable]
public class MatterPickedData
{
    public string materialType;
    public Vector3 position;
}

[System.Serializable]
public class MatterDroppedData
{
    public string materialType;
    public Vector3 position;
    public float holdDuration;
}

[System.Serializable]
public class ContentionCorrectData
{
    public string level;
    public string materialType;
    public string expectedMaterial;
    public int buildingStep;
    public int totalSteps;
    public string algorithm;
    public float progressPercentage;
}

[System.Serializable]
public class ContentionErrorData
{
    public string level;
    public string droppedMaterial;
    public string expectedMaterial;
    public int buildingStep;
    public int totalSteps;
    public string algorithm;
}

[System.Serializable]
public class BuildingCompleteData
{
    public string level;
    public int totalSteps;
    public string algorithm;
}

[System.Serializable]
public class BuildingGrabbedData
{
    public string level;
}

[System.Serializable]
public class BuildingPlacedData
{
    public string level;
    public float holdDuration;
}

[System.Serializable]
public class GeneratorInitializedData
{
    public string level;
    public int totalMatter;
    public string[] sequence;
    public string algorithm;
}

[System.Serializable]
public class GeneratorSpawnedData
{
    public string level;
    public string materialType;
    public int remainingCount;
}

[System.Serializable]
public class GeneratorEmptyData
{
    public string level;
}

[System.Serializable]
public class ContentionInitializedData
{
    public string level;
    public string[] expectedSequence;
    public string algorithm;
}

[System.Serializable]
public class BuildingStageRevealedData
{
    public string level;
    public int stage;
    public string stageName;
    public int visibleStages;
}

[System.Serializable]
public class NPCInteractedData
{
    public string currentStage;
    public int globalStateIndex;
    public int currentLineIndex;
    public bool isPlaying;
}

[System.Serializable]
public class DialogueStartedData
{
    public string dialogueStage;
    public int globalStateIndex;
    public int totalLines;
}

[System.Serializable]
public class DialogueEndedData
{
    public string dialogueStage;
    public int linesPlayed;
    public int globalStateIndex;
}

[System.Serializable]
public class ApplicationQuitData
{
    public string reason;
}