using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue")]
    public TextMeshProUGUI dialogueTextBox;
    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI objectiveTextBox;
    
    public AudioClip dialogueSFX;

    private Queue<string> dialogue;
    private float dialogueSpeed;
    private bool isTalking = false;
    private bool dialogueEnabled = false;

    private string objective;
    private bool giveObjective = false;
    
    private bool gameIsPaused = false;

    private void Start()
    {
        dialogue = new();
    }

    private void Update()
    {
        gameIsPaused = Time.timeScale == 0;

        dialogueTextBox.enabled = dialogueEnabled && !gameIsPaused;
        nameTextBox.enabled = dialogueEnabled && !gameIsPaused;
        objectiveTextBox.enabled = !gameIsPaused;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log($"{dialogue.name} is talking...");

        dialogueEnabled = true;
        dialogueSpeed = dialogue.dialogueSpeed;
        nameTextBox.text = dialogue.name;

        objective = dialogue.objective;
        giveObjective = dialogue.giveObjective;

        this.dialogue.Clear();

        foreach (string sentences in dialogue.dialogue)
        {
            this.dialogue.Enqueue(sentences);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = dialogue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(Write(sentence, dialogueTextBox, dialogueSFX));

        Debug.Log(sentence);
    }

    private IEnumerator Write(string sentence, TextMeshProUGUI textBox, AudioClip sfx)
    {
        isTalking = true;

        textBox.text = string.Empty;

        foreach (char letter in sentence.ToCharArray())
        {
            textBox.text += letter;
            if (letter.ToString() != " ") Camera.main.GetComponent<AudioSource>().PlayOneShot(sfx);
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

        if (giveObjective && objectiveTextBox.text != objective)
        {
            StopAllCoroutines();
            StartCoroutine(Write(objective, objectiveTextBox, FindFirstObjectByType<ObjectiveManager>().objectiveSFX));
        }
    }

    public bool IsTalking => isTalking;

    public bool DialogueEnabled => dialogueEnabled;

    public void EnableDialogue(bool enable)
    {
        dialogueEnabled = enable;
    }
}
