using TMPro;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    public Objective objective;

    public void GiveObjective()
    {
        DialogueTrigger other = objective.dialogueTrigger;

        FindFirstObjectByType<ObjectiveManager>().GiveObjective(objective);
        other.dialogue.dialogue = objective.dialogue; // changes the DIALOGUE of the NPC specified in the dialogue class
        other.dialogue.objective = objective.objective_2; // changes the OBJECTIVE of the NPC specified in the dialogue class
    }
}