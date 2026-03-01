using UnityEngine;

public class DialogueTrigger : UnityEngine.MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void NextSentence()
    {
        DialogueManager.Instance.DisplayNextSentence();
    }
}
