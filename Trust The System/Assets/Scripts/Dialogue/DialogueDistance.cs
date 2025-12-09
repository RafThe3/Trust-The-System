using UnityEngine;

public class DialogueDistance : MonoBehaviour
{
    public float dialogueDistance = 1;

    private void Update()
    {
        Transform player = GameObject.FindWithTag("Player").transform;
        Vector3 playerPosition = transform.position - player.position;
        bool isPlayerNear = playerPosition.magnitude < dialogueDistance;

        if (!isPlayerNear)
        {
            FindFirstObjectByType<DialogueManager>().ShowDialogue(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dialogueDistance);
    }
}
