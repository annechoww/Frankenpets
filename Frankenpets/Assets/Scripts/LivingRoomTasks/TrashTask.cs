using UnityEngine;
using UnityEngine.UI;

public class TrashTask : MonoBehaviour
{
    [Header("References")]
    public GameObject flipFlop1;
    public GameObject flipFlop2;
    private bool ff1InTrash = false;
    private bool ff2InTrash = false;
    public AudioClip taskCompleteSound;
    private AudioSource audioSource;

    [Header("Task Manager")]
    public Image taskItem;
    public Color completedColor;

    public Task task = new Task("Throw away shoes", 1);
    private TutorialText tutorialText;

    void Awake()
    {
        // tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
        audioSource = GameObject.Find("Background Music").GetComponent<AudioSource>(); 
        TaskManager.RegisterTask(task);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == flipFlop1)
        {
            ff1InTrash = true;
            CheckTaskCompletion();
        }

        if (other.gameObject == flipFlop2)
        {
            ff2InTrash = true;
            CheckTaskCompletion();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == flipFlop1)
        {
            ff1InTrash = false;
        }

        if (other.gameObject == flipFlop2)
        {
            ff2InTrash = false;
        }
    }

    void CheckTaskCompletion()
    {
        if (ff1InTrash && ff2InTrash)
        {
            if (!task.IsComplete)
            {
                FinishTask();
            }
        }
    }

    private void FinishTask()
    {
        taskItem.color = completedColor;
        // tutorialText.advanceTutorialStage();
        task.IsComplete = true;
        if (taskCompleteSound != null)
        {
            audioSource.PlayOneShot(taskCompleteSound);
        }
    }
}
