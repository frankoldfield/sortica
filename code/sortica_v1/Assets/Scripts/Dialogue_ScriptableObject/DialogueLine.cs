using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "Dialogue/DialogueLine")]
public class DialogueLine : ScriptableObject
{
    [TextArea(3, 10)]
    public string subtitleText;
    public AudioClip voiceClip;
}
