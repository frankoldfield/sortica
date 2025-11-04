using UnityEngine;

public enum GameStates
{
    Restart_Game,
    Loading_Game,
    Start,
    Introduction,
    First_Level,
    First_Finished,
    Second_Level,
    Second_Finished,
    Game_Finished
}

public class MasterScript : MonoBehaviour
{
    public GameStates game_state;
    private GameStates previous_state;
    
    public string username;
    
    [Header("Game Components")]
    public MatterGenerator matterGenerator;
    public ContentionUnit contentionUnit;
    public NPCDialogueManager supervisor;
    public GameObject Controller;

    public Animator ContainerAnimator;

    public Animator HoverAnimator;
    
    private VRMovementTracker movementTracker;
    
    // Movement data storage
    private MovementData level1MovementData;
    private MovementData level2MovementData;


    void Start()
    {
        Controller.SetActive(true);
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
        game_state = GameStates.Introduction;
        ContainerAnimator.SetBool("open", true);
    }

    void Update()
    {
        // Detect state changes and log them
        if (game_state != previous_state)
        {
            Debug.Log("Stage transition from "+ previous_state.ToString()+" to "+ game_state.ToString());
            HandleStateTransition(previous_state, game_state);
            previous_state = game_state;
        }
    }
    
    void HandleStateTransition(GameStates fromState, GameStates toState)
    {
        // Log the state change
        AnalyticsLogger.Instance.LogEvent("stateChanged", new StateChangedData
        {
            fromState = fromState.ToString(),
            toState = toState.ToString()
        });

        // Handle specific transitions
        switch (toState)
        {
            case GameStates.Loading_Game:

                break;
            case GameStates.Start:

                break;
            case GameStates.Introduction:
                supervisor.StartDialogue(DialogueStage.Introduction);
                
                break;
            case GameStates.First_Level:
                AnalyticsLogger.Instance.LogEvent("levelStart", new LevelStartData { level = "level1", algorithm = "FIFO" });
                movementTracker.ResetTracking();

                // Initialize generator and contention unit for level 1
                Debug.Log("Empieza primer nivel");
                matterGenerator.InitializeForLevel("level1");
                contentionUnit.InitializeForLevel("level1");
                supervisor.StartDialogue(DialogueStage.Hints1);
                supervisor.OnNPCInteracted(null);
                break;
                
            case GameStates.First_Finished:
                // Capture level 1 movement data
                level1MovementData = movementTracker.GetMovementData();
                AnalyticsLogger.Instance.LogLevelComplete("level1", level1MovementData);
                supervisor.StartDialogue(DialogueStage.Level2);
                supervisor.OnNPCInteracted(null);
                supervisor.OnNPCInteracted(null);
                break;
                
            case GameStates.Second_Level:
                AnalyticsLogger.Instance.LogEvent("levelStart", new LevelStartData { level = "level2", algorithm = "LIFO" });
                movementTracker.ResetTracking();
                
                // Initialize generator and contention unit for level 2
                matterGenerator.InitializeForLevel("level2");
                contentionUnit.InitializeForLevel("level2");
                supervisor.StartDialogue(DialogueStage.Hints2);
                supervisor.OnNPCInteracted(null);
                break;
                
            case GameStates.Second_Finished:
                // Capture level 2 movement data
                level2MovementData = movementTracker.GetMovementData();
                AnalyticsLogger.Instance.LogLevelComplete("level2", level2MovementData);
                supervisor.StartDialogue(DialogueStage.FinishedGame);
                supervisor.OnNPCInteracted(null);
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

                AnalyticsLogger.Instance.LogEvent("totalMovement", new TotalMovementData
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

    public void OnDialogueEnded(DialogueStage dialogueStage)
    {
        switch (dialogueStage) 
        { 
            case DialogueStage.NotSpeaking:

                break;
            case DialogueStage.Hints1:

                break;
            case DialogueStage.Introduction:
                //TO-DO: Move supervisor
                game_state = GameStates.First_Level;
                break;
            case DialogueStage.Level2:
                game_state = GameStates.Second_Level;
                break;

            case DialogueStage.Hints2:

                break;

            case DialogueStage.FinishedGame:
                game_state = GameStates.Game_Finished;
                break;
        }
    }

    public void ExitGame() 
    
    {
        ContainerAnimator.SetBool("close", true);
        ContainerAnimator.SetBool("open", false);
    }
}