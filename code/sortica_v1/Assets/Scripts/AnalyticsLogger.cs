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

    // Queue for thread-safe logging
    private Queue<string> logQueue = new Queue<string>();
    private object logLock = new object();

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

    void Update()
    {
        // Process queued logs on the main thread
        lock (logLock)
        {
            while (logQueue.Count > 0)
            {
                string json = logQueue.Dequeue();
                if (writer != null)
                {
                    writer.WriteLine(json);
                }
            }
        }
    }

    public void SetUserId(string username)
    {
        userId = string.IsNullOrEmpty(username) ? SystemInfo.deviceUniqueIdentifier : username;

        // Initialize file after userId is set
        string fileName = $"analytics_{userId}_{sessionId}_{DateTime.Now:yyyyMMdd_HHmmss}.jsonl";
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        writer = new StreamWriter(filePath, append: true);

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

        // Queue the log instead of writing directly
        lock (logLock)
        {
            logQueue.Enqueue(json);
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

        // Force flush remaining logs
        FlushLogs();
    }

    void FlushLogs()
    {
        lock (logLock)
        {
            while (logQueue.Count > 0)
            {
                string json = logQueue.Dequeue();
                if (writer != null)
                {
                    writer.WriteLine(json);
                }
            }

            if (writer != null)
            {
                writer.Flush();
            }
        }
    }

    void OnApplicationQuit()
    {
        FlushLogs();

        if (writer != null)
        {
            writer.Close();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            FlushLogs();

            if (writer != null)
            {
                writer.Close();
            }
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
    public float distanceFromTarget;
    public bool placementAccurate;
    public float holdDuration;
}

[System.Serializable]
public class BuildingPlacementAttemptData
{
    public string level;
    public float distanceFromTarget;
    public bool tooFar;
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