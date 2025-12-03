using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueTextBox;
    public TextMeshProUGUI nameTextBox;
    public AudioClip dialogueSFX;

    private Queue<string> dialogue;
    private float dialogueSpeed;
    private bool isTalking = false;
    private bool dialogueEnabled = false;

    private void Start()
    {
        dialogue = new();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log($"{dialogue.name} is talking...");

        dialogueEnabled = true;

        dialogueSpeed = dialogue.dialogueSpeed;

        this.dialogue.Clear();

        foreach (string sentences in dialogue.dialogue)
        {
            this.dialogue.Enqueue(sentences);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(dialogue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = dialogue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(Write(sentence));

        Debug.Log(sentence);
    }

    private IEnumerator Write(string sentence)
    {
        isTalking = true;

        dialogueTextBox.text = string.Empty;
        //if (textBox == dialogueTextBox) nameTextBox.text = objectName;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTextBox.text += letter;
            if (letter.ToString() != " ") Camera.main.GetComponent<AudioSource>().PlayOneShot(dialogueSFX);
            yield return new WaitForSeconds(0.1f / dialogueSpeed);
        }

        isTalking = false;
    }

    private void EndDialogue()
    {
        Debug.Log("End of conversation...");

        dialogue.Clear();
        dialogueEnabled = false;
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;
    }

    public bool IsTalking => isTalking;

    public bool DialogueEnabled => dialogueEnabled;
}
