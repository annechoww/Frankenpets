using UnityEngine;
using UnityEngine.UI;

public class MoveRugTask : MonoBehaviour
{
    public Image taskItem;
    public Color completedColor;
    public GameObject openedAtticDoor;
    public Rigidbody rug;

    private TutorialText tutorialText;
    private Task task = new Task("Move Rug", 0);

    private float minTime = 3f;
    private float timeSinceCollision = 0f;
    private bool isColliding = false;

    void Awake()
    {
        TaskManager.RegisterTask(task);
        rug.constraints = RigidbodyConstraints.FreezeAll;
        foreach (Transform child in rug.transform)
        {
            child.gameObject.tag = "Untagged";
        }
    }

    void Update()
    {
        if (ArePriorTasksComplete())
        {
            if (rug != null)
                {
                    rug.constraints = RigidbodyConstraints.None;
                    foreach (Transform child in rug.transform)
                    {
                        child.gameObject.tag = "Draggable";
                    }
                }
        }

        if (!isColliding)
        {
            timeSinceCollision += Time.deltaTime;

            if (timeSinceCollision >= minTime)
            {
                FinishTask();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Rug")
        {
            isColliding = true;
            timeSinceCollision = 0f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Rug")
        {
            isColliding = false;
        }
    }

    private void FinishTask()
    {
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();

        // Destroy the closed attic door after rug is removed 
        Destroy(gameObject);

        taskItem.color = completedColor;
        if (tutorialText.getCurrTutorialStage() == TutorialText.tutDragRug)
        {
            tutorialText.advanceTutorialStage();
        }

        OpenAtticDoor();
        gameObject.SetActive(false);
        task.IsComplete = true;

        TaskManager.Instance.CompleteTask();
    }

    public static bool ArePriorTasksComplete()
    {
        Task shatterVaseTask = TaskManager.FindTaskByName("Shatter Vase");
        Task scatterBoxesTask = TaskManager.FindTaskByName("Scatter Boxes");

        return shatterVaseTask != null && scatterBoxesTask != null && shatterVaseTask.IsComplete && scatterBoxesTask.IsComplete;
    }

    void OpenAtticDoor()
    {
        openedAtticDoor.SetActive(true);
    }

}
