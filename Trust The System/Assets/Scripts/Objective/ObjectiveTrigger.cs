using TMPro;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    public Objective objective;

    public void GiveObjective()
    {
        if (!objective.giveObjective)
        {
            return;
        }

        DialogueTrigger other = objective.dialogueTrigger;
        Dialogue dialogue = other.dialogue;

        ObjectiveManager.Instance.GiveObjective(objective);

        dialogue.dialogue = objective.dialogue; // changes the DIALOGUE of the NPC specified in the dialogue class
        dialogue.objective = objective.objective_2; // changes the OBJECTIVE of the NPC specified in the dialogue class
        dialogue.objectiveTrigger = objective.interactableToEnable; // changes the INTERACTABLE to be enabled when next dialogue if finished
        dialogue.enableInteractable = true;
        dialogue.disableDialogueAfterInteract = objective.disableDialogueAfterInteract; 
        dialogue.other = objective.other;
        dialogue.enableOtherDialogue = objective.enableOtherDialogue;
    }
}