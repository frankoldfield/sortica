using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

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
    public bool buildingCompleted = false;
    
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
        
    }

    void Update()
    {
        // Detect state changes and log them
        if (game_state != previous_state)
        {
            Debug.Log("Stage transition from "+ previous_state.ToString()+" to "+ game_state.ToString());
            GameStates actualPrevious = previous_state;
            previous_state = game_state;
            if (game_state.Equals(GameStates.Restart_Game))
            {
                HandleStateTransition(actualPrevious, game_state);
            }
            else 
            {
                HandleStateTransition(previous_state, game_state);
            }


        }
    }

    public void RestartLevel() 
    {
        if ((game_state.Equals(GameStates.First_Level) || game_state.Equals(GameStates.Second_Level)) && !buildingCompleted) 
        {
            game_state = GameStates.Restart_Game;
        }
    }

    void HandleStateTransition(GameStates fromState, GameStates toState)
    {
        // Log the state change
        AnalyticsLogger.Instance.LogEvent("stateChanged", new StateChangedData
        {
            
            fromState = fromState.ToString(),
            toState = toState.ToString()
        }); ;

        // Handle specific transitions
        switch (toState)
        {
            case GameStates.Restart_Game:
                if (fromState.Equals(GameStates.First_Level))
                {
                    
                    AnalyticsLogger.Instance.LogEvent("levelStart", new LevelRestartStartData { level = "level1" });
                    matterGenerator.restartLevel("level1");

                    contentionUnit.level1CompletedBuilding.GetComponent<BuildingPlacement>().RestartMovement();

                    contentionUnit.InitializeForLevel("level1");
                    game_state = GameStates.First_Level;
                }
                else if (fromState.Equals(GameStates.Second_Level))
                {
                    AnalyticsLogger.Instance.LogEvent("levelStart", new LevelRestartStartData { level = "level2" });
                    matterGenerator.restartLevel("level2");
                    contentionUnit.level2CompletedBuilding.GetComponent<BuildingPlacement>().RestartMovement();
                    contentionUnit.InitializeForLevel("level2");
                    game_state = GameStates.Second_Level;
                }
                else 
                {
                    AnalyticsLogger.Instance.LogEvent("levelStart", new LevelRestartStartData { level = "INVALID" });
                }
                break;
            case GameStates.Start:
                StartCoroutine(FirstDialogue());
                game_state = GameStates.Introduction;
                ContainerAnimator.SetBool("open", true);
                Controller.SetActive(false);
                break;
            case GameStates.Introduction:
                supervisor.StartDialogue(DialogueStage.Introduction);
                
                break;
            case GameStates.First_Level:
                if (!fromState.Equals(GameStates.Restart_Game)) 
                {
                    AnalyticsLogger.Instance.LogEvent("levelStart", new LevelStartData { level = "level1", algorithm = "FIFO" });
                    movementTracker.ResetTracking();

                    // Initialize generator and contention unit for level 1
                    //Debug.Log("Empieza primer nivel");
                    matterGenerator.InitializeForLevel("level1");
                    contentionUnit.InitializeForLevel("level1");
                }
                
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
                if (!fromState.Equals(GameStates.Restart_Game))
                {
                    buildingCompleted = false;
                    AnalyticsLogger.Instance.LogEvent("levelStart", new LevelStartData { level = "level2", algorithm = "LIFO" });
                    movementTracker.ResetTracking();

                    // Initialize generator and contention unit for level 2
                    matterGenerator.InitializeForLevel("level2");
                    contentionUnit.InitializeForLevel("level2");
                }
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

    public void playGrabBuilding() 
    {
        StartCoroutine(GrabDialogue());
    }

    IEnumerator GrabDialogue()
    {

        // Wait
        Debug.Log("Diálogo grab building!");
        yield return new WaitForSeconds(0.5f);
        supervisor.currentLineIndex = 0;
        supervisor.PlayCurrentLine(supervisor.dialogueDictionary[DialogueStage.grabBuilding]);
        supervisor.currentLineIndex = 0;

    }

    IEnumerator FirstDialogue()
    {

        // Wait
        yield return new WaitForSeconds(2.5f);
        supervisor.PlayCurrentLine(supervisor.dialogueDictionary[DialogueStage.hey_dialogue]);
        supervisor.currentLineIndex = 0;

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