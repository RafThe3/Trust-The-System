using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Ray")]
    public float distance = 1;

    [Header("UI")]
    public TextMeshProUGUI interactTextBox;

    private readonly string interactText = "Press E to Interact";
    private readonly string dialogueText = "Press E to Continue";

    private bool gameIsPaused = false;

    private void Start()
    {
        interactTextBox.enabled = false;
    }

    private void Update()
    {
        gameIsPaused = Time.timeScale == 0;

        Dialogue();

        if (gameIsPaused) interactTextBox.enabled = false;
    }

    private void Dialogue()
    {
        if (gameIsPaused)
        {
            return;
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, distance))
        {
            Debug.Log("Hitting something...");

            Collider npc = hit.collider;
            DialogueTrigger dialogueTrigger = npc.GetComponent<DialogueTrigger>();
            bool hasDialogue = npc.TryGetComponent(out DialogueTrigger _);
            bool isNPC = npc.CompareTag("NPC");
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();

            if (!isNPC)
            {
                return;
            }

            interactTextBox.enabled = true;
            interactTextBox.text = !dialogueManager.DialogueEnabled ? interactText : dialogueText;
            interactTextBox.transform.position = Camera.main.WorldToScreenPoint(npc.transform.position);

            if (hasDialogue && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Hitting E...");

                if (!dialogueManager.DialogueEnabled)
                {
                    dialogueTrigger.TriggerDialogue();
                }
                else
                {
                    dialogueTrigger.NextSentence();
                }
            }
        }
        else
        {
            interactTextBox.enabled = false;
        }
    }
}
