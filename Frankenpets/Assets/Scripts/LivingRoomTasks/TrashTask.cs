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
    public TMPro.TextMeshProUGUI taskLabel;
    public Task task = new Task("Throw away shoes", 1);

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;


    private TutorialText tutorialText;

    void Awake()
    {
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
        if (ff1InTrash || ff2InTrash)
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
        taskLabel.fontStyle = TMPro.FontStyles.Strikethrough;
        task.IsComplete = true;
        FindTasks.Instance.DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
        TaskManager.Instance.CompleteTask();
        AudioManager.Instance.PlayTaskCompletionSound();
    }
}
