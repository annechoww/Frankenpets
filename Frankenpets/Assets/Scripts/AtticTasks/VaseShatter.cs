using UnityEngine;
using UnityEngine.UI;

public class VaseShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenVase;
    public float shatterForce = 1f;
    public AudioClip shatterSound;


    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;

    public Task task = new Task("Shatter Vase", 0);
    private bool isShattered = false;
    private TutorialText tutorialText;

    void Awake()
    {
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
        audioSource = GameObject.Find("Background Music").GetComponent<AudioSource>(); 
        TaskManager.RegisterTask(task);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the vase hits the ground with enough force
        if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
        {
            ShatterVase();
        }
    }
    
    void ShatterVase()
    {
        isShattered = true;

        
        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position);
        }

        // Instantiate the broken vase at the vase's position and rotation
        Instantiate(brokenVase, transform.position, transform.rotation);

        // Destroy the intact vase after shattering
        Destroy(gameObject);

        FinishTask();
    }

    private void FinishTask(){
        taskItem.color = completedColor;
        tutorialText.advanceTutorialStage();
        task.IsComplete = true;

        TaskManager.Instance.CompleteTask();
    }
}
