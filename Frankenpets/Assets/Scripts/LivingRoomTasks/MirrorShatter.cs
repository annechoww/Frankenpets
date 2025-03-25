using UnityEngine;
using UnityEngine.UI;

public class MirrorShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenMirror;
    public float shatterForce = 1f;
    public AudioClip shatterSound;

    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;
    public TMPro.TextMeshProUGUI taskLabel;
    public bool isTask = false;

    public Task task = new Task("Shatter Mirror", 1);
    private bool isShattered = false;
    
    void Awake()
    {
        if (isTask) TaskManager.RegisterTask(task);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the mirror hits the ground with enough force
        if (isTask && !isShattered && collision.relativeVelocity.magnitude > shatterForce)
        {
            ShatterMirror();
        }
    }
    

    void ShatterMirror()
    {
        isShattered = true;

        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position);
        }

        // Instantiate the broken mirror at the mirror's position and rotation
        Instantiate(brokenMirror, transform.position, transform.rotation);

        // Destroy the intact mirror after shattering
        Destroy(gameObject);

        if (isTask) FinishTask();
    }

    private void FinishTask(){
        taskItem.color = completedColor;
        taskLabel.fontStyle = TMPro.FontStyles.Strikethrough;
        // tutorialText.advanceTutorialStage();
        task.IsComplete = true;
        
        TaskManager.Instance.CompleteTask();
        AudioManager.Instance.PlayTaskCompletionSound();
    }
}
