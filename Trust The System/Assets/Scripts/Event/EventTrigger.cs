using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public Event _event;

    public void DestroyObject()
    {
        FindFirstObjectByType<EventManager>().DestroyObject(_event);
    }
}
