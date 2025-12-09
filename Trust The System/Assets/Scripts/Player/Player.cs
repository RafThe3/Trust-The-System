using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public bool infiniteStamina = false;
    [Min(0)] public float sprintDuration = 1;
    [Min(0)] public float sprintCooldownTime = 1;
    [Min(0)] public float staminaRecoverySpeed = 1;

    [Header("Ray")]
    public float distance = 1;

    [Header("UI")]
    public TextMeshProUGUI interactTextBox;
    public Image crosshair;
    public Slider staminaBar;
    public Image staminaBarFillArea;

    private readonly string interactText = "Press E to Interact";
    private readonly string dialogueText = "Press E to Continue";

    private bool isOnSprintCooldown;
    private FirstPersonController controller;
    private CharacterController character;
    private Image[] staminaBarImages;
    private bool isMoving;

    private bool gameIsPaused = false;

    private void Awake()
    {
        controller = GetComponent<FirstPersonController>();
        character = GetComponent<CharacterController>();
    }

    private void Start()
    {
        interactTextBox.enabled = false;

        staminaBar.maxValue = sprintDuration;
        staminaBar.value = sprintDuration;
        staminaBarImages = staminaBar.GetComponentsInChildren<Image>();
        staminaBarFillArea.color = Color.yellow;
    }

    private void Update()
    {
        gameIsPaused = Time.timeScale == 0;

        ToggleUI();

        if (gameIsPaused)
        {
            interactTextBox.enabled = false;
            return;
        }

        isMoving = Mathf.Abs(character.velocity.x) > Mathf.Epsilon || Mathf.Abs(character.velocity.z) > Mathf.Epsilon;

        Sprint();
        Dialogue();
        Interactable();
    }

    private void ToggleUI()
    {
        foreach (Image image in staminaBarImages)
        {
            image.enabled = !gameIsPaused;
        }

        crosshair.enabled = !gameIsPaused;
    }

    private void Sprint()
    {
        if (infiniteStamina)
        {
            ToggleImages(false);
            return;
        }

        if (staminaBar.value <= 0)
        {
            isOnSprintCooldown = true;
            controller.SprintSpeed = controller.MoveSpeed;
            staminaBarFillArea.color = Color.red;
        }

        if (isOnSprintCooldown && staminaBar.value >= sprintCooldownTime)
        {
            isOnSprintCooldown = false;
            controller.SprintSpeed = controller._tempSprintSpeed;
            staminaBarFillArea.color = Color.yellow;
        }

        if (controller._isSprinting && !isOnSprintCooldown && isMoving)
        {
            ToggleImages(true);
            staminaBar.value -= Time.deltaTime;
        }
        else if (staminaBar.value < staminaBar.maxValue)
        {
            staminaBar.value += Time.deltaTime * staminaRecoverySpeed;
        }

        if (staminaBar.value == staminaBar.maxValue)
        {
            ToggleImages(false);
        }

        void ToggleImages(bool toggle)
        {
            foreach (Image image in staminaBarImages)
            {
                image.enabled = toggle;
            }
        }
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

            interactTextBox.enabled = dialogueManager.CanTalk;
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

            interactTextBox.enabled = objectiveTrigger.objective.giveObjective;
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
