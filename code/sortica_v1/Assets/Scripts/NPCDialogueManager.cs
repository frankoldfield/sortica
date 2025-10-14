using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;

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
    private bool isPlaying = false;
    private DialogueStage currentStage;

    public Dictionary<DialogueStage, DialogueLine[]> dialogueDictionary;



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
        subtitleCanvas.gameObject.SetActive(false);

        // Subscribe to interaction events
        interactable.selectEntered.AddListener(OnNPCInteracted);
        currentStage = DialogueStage.NotSpeaking;
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnNPCInteracted);
    }

    private void OnNPCInteracted(SelectEnterEventArgs args)
    {
        if (currentStage.Equals(DialogueStage.NotSpeaking)) 
        {
            return;
        }

        DialogueLine[] dialogueLines = dialogueDictionary[currentStage];
        
        // First interaction starts dialogue
        if (!isPlaying)
        {
            PlayCurrentLine(dialogueLines);
        }
    }

    private void PlayCurrentLine(DialogueLine[] dialogueLines)
    {
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
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