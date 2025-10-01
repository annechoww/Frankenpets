using UnityEngine;
using UnityEngine.UI;

public class TrashTask : MonoBehaviour
{
    [Header("References")]
    public GameObject rollingPin;
    public AudioClip thrownInTrashSound;
    private bool inTrash = false;
    public AudioClip taskCompleteSound;
    private AudioSource audioSource;

    [Header("Task Manager")]
    public GameObject taskItem;
    //public Color completedColor;
    //public TMPro.TextMeshProUGUI taskLabel;
    public Task task = new Task("Throw away rolling pin", 1);

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
        if (other.gameObject == rollingPin)
        {
            inTrash = true;
            CheckTaskCompletion();
        }

        if (thrownInTrashSound != null)
        {
            AudioManager.Instance.PlaySFX(thrownInTrashSound);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == rollingPin)
        {
            inTrash = false;
        }
    }

    void CheckTaskCompletion()
    {
        if (inTrash)
        {
            if (!task.IsComplete)
            {
                FinishTask();
            }
        }
    }

    private void FinishTask()
    {
        //taskItem.color = completedColor;
        //taskLabel.fontStyle = TMPro.FontStyles.Strikethrough;
        taskItem.transform.GetChild(1).gameObject.SetActive(true);
        task.IsComplete = true;
        FindTasks.Instance.DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
        TaskManager.Instance.CompleteTask();
        AudioManager.Instance.PlayTaskCompletionSound();
    }
}
