using UnityEngine;

public class AmmoBox : UnityEngine.MonoBehaviour
{
    public int amountToGive = 30;

    private void OnTriggerEnter(Collider other)
    {
        Shooting[] weapons = FindObjectsByType<Shooting>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (Shooting weapon in weapons)
        {
            weapon.AddAmmo(amountToGive);
        }

        Destroy(gameObject);
    }
}
