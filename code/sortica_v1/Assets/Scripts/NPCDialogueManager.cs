using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;
using System.Linq;

public enum DialogueStage
{
    NotSpeaking,
    Hints,
    Introduction,
    Level2,
    FinishedGame

}

public class NPCDialogueManager : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueLine[] Hints_dialogueLines;
    [SerializeField] private DialogueLine[] Introduction_dialogueLines;
    [SerializeField] private DialogueLine[] Level2_dialogueLines;
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
    List<int> FalseAnimations = new List<int> {1, 3};

    private void Start()
    {
        dialogueDictionary = new Dictionary<DialogueStage, DialogueLine[]>()
        {
            {DialogueStage.NotSpeaking, new DialogueLine[0] },
            {DialogueStage.Hints, Hints_dialogueLines },
            {DialogueStage.Introduction, Introduction_dialogueLines },
            {DialogueStage.Level2, Level2_dialogueLines },
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


    public void Update()
    {
        
        //if (InAnimation) 
        //{
        //    AnimatorStateInfo animStateInfo = SupervisorAnimator.GetCurrentAnimatorStateInfo(0);
        //    if (!animStateInfo.IsTag("idle") && animStateInfo.normalizedTime >= 1) 
        //    {
        //        int new_index = SupervisorAnimator.GetInteger("animation_index") + 1;
        //        Debug.Log("New index: "+new_index);
        //        SupervisorAnimator.SetInteger("animation_index", new_index);
        //        if (!FalseAnimations.Contains(new_index))
        //        {
        //            Debug.Log("InAnimation is now false");
        //            InAnimation = false;
        //        }
                
        //    }
        //}
    }

    private void OnNPCInteracted(SelectEnterEventArgs args)
    {
        
        if (currentStage.Equals(DialogueStage.NotSpeaking))
        {
            return;
        }

        DialogueLine[] dialogueLines = dialogueDictionary[currentStage];

        // First interaction starts dialogue

        if (SupervisorAnimator.GetCurrentAnimatorStateInfo(0).IsTag("interactable") && !isPlaying) 
        {
            int animation_index;
            Debug.Log("globalStateIndex: "+ globalStateIndex);
            Debug.Log("Animation index= " + SupervisorAnimator.GetInteger("animation_index"));
            switch (globalStateIndex)
            {
                case 0:
                    //va a table_idle
                    
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    break;
                case 1:
                    //primer diálogo de welcome
                    PlayCurrentLine(dialogueLines);
                    
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    break;
                case 2:
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 3:
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 4:
                    //animation_index = SupervisorAnimator.GetInteger("animation_index");
                    //SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 5:
                    
                    if (!currentStage.Equals(DialogueStage.Hints) && !currentStage.Equals(DialogueStage.NotSpeaking))
                    {
                        animation_index = SupervisorAnimator.GetInteger("animation_index");
                        SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                        
                    }
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    break;
                case 6:
                    globalStateIndex++;
                    PlayCurrentLine(dialogueLines);
                    break;
                case 7:

                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 8:
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 9:
                    animation_index = SupervisorAnimator.GetInteger("animation_index");
                    SupervisorAnimator.SetInteger("animation_index", animation_index + 1);
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 10:
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 11:
                    if (!currentStage.Equals(DialogueStage.Hints))
                    {
                        globalStateIndex++;
                    }
                    PlayCurrentLine(dialogueLines);
                    break;
                case 12:
                    
                    PlayCurrentLine(dialogueLines);
                    break;
                default:

                    break;
            }

            

            
        }
        
    }

    private void PlayCurrentLine(DialogueLine[] dialogueLines)
    {
        
        if (currentStage.Equals(DialogueStage.NotSpeaking))
        {
            return;
        }
        else if (currentLineIndex >= dialogueLines.Length)
        {
            if (currentStage.Equals(DialogueStage.Hints))
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

        // Show subtitle
        subtitleCanvas.gameObject.SetActive(true);
        subtitleText.text = currentLine.subtitleText;

        // Play audio
        audioSource.clip = currentLine.voiceClip;
        audioSource.Play();

        isPlaying = true;

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

        isPlaying = false;
    }

    public void StartDialogue(DialogueStage dialogue)
    {
        currentLineIndex = 0;
        isPlaying = false;
        currentStage = dialogue;
    }

    public void EndDialogue()
    {
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