using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Ray")]
    public float distance = 1;

    private void Update()
    {
        Dialogue();
    }

    private void Dialogue()
    {
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
    }
}
