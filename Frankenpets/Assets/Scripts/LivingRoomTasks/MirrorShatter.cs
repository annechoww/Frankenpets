using UnityEngine;

public class MirrorShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenMirror;
    public float shatterForce = 1f;
    public AudioClip shatterSound;

    // [Header("Task Manager")]
    // public Image taskItem;
    // public Color completedColor;

    public Task task = new Task("Shatter Mirror", 1);
    private bool isShattered = false;
    // private TutorialText tutorialText;

    void Awake()
    {
        // tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
        TaskManager.RegisterTask(task);
    }

    void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        // Check if the mirror hits the ground with enough force
        if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
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

        FinishTask();
    }

    private void FinishTask(){
        // taskItem.color = completedColor;
        // tutorialText.advanceTutorialStage();
        task.IsComplete = true;
    }
}
