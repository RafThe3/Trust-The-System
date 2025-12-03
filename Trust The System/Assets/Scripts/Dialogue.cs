using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3,5)] public string[] dialogue;
    [Min(0)] public float dialogueSpeed = 1;
}
