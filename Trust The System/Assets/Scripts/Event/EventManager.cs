using UnityEngine;

public class EventManager : UnityEngine.MonoBehaviour
{
    public void DestroyObject(Event _event)
    {
        Destroy(_event.target);
    }
}
