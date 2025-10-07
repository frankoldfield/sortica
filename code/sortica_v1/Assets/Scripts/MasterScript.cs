using UnityEngine;

public enum GameStates
{
    Restart_Game = -1,
    Loading_Game = 0,
    Start = 1,
    Introduction = 2,
    First_Level = 3,
    First_Finished = 4,
    First_to_Second = 5,
    Second_Level = 6,
    Second_Finished = 7,
    Game_Finished = 8
}

public class MasterScript : MonoBehaviour
{
    public GameStates game_state;
    private GameStates previous_state;
    
    public string username;
    
    [Header("Game Components")]
    public MatterGenerator matterGenerator;
    public ContentionUnit contentionUnit;
    
    private VRMovementTracker movementTracker;
    
    // Movement data storage
    private MovementData level1MovementData;
    private MovementData level2MovementData;

    void Start()
    {
        game_state = GameStates.Loading_Game;
        previous_state = game_state;

        // Initialize analytics logger as singleton
        if (AnalyticsLogger.Instance == null)
        {
            gameObject.AddComponent<AnalyticsLogger>();
        }

        // Set username in analytics logger
        AnalyticsLogger.Instance.SetUserId(username);

        // Initialize movement tracker
        movementTracker = gameObject.AddComponent<VRMovementTracker>();

        // Read progress
        // Load progress
        // Start world
        game_state = GameStates.First_Level;
    }

    void Update()
    {
        // Detect state changes and log them
        if (game_state != previous_state)
        {
            HandleStateTransition(previous_state, game_state);
            previous_state = game_state;
        }
        
        // switch case with different game states
        // Do state checks
        // Perform state actions
        // Load new state or keep same state
    }
    
    void HandleStateTransition(GameStates fromState, GameStates toState)
    {
        // Log the state change
        AnalyticsLogger.Instance.LogEvent("stateChanged", new 
        { 
            fromState = fromState.ToString(), 
            toState = toState.ToString() 
        });
        
        // Handle specific transitions
        switch (toState)
        {
            case GameStates.First_Level:
                AnalyticsLogger.Instance.LogEvent("levelStart", new { level = "level1", algorithm = "FIFO" });
                movementTracker.ResetTracking();

                // Initialize generator and contention unit for level 1
                Debug.Log("Empieza primer nivel lol");
                matterGenerator.InitializeForLevel("level1");
                contentionUnit.InitializeForLevel("level1");
                break;
                
            case GameStates.First_Finished:
                // Capture level 1 movement data
                level1MovementData = movementTracker.GetMovementData();
                AnalyticsLogger.Instance.LogLevelComplete("level1", level1MovementData);
                break;
                
            case GameStates.Second_Level:
                AnalyticsLogger.Instance.LogEvent("levelStart", new { level = "level2", algorithm = "LIFO" });
                movementTracker.ResetTracking();
                
                // Initialize generator and contention unit for level 2
                matterGenerator.InitializeForLevel("level2");
                contentionUnit.InitializeForLevel("level2");
                break;
                
            case GameStates.Second_Finished:
                // Capture level 2 movement data
                level2MovementData = movementTracker.GetMovementData();
                AnalyticsLogger.Instance.LogLevelComplete("level2", level2MovementData);
                break;
                
            case GameStates.Game_Finished:
                // Log total movement (sum of both levels)
                MovementData totalMovement = new MovementData
                {
                    headDistance = level1MovementData.headDistance + level2MovementData.headDistance,
                    leftHandDistance = level1MovementData.leftHandDistance + level2MovementData.leftHandDistance,
                    rightHandDistance = level1MovementData.rightHandDistance + level2MovementData.rightHandDistance,
                    headRotation = level1MovementData.headRotation + level2MovementData.headRotation,
                    leftHandRotation = level1MovementData.leftHandRotation + level2MovementData.leftHandRotation,
                    rightHandRotation = level1MovementData.rightHandRotation + level2MovementData.rightHandRotation
                };
                
                AnalyticsLogger.Instance.LogEvent("totalMovement", new
                {
                    headDistance = totalMovement.headDistance,
                    headRotation = totalMovement.headRotation,
                    leftHandDistance = totalMovement.leftHandDistance,
                    leftHandRotation = totalMovement.leftHandRotation,
                    rightHandDistance = totalMovement.rightHandDistance,
                    rightHandRotation = totalMovement.rightHandRotation
                });
                
                AnalyticsLogger.Instance.LogSessionEnd("session_complete");
                break;
        }
    }
    
    // Called by BuildingPlacement when building is successfully placed
    public void OnBuildingPlaced(string level)
    {
        if (level == "level1" && game_state == GameStates.First_Level)
        {
            game_state = GameStates.First_Finished;
        }
        else if (level == "level2" && game_state == GameStates.Second_Level)
        {
            game_state = GameStates.Second_Finished;
        }
    }
}