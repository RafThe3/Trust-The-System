using System.Collections;
using TMPro;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI objectiveTextBox;
    public AudioClip objectiveSFX;

    private float textSpeed = 1;
    private bool gameIsPaused = false;

    private void Start()
    {
        objectiveTextBox.enabled = !gameIsPaused;
    }

    private void Update()
    {
        gameIsPaused = Time.timeScale == 0;
    }

    public void GiveObjective(Objective objective)
    {
        textSpeed = objective.textSpeed;

        if (objective.giveObjective && objectiveTextBox.text != objective.objective)
        {
            StopAllCoroutines();
            StartCoroutine(Write(objective.objective, objectiveTextBox));
        }
    }

    private IEnumerator Write(string sentence, TextMeshProUGUI textBox)
    {
        textBox.text = string.Empty;

        foreach (char letter in sentence.ToCharArray())
        {
            textBox.text += letter;
            if (letter.ToString() != " ") Camera.main.GetComponent<AudioSource>().PlayOneShot(objectiveSFX);
            yield return new WaitForSeconds(0.1f / textSpeed);
        }
    }
}