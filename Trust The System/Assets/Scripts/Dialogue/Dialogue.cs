using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [Header("Dialogue")]
    public string name;
    [TextArea(3, 5)] public string[] dialogue;
    [Min(0)] public float dialogueSpeed = 1;
    public bool enableDialogue = true;
    public bool disableDialogueAfterInteract = false;

    [Header("Objective")]
    [TextArea(3, 5)] public string objective;
    public bool giveObjective = false;
    public ObjectiveTrigger objectiveTrigger;
    public bool enableInteractable = false;

    [Header("NPC (to enable dialogue)")]
    public DialogueTrigger other;
    public bool enableOtherDialogue = false;
}
