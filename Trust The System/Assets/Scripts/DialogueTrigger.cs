using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }

    public void NextSentence()
    {
        FindFirstObjectByType<DialogueManager>().DisplayNextSentence();
    }
}
