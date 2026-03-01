using System;
using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : UnityEngine.MonoBehaviour
{
    [Header("Movement")]
    public bool infiniteStamina = false;
    [Min(0)] public float sprintDuration = 1;
    [Min(0)] public float sprintCooldownTime = 1;
    [Min(0)] public float staminaRecoverySpeed = 1;

    [Header("Ray")]
    [Min(0)] public float distance = 1;

    [Header("UI")]
    public TextMeshProUGUI interactTextBox, gameSavedTextBox;
    public GameObject crosshair;
    public Slider staminaBar;
    public Image staminaBarFillArea;

    private bool isOnSprintCooldown;
    private FirstPersonController controller;
    private CharacterController character;
    private Image[] staminaBarImages;
    private bool isMoving;

    private bool gameIsPaused = false;
    private Camera mainCamera;

    private void Awake()
    {
        controller = GetComponent<FirstPersonController>();
        character = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        interactTextBox.enabled = false;
        gameSavedTextBox.enabled = false;

        staminaBar.maxValue = sprintDuration;
        staminaBar.value = sprintDuration;
        staminaBarImages = staminaBar.GetComponentsInChildren<Image>();
        staminaBarFillArea.color = Color.yellow;

        Debug.Log(Application.persistentDataPath);
        LoadPlayer();
    }

    private void Update()
    {
        gameIsPaused = Time.timeScale == 0;

        ToggleUI();

        if (gameIsPaused)
        {
            interactTextBox.enabled = false;
            gameSavedTextBox.enabled = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayer();
        }

        isMoving = Mathf.Abs(character.velocity.x) > Mathf.Epsilon || Mathf.Abs(character.velocity.z) > Mathf.Epsilon;

        Sprint();

        /*
        Dialogue();
        Interactable();
        Checkpoint();
        */

        CheckInteraction();
    }

    private void ToggleUI()
    {
        foreach (Image image in staminaBarImages)
        {
            image.enabled = !gameIsPaused;
        }

        Image[] crosshairImages = crosshair.GetComponentsInChildren<Image>();
        
        foreach (Image crosshairImage in crosshairImages)
        {
            crosshairImage.enabled = !gameIsPaused;
        }

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

    /*
    private void Dialogue()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, distance))
        {
            Debug.Log("Hitting something...");

            Collider npc = hit.collider;
            DialogueTrigger dialogueTrigger = npc.GetComponent<DialogueTrigger>();
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            Dialogue dialogue = null;
            bool hasDialogue = npc.TryGetComponent(out DialogueTrigger _);
            bool isNPC = npc.CompareTag("NPC");

            if (!isNPC)
            {
                return;
            }

            if (hasDialogue)
            {
                dialogue = dialogueTrigger.dialogue;
            }

            interactTextBox.enabled = dialogue.enableDialogue;
            interactTextBox.text = !dialogueManager.ShowingDialogue ? dialogueText : continueText;
            interactTextBox.transform.position = mainCamera.WorldToScreenPoint(npc.transform.position);

            if (interactTextBox.transform.position.z <= 0)
            {
                interactTextBox.enabled = false;
            }

            if (dialogue.enableDialogue && hasDialogue && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Hitting E...");

                if (!dialogueManager.ShowingDialogue)
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
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, distance))
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
            interactTextBox.transform.position = mainCamera.WorldToScreenPoint(interactable.transform.position);

            if (hasTrigger && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Hitting E...");

                objectiveTrigger.GiveObjective();
            }
        }
        else
        {
            interactTextBox.enabled = false;
        }
    }

    private void Checkpoint()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, distance))
        {
            Collider checkpoint = hit.collider;
            bool isCheckpoint = checkpoint.CompareTag("Checkpoint");

            if (!isCheckpoint)
            {
                return;
            }

            interactTextBox.enabled = isCheckpoint;
            interactTextBox.text = hasSaved ? "Game Saved" : saveText;
            interactTextBox.transform.position = mainCamera.WorldToScreenPoint(checkpoint.transform.position + Vector3.up);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Hitting E...");

                Player player = this;
                SaveSystem.Save(ref player);
                Debug.Log("Saved game...");
            }
        }
        else
        {
            interactTextBox.enabled = false;
        }
    }
    */

    private void CheckInteraction()
    {
        string interactText = "Press E to Interact";
        string dialogueText = "Press E to Talk";
        string pickupText = "Press E to Pickup";
        string saveText = "Press E to Save Game";
        string continueText = "Press E to Continue";

        bool hasInteracted = Input.GetKeyDown(KeyCode.E);

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, distance))
        {
            Collider interactable = hit.collider;

            DialogueTrigger dialogueTrigger = interactable.GetComponent<DialogueTrigger>();
            DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
            Dialogue dialogue = null;
            bool hasDialogue = interactable.TryGetComponent(out DialogueTrigger _);
            bool isNPC = interactable.CompareTag("NPC");

            ObjectiveTrigger objectiveTrigger = interactable.GetComponent<ObjectiveTrigger>();
            bool isInteractable = interactable.CompareTag("Interactable");
            bool hasTrigger = interactable.TryGetComponent(out ObjectiveTrigger _);

            bool isCheckpoint = interactable.CompareTag("Checkpoint");

            bool isItem = interactable.CompareTag("Item");

            if (isNPC)
            {
                if (hasDialogue)
                {
                    dialogue = dialogueTrigger.dialogue;
                }

                interactTextBox.enabled = dialogue.enableDialogue;
                interactTextBox.text = !dialogueManager.ShowingDialogue ? dialogueText : continueText;
                interactTextBox.transform.position = mainCamera.WorldToScreenPoint(interactable.transform.position);
                
                if (hasDialogue && hasInteracted && dialogue.enableDialogue)
                {
                    if (!dialogueManager.ShowingDialogue)
                    {
                        dialogueTrigger.TriggerDialogue();
                    }
                    else
                    {
                        dialogueTrigger.NextSentence();
                    }
                }
                return;
            }

            if (isInteractable)
            {
                interactTextBox.enabled = objectiveTrigger.objective.giveObjective;
                interactTextBox.text = interactText;
                interactTextBox.transform.position = mainCamera.WorldToScreenPoint(interactable.transform.position);

                if (hasTrigger && hasInteracted)
                {
                    objectiveTrigger.GiveObjective();
                }
                return;
            }

            if (isCheckpoint)
            {
                interactTextBox.enabled = isCheckpoint;
                interactTextBox.text = saveText;
                Vector3 offset = Vector3.up;
                interactTextBox.transform.position = mainCamera.WorldToScreenPoint(interactable.transform.position + offset);

                if (hasInteracted)
                {
                    // Save the game
                    SavePlayer();

                    StartCoroutine(EnableTextBox(gameSavedTextBox, 5));
                }
                return;
            }

            if (isItem)
            {
                interactTextBox.enabled = isItem;
                interactTextBox.text = pickupText;
                interactTextBox.transform.position = mainCamera.WorldToScreenPoint(interactable.transform.position);

                if (hasInteracted)
                {
                    // Pick up item
                    Debug.Log($"Picking up {interactable.name}");
                    GameObject item = interactable.gameObject;
                    bool itemPickedUp = Inventory.Instance.Add(item);
                    if (itemPickedUp)
                    {
                        Destroy(item);
                    }
                }
                return;
            }
        }

        interactTextBox.enabled = false;
    }

    private IEnumerator EnableTextBox(TextMeshProUGUI text, float duration)
    {
        text.enabled = true;
        yield return new WaitForSeconds(duration);
        text.enabled = false;
    }

    public void SavePlayer()
    {
        Player player = this;
        SaveSystem.Save(ref player);
        Debug.Log("Saved game...");
    }

    public void LoadPlayer()
    {
        PlayerData playerData = SaveSystem.Load();

        // player position
        Vector3 newPlayerPos = new(playerData.x, playerData.y, playerData.z);

        controller.enabled = false;
        transform.position = newPlayerPos;
        Physics.SyncTransforms();
        controller.enabled = true;

        Debug.Log("Loaded game...");
        Debug.Log($"Save Coordinates: {playerData.x}, {playerData.y}, {playerData.z}");
    }
}

[Serializable]
public class PlayerData
{
    public float x, y, z;
    public int sceneIndex;

    public PlayerData(Player player)
    {
        x = player.transform.position.x;
        y = player.transform.position.y;
        z = player.transform.position.z;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
