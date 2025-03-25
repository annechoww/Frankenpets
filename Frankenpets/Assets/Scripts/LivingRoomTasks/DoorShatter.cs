using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenGlass;
    public float shatterForce = 1f;
    public AudioClip shatterSound;
    public AudioClip crackSound;

    [Header("Glass Materials")]
    public Material intactGlassMaterial;
    public Material crackedGlassMaterial;

    [Header("Required Object")]
    public Collider specificCollider;
    

    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;

    public Task task = new Task("Shatter Door", 1);
    private bool isShattered = false;
    private bool isCracked = false;


    // private TutorialText tutorialText;

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;

    private Renderer glassRenderer;

    void Awake()
    {
        // tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
        TaskManager.RegisterTask(task);

        glassRenderer = GetComponent<Renderer>();

        if (glassRenderer != null && intactGlassMaterial != null)
        {
            if (intactGlassMaterial == null) {
                intactGlassMaterial = glassRenderer.material;
            }
            else {
                glassRenderer.material = intactGlassMaterial;
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.collider == specificCollider)
        {
            // Check if the glass hits the ground with enough force
            if (!isShattered)
            {
                ShatterGlass();
            }
        }
        else {
            PlayerActions pa = collision.gameObject.GetComponent<PlayerActions>();
            if (pa != null && pa.IsDashing && collision.relativeVelocity.magnitude > shatterForce) {
                if (!isShattered) {
                    if (isCracked) {
                        ShatterGlass();
                    }
                    else if (!isCracked) {
                        CrackGlass();
                    }
                }
            }
        }
    }

    void CrackGlass()
    {
        isCracked = true;
        Debug.Log("Glass cracked!");
        
        // Change to cracked glass material
        if (glassRenderer != null && crackedGlassMaterial != null)
        {
            glassRenderer.material = crackedGlassMaterial;
        }
        
        // Play cracking sound
        if (crackSound != null)
        {
            AudioManager.Instance.PlaySFX(crackSound);
        }
    }


    

    void ShatterGlass()
    {
        isShattered = true;

        
        if (shatterSound != null)
        {
           AudioManager.Instance.PlaySFX(shatterSound);
        }

        // Instantiate the broken glass at the glass's position and rotation
        Instantiate(brokenGlass, transform.position, transform.rotation);

        // Destroy the intact glass after shattering
        Destroy(gameObject);

        FinishTask();
    }

    private void FinishTask(){
        task.IsComplete = true;
        IsComplete();
        // GetComponent<FindTasks>().DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
    }

    private void IsComplete()
    {
        List<Task> doorTasks = TaskManager.FindTasksByName("Shatter Door");
        bool taskComplete = TaskManager.CheckTaskCompletion(doorTasks);
        Debug.Log(taskComplete);
        if (taskComplete)
        {
            taskItem.color = completedColor;

            FindTasks.Instance.DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
            TaskManager.Instance.CompleteTask();
            AudioManager.Instance.PlayTaskCompletionSound();
        }

    }
}
