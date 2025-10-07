using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

public class MatterGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public GameObject[] matterBallPrefabs;
    public Transform spawnPoint;

    [Header("Level 1 Sequence (FIFO)")]
    public MaterialType[] level1Sequence;

    [Header("Level 2 Sequence (LIFO)")]
    public MaterialType[] level2Sequence;

    private Queue<MaterialType> generationQueue;
    private GameObject currentMatterBall;
    private MatterBall currentMatterBallScript;
    private bool canGenerate = true;
    private string currentLevel;

    public void InitializeForLevel(string level)
    {
        currentLevel = level;
        generationQueue = new Queue<MaterialType>();

        // Load the appropriate sequence
        MaterialType[] sequence = level == "level1" ? level1Sequence : level2Sequence;

        foreach (MaterialType materialType in sequence)
        {
            generationQueue.Enqueue(materialType);
        }

        // Convert sequence to string array for logging
        string[] sequenceStrings = System.Array.ConvertAll(sequence, x => x.ToString());

        AnalyticsLogger.Instance.LogEvent("generatorInitialized", new
        {
            level = level,
            totalMatter = generationQueue.Count,
            sequence = sequenceStrings,
            algorithm = level == "level1" ? "FIFO" : "LIFO"
        });

        GenerateNextMatter();
    }

    void GenerateNextMatter()
    {
        if (generationQueue.Count == 0)
        {
            AnalyticsLogger.Instance.LogEvent("generatorEmpty", new
            {
                level = currentLevel
            });
            Debug.Log("Generator empty - all matter generated");
            return;
        }

        // Get next material type from queue
        MaterialType materialType = generationQueue.Dequeue();

        // Find the correct prefab for this material type
        GameObject prefab = GetPrefabForType(materialType);

        if (prefab == null)
        {
            Debug.LogError($"No prefab found for material type: {materialType}");
            return;
        }

        // Instantiate the matter ball
        currentMatterBall = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

        // Get the MatterBall script
        currentMatterBallScript = currentMatterBall.GetComponent<MatterBall>();
        if (currentMatterBallScript != null)
        {
            currentMatterBallScript.LogGeneration();

            // Subscribe to pickup event
            currentMatterBallScript.OnMatterPickedUp += OnCurrentMatterPickedUp;
        }
        else
        {
            Debug.LogError($"MatterBall component not found on spawned prefab!");
        }

        canGenerate = false; // Wait for pickup before generating next

        AnalyticsLogger.Instance.LogEvent("generatorSpawned", new
        {
            level = currentLevel,
            materialType = materialType.ToString(),
            remainingCount = generationQueue.Count
        });

        Debug.Log($"Generated {materialType}, remaining: {generationQueue.Count}");
    }

    void OnCurrentMatterPickedUp()
    {
        Debug.Log("Generator detected matter pickup!");

        // Unsubscribe from the event
        if (currentMatterBallScript != null)
        {
            currentMatterBallScript.OnMatterPickedUp -= OnCurrentMatterPickedUp;
        }

        // Generate next matter after a short delay
        if (canGenerate == false)
        {
            canGenerate = true;
            Invoke(nameof(GenerateNextMatter), 0.5f);
        }
    }

    GameObject GetPrefabForType(MaterialType materialType)
    {
        foreach (GameObject prefab in matterBallPrefabs)
        {
            MatterBall ball = prefab.GetComponent<MatterBall>();
            if (ball != null && ball.materialType == materialType)
            {
                return prefab;
            }
        }
        return null;
    }
}