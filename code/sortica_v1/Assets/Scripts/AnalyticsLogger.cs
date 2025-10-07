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
        LogEvent("sessionStart", new { username = userId });
    }
    
    public void LogEvent(string eventType, object data)
    {
        if (writer == null)
        {
            Debug.LogWarning("AnalyticsLogger: Writer not initialized. Call SetUserId first.");
            return;
        }
        
        var logEntry = new
        {
            userId = userId,
            sessionId = sessionId,
            timestamp = Time.time - sessionStartTime,
            eventType = eventType,
            data = data
        };
        
        string json = JsonUtility.ToJson(logEntry);
        
        // Queue the log instead of writing directly
        lock (logLock)
        {
            logQueue.Enqueue(json);
        }
    }
    
    public void LogLevelComplete(string level, MovementData movementData)
    {
        LogEvent("levelComplete", new { level = level });
        
        LogEvent("movement", new
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
        LogEvent("sessionEnd", new { reason = reason });
        
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