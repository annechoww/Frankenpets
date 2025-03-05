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
    }

    // void OnCollisionExit(Collision collision)
    // {
    //     Debug.Log("Exit " + collision.transform.name);

    //     if (collision.transform.name == "Rug") 
    //     {
    //         FinishTask();
    //     }
    // }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "Rug")
        {
            if (ArePriorTasksComplete())
            {
                FinishTask();
            }
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
        // Invoke("OpenAtticDoor", 2f);
        OpenAtticDoor();
        gameObject.SetActive(false);
        task.IsComplete = true;
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
