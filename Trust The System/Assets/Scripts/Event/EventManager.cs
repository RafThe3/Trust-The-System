using UnityEngine;

public class EventManager : MonoBehaviour
{
    public void DestroyObject(Event _event)
    {
        Destroy(_event.target);
    }
}
