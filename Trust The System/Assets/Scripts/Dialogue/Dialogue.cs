using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [Header("Dialogue")]
    public string name;
    [TextArea(3,5)] public string[] dialogue;
    [Min(0)] public float dialogueSpeed = 1;

    [Header("Objective")]
    [TextArea(3, 5)] public string objective;
    public bool giveObjective = false;
}
