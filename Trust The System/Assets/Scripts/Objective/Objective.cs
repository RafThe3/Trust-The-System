using System;
using UnityEngine;

[Serializable]
public class Objective
{
    [Header("Objective (of current object)")]
    [TextArea(3, 5)] public string objective;
    public bool giveObjective = false;
    public bool disableAfterInteract = false;
    public float textSpeed = 1;

    [Header("Dialogue (to replace)")]
    public DialogueTrigger dialogueTrigger;
    [TextArea(3, 5)] public string[] dialogue;

    [Header("Objective (to replace)")]
    [TextArea(3, 5)] public string objective_2;
    public ObjectiveTrigger interactableToEnable;
}
