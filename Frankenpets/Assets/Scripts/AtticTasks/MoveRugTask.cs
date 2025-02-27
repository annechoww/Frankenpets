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
                if (rug != null)
                {
                    rug.constraints = RigidbodyConstraints.None;
                }
               
                FinishTask();
            }
        }
    }

    private void FinishTask()
    {
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();

        // Destroy the intact vase after shattering
        Destroy(gameObject);

        taskItem.color = completedColor;
        if (tutorialText.getCurrTutorialStage() == TutorialText.tutDragRug)
        {
            tutorialText.advanceTutorialStage();
        }

        openedAtticDoor.SetActive(true);
        gameObject.SetActive(false);
        task.IsComplete = true;
    }

    private bool ArePriorTasksComplete()
    {
        Task shatterVaseTask = TaskManager.FindTaskByName("Shatter Vase");
        Task scatterBoxesTask = TaskManager.FindTaskByName("Scatter Boxes");

        return shatterVaseTask != null && scatterBoxesTask != null && shatterVaseTask.IsComplete && scatterBoxesTask.IsComplete;
    }

}
