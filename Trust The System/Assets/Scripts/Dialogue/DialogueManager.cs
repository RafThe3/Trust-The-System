using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class DialogueManager : UnityEngine.MonoBehaviour
{
    #region Singleton

    public static DialogueManager Instance;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning($"More than one instance of DialogueManager found on {name}");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    [Header("Dialogue")]
    public TextMeshProUGUI dialogueTextBox;
    public TextMeshProUGUI nameTextBox;
    public TextMeshProUGUI objectiveTextBox;
    public AudioSource sfxSource;
    public AudioClip dialogueSFX;

    private Queue<string> dialogue;
    private float dialogueSpeed;
    private bool isTalking = false;
    private bool showDialogue = false;
    private bool enableDialogue = true;
    private bool disableAfterInteract = false;

    private ObjectiveTrigger objectiveTrigger;
    private Dialogue dialogue1;
    private Dialogue other;
    private bool enableInteractable = false;
    private bool enableOtherDialogue = false;
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

        dialogueTextBox.enabled = showDialogue && !gameIsPaused;
        nameTextBox.enabled = showDialogue && !gameIsPaused;
        objectiveTextBox.enabled = !gameIsPaused;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogue1 = dialogue;
        enableDialogue = dialogue.enableDialogue;

        if (!enableDialogue)
        {
            return;
        }

        Debug.Log($"{dialogue.name} is talking...");

        showDialogue = true;
        dialogueSpeed = dialogue.dialogueSpeed;
        nameTextBox.text = dialogue.name;
        disableAfterInteract = dialogue.disableDialogueAfterInteract;
        enableInteractable = dialogue.enableInteractable;
        enableOtherDialogue = dialogue.enableOtherDialogue;

        if (enableInteractable)
        {
            objectiveTrigger = dialogue.objectiveTrigger;
        }

        if (enableOtherDialogue)
        {
            other = dialogue.other.dialogue;
        }

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
        if (!enableDialogue)
        {
            return;
        }

        if (dialogue.Count == 0)
        {
            EndDialogue();
            return;
        }

        sfxSource.clip = dialogueSFX;

        string sentence = dialogue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(Write(sentence, dialogueTextBox));

        Debug.Log(sentence);
    }

    private IEnumerator Write(string sentence, TextMeshProUGUI textBox)
    {
        isTalking = true;

        textBox.text = string.Empty;

        foreach (char letter in sentence.ToCharArray())
        {
            textBox.text += letter;
            if (letter.ToString() != " ") sfxSource.Play();
            yield return new WaitForSeconds(0.1f / dialogueSpeed);
        }

        isTalking = false;
    }

    private void EndDialogue()
    {
        Debug.Log("End of conversation...");

        dialogue.Clear();
        showDialogue = false;
        dialogueTextBox.text = string.Empty;
        nameTextBox.text = string.Empty;

        if (giveObjective && objectiveTextBox.text != objective)
        {
            sfxSource.clip = FindFirstObjectByType<ObjectiveManager>().objectiveSFX;
            StopAllCoroutines();
            StartCoroutine(Write(objective, objectiveTextBox));

            if (enableInteractable && objectiveTrigger)
            {
                objectiveTrigger.objective.giveObjective = true;
            }
        }

        if (enableOtherDialogue)
        {
            other.enableDialogue = true;
        }

        if (disableAfterInteract)
        {
            dialogue1.enableDialogue = false;
        }
    }

    public bool IsTalking => isTalking;

    public bool ShowingDialogue => showDialogue;

    public bool DialogueEnabled => enableDialogue;

    public void ShowDialogue(bool showDialogue)
    {
        this.showDialogue = showDialogue;
    }

    public void EnableDialogue(bool enable)
    {
        enableDialogue = enable;
    }
}
