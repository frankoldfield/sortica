using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;
using System.Linq;

public enum DialogueStage
{
    NotSpeaking,
    Hints1,
    Introduction,
    Level2,
    Hints2,
    FinishedGame
}

public class NPCDialogueManager : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueLine[] Hints_Level1_dialogueLines;
    [SerializeField] private DialogueLine[] Introduction_dialogueLines;
    [SerializeField] private DialogueLine[] Level2_dialogueLines;
    [SerializeField] private DialogueLine[] Hints_Level2_dialogueLines;
    [SerializeField] private DialogueLine[] FinishedGame_dialogueLines;

    [Header("References")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Canvas subtitleCanvas;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private XRSimpleInteractable interactable;

    private int currentLineIndex = 0;
    private int globalStateIndex = 0;
    private bool isPlaying = false;
    private DialogueStage currentStage;

    public Dictionary<DialogueStage, DialogueLine[]> dialogueDictionary;

    public Animator SupervisorAnimator;
    public Animator MouthAnimator;

    public GameObject GeneratorParticles;
    public GameObject ContentionParticles;

    private ParticleSystem GeneratorParticleSystem;
    private ParticleSystem ContentionParticleSystem;

    List<int> FalseAnimations = new List<int> { 1, 3 };

    private void Start()
    {

        GeneratorParticleSystem = GeneratorParticles.GetComponent<ParticleSystem>();
        ContentionParticleSystem = ContentionParticles.GetComponent<ParticleSystem>();
        GeneratorParticles.SetActive(false);
        ContentionParticles.SetActive(false);

        dialogueDictionary = new Dictionary<DialogueStage, DialogueLine[]>()
        {
            {DialogueStage.NotSpeaking, new DialogueLine[0] },
            {DialogueStage.Hints1, Hints_Level1_dialogueLines },
            {DialogueStage.Introduction, Introduction_dialogueLines },
            {DialogueStage.Level2, Level2_dialogueLines },
            {DialogueStage.Hints2, Hints_Level2_dialogueLines },
            {DialogueStage.FinishedGame, FinishedGame_dialogueLines },
        };

        // Hide subtitles initially
        subtitleCanvas.gameObject.SetActive(true);

        // Subscribe to interaction events
        interactable.selectEntered.AddListener(OnNPCInteracted);
        currentStage = DialogueStage.NotSpeaking;
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnNPCInteracted);
    }

    public void OnNPCInteracted(SelectEnterEventArgs args)
    {
        // Log NPC interaction
        AnalyticsLogger.Instance.LogEvent("npcInteracted", new NPCInteractedData
        {
            currentStage = currentStage.ToString(),
            globalStateIndex = globalStateIndex,
            currentLineIndex = currentLineIndex,
            isPlaying = isPlaying
        });

        if (currentStage.Equals(DialogueStage.NotSpeaking))
        {
            return;
        }

        DialogueLine[] dialogueLines = dialogueDictionary[currentStage];

        // First interaction starts dialogue
        if (SupervisorAnimator.GetCurrentAnimatorStateInfo(0).IsTag("interactable") && !isPlaying)
        {
            int animation_index;
            Debug.Log("globalStateIndex: " + globalStateIndex);
            Debug.Log("Animation index= " + SupervisorAnimator.GetInteger("animation_index"));

            switch (globalStateIndex)
            {
                case 0:
                    //Welcome! I'm your supervisor...
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    globalStateIndex++;
                    MasterScript master = FindFirstObjectByType<MasterScript>();
                    if (master != null)
                    {
                        master.Controller.SetActive(false);
                    }
                    
                    PlayCurrentLine(dialogueLines);
                    break;
                case 1:
                    //See how I've changed size...
                    PlayCurrentLine(dialogueLines);

                    globalStateIndex++;
                    break;
                case 2:
                    //Different types of programmable matter....

                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    GeneratorParticles.SetActive(true);
                    

                    break;
                case 3:
                    //Your job is to put programmable matter into this contention unit...
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    ContentionParticles.SetActive(true);
                   
                    break;
                case 4:
                    //Pull the reset lever...
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    break;
                case 5:
                    //Interact with me if you need any hints...
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    break;
                case 6:
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    break;
                case 7:
                    //Level 1 starts + First hint and hint loop AND WHEN LEVEL 1 FINISHED, GOOD JOB, AT LEAST...
                    if (!currentStage.Equals(DialogueStage.Hints1))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 8:
                    //Shit, the generator is messed up!
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints1))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);

                    Color newColor = new Color(152, 0, 0, 255);
                    GeneratorParticles.GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
                    GeneratorParticleSystem.startSpeed = 5;

                    ContentionParticles.GetComponent<Renderer>().material.SetColor("_EmissionColor", newColor);
                    ContentionParticleSystem.startSpeed = 5;
                    break;
                case 9:
                    //Well gg...
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints2))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 10:
                    //Skipped for some reason
                    if (!currentStage.Equals(DialogueStage.Hints2))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 11:
                    //First LIFO hint and hint loop for level 2
                    if (!currentStage.Equals(DialogueStage.Hints2))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 12:
                    //You've made it...
                    GeneratorParticles.SetActive(false);
                    ContentionParticles.SetActive(false);
                    PlayCurrentLine(dialogueLines);
                    break;
                default:
                    break;
            }
        }
    }

    public void PlayCurrentLine(DialogueLine[] dialogueLines)
    {
        if (currentStage.Equals(DialogueStage.NotSpeaking))
        {
            return;
        }
        else if (currentLineIndex >= dialogueLines.Length)
        {
            if (currentStage.Equals(DialogueStage.Hints1) || currentStage.Equals(DialogueStage.Hints2))
            {
                currentLineIndex = 0;
            }
            else
            {
                EndDialogue();
                return;
            }
        }

        DialogueLine currentLine = dialogueLines[currentLineIndex];
        MasterScript master = FindFirstObjectByType<MasterScript>();
        master.HoverAnimator.SetBool("hover", false);
        // Show subtitle
        subtitleCanvas.gameObject.SetActive(true);
        subtitleText.text = currentLine.subtitleText;

        // Play audio
        audioSource.clip = currentLine.voiceClip;
        audioSource.Play();

        isPlaying = true;

        //Start talking animation
        MouthAnimator.SetBool("talking", true);
        // Start checking when audio finishes
        StartCoroutine(WaitForAudioToFinish());

        currentLineIndex++;
    }

    private System.Collections.IEnumerator WaitForAudioToFinish()
    {
        // Wait until audio stops playing
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        //Disable talking animation
        isPlaying = false;
        MouthAnimator.SetBool("talking", false);
        MasterScript master = FindFirstObjectByType<MasterScript>();
        master.HoverAnimator.SetBool("hover", true);
        if (globalStateIndex == 8 || globalStateIndex == 9) 
        {
            OnNPCInteracted(null);
        }
        if (currentStage.Equals(DialogueStage.Hints1) || currentStage.Equals(DialogueStage.Hints2)) 
        {
            subtitleCanvas.gameObject.SetActive(false);
        }
    }

    public void StartDialogue(DialogueStage dialogue)
    {
        currentLineIndex = 0;
        isPlaying = false;
        currentStage = dialogue;

        // Log dialogue start
        AnalyticsLogger.Instance.LogEvent("dialogueStarted", new DialogueStartedData
        {
            dialogueStage = dialogue.ToString(),
            globalStateIndex = globalStateIndex,
            totalLines = dialogueDictionary[dialogue].Length
        });
    }

    public void EndDialogue()
    {
        // Log dialogue end
        AnalyticsLogger.Instance.LogEvent("dialogueEnded", new DialogueEndedData
        {
            dialogueStage = currentStage.ToString(),
            linesPlayed = currentLineIndex,
            globalStateIndex = globalStateIndex
        });

        subtitleCanvas.gameObject.SetActive(false);
        currentLineIndex = 0;
        isPlaying = false;

        MasterScript master = FindFirstObjectByType<MasterScript>();
        if (master != null)
        {
            master.OnDialogueEnded(currentStage);
        }

        currentStage = DialogueStage.NotSpeaking;
    }
}