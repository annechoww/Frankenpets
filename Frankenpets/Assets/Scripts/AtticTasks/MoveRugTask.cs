using UnityEngine;
using UnityEngine.UI;

public class MoveRugTask : MonoBehaviour
{
    public Image taskItem;
    public Color completedColor;
    public GameObject openedAtticDoor;

    private TutorialText tutorialText;

    void Awake()
    {
        tutorialText = GameObject.Find("TutorialTextManager").GetComponent<TutorialText>();
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
            FinishTask();
        }
    }

    private void FinishTask()
    {
        // Destroy the intact vase after shattering
        Destroy(gameObject);

        taskItem.color = completedColor;
        if (tutorialText.getCurrTutorialStage() == TutorialText.tutDragRug)
        {
            tutorialText.advanceTutorialStage();
        }

        openedAtticDoor.SetActive(true);
        gameObject.SetActive(false);
    }
}
