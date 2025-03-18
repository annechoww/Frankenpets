using UnityEngine;
using UnityEngine.UI;

public class JugShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenJug;
    public float shatterForce = 1f;
    public AudioClip shatterSound;
    // public AudioClip taskCompleteSound;

    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;

    public Task task = new Task("Shatter Jug", 1);
    private bool isShattered = false;
    // private TutorialText tutorialText;

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;

    void Awake()
    {
        // tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
        TaskManager.RegisterTask(task);
    }

    void OnCollisionEnter(Collision collision)
    {

        // Check if the jug hits the ground with enough force
        if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
        {
            ShatterJug();
        }
    }
    

    void ShatterJug()
    {
        isShattered = true;

        
        if (shatterSound != null)
        {
            // AudioSource.PlayClipAtPoint(shatterSound, transform.position);
            AudioManager.Instance.PlaySFX(shatterSound);
        }

        // Instantiate the broken jug at the jug's position and rotation
        Instantiate(brokenJug, transform.position, transform.rotation);

        // Destroy the intact jug after shattering
        Destroy(gameObject);

        FinishTask();
    }

    private void FinishTask(){
        taskItem.color = completedColor;
        // tutorialText.advanceTutorialStage();
        task.IsComplete = true;
        FindTasks.Instance.DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
        TaskManager.Instance.CompleteTask();
        AudioManager.Instance.PlayTaskCompletionSound();
    }
}
