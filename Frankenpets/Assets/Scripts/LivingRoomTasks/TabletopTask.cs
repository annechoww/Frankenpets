using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletopTask : MonoBehaviour
{
    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;
    public AudioClip taskCompleteSound;

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;

    public Task task = new Task("Empty Tabletop", 1);
    // private TutorialText tutorialText;
    private HashSet<Collider> itemsOnTable = new HashSet<Collider>();
    private bool taskCompleted = false;
    private AudioSource audioSource;

    void Awake()
    {
        // tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
        audioSource = GameObject.Find("Background Music").GetComponent<AudioSource>(); 
        TaskManager.RegisterTask(task);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            itemsOnTable.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            itemsOnTable.Remove(other);
        }
    }

    private void Update()
    {
        if (itemsOnTable.Count == 0 && !taskCompleted)
        {
            Debug.Log("All items are off the table!");
            taskCompleted = true;
            FinishTask();
        }
    }

    private void FinishTask(){
        taskItem.color = completedColor;
        // tutorialText.advanceTutorialStage();
        task.IsComplete = true;
        // GetComponent<FindTasks>().DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
        if (taskCompleteSound != null)
        {
            audioSource.PlayOneShot(taskCompleteSound);
        }
    }
}
