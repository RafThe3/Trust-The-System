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

        if (gameIsPaused)
        {
            interactTextBox.enabled = false;
            return;
        }

        Dialogue();
        Interactable();
    }

    private void Dialogue()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, distance))
        {
            Debug.Log("Hitting something...");

            Collider npc = hit.collider;
            DialogueTrigger dialogueTrigger = npc.GetComponent<DialogueTrigger>();
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            bool hasDialogue = npc.TryGetComponent(out DialogueTrigger _);
            bool isNPC = npc.CompareTag("NPC");

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

    private void Interactable()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, distance))
        {
            Collider interactable = hit.collider;
            ObjectiveTrigger objectiveTrigger = interactable.GetComponent<ObjectiveTrigger>();
            bool isInteractable = interactable.CompareTag("Interactable");
            bool hasTrigger = interactable.TryGetComponent(out ObjectiveTrigger _);

            if (!isInteractable)
            {
                return;
            }

            interactTextBox.enabled = true;
            interactTextBox.text = interactText;
            interactTextBox.transform.position = Camera.main.WorldToScreenPoint(interactable.transform.position);

            if (hasTrigger && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Hitting E...");

                objectiveTrigger.GiveObjective();
            }
        }
    }
}
