using UnityEngine;

public class EventTrigger : UnityEngine.MonoBehaviour
{
    public Event _event;

    public void DestroyObject()
    {
        FindFirstObjectByType<EventManager>().DestroyObject(_event);
    }
}
