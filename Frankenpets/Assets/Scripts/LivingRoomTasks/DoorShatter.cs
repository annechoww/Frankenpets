using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DoorShatter : MonoBehaviour
{
    [Header("References")]
    public GameObject brokenGlass;
    public float shatterForce = 1f;
    public AudioClip shatterSound;

    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;

    public Task task = new Task("Shatter Door", 1);
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

        if (collision.gameObject.CompareTag("Grabbable"))
        {
            // Check if the glass hits the ground with enough force
            if (!isShattered && collision.relativeVelocity.magnitude > shatterForce)
            {
                ShatterGlass();
            }
        }
    }
    

    void ShatterGlass()
    {
        isShattered = true;

        
        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, transform.position);
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
            // tutorialText.advanceTutorialStage();
        }

    }
}
